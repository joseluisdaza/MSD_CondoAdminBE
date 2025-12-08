using Condominio.DTOs;
using Condominio.Repository.Repositories;
using Condominio.Utils;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace CondominioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly IExpenseCategoryRepository _expenseCategoryRepository;

        public ExpenseCategoriesController(IExpenseCategoryRepository expenseCategoryRepository)
        {
            _expenseCategoryRepository = expenseCategoryRepository;
        }

        /// <summary>
        /// Obtiene todas las categorías de gastos (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpenseCategoryResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > ExpenseCategories > GetAll. User: {0}", User.Identity?.Name);
                
                var categories = await _expenseCategoryRepository.GetAllWithExpensesAsync();
                var categoryResponses = categories.Select(c => c.ToExpenseCategoryResponse()).ToList();
                
                return Ok(categoryResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all expense categories");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una categoría de gasto por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseCategoryResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > ExpenseCategories > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var category = await _expenseCategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound($"Categoría de gasto con ID {id} no encontrada");
                }

                return Ok(category.ToExpenseCategoryResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense category by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene una categoría de gasto por nombre de categoría
        /// </summary>
        [HttpGet("category/{categoryName}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseCategoryResponse>> GetByCategory(string categoryName)
        {
            try
            {
                Log.Information("GET > ExpenseCategories > ByCategory. User: {0}, Category: {1}", 
                    User.Identity?.Name, categoryName);
                
                var category = await _expenseCategoryRepository.GetByCategoryAsync(categoryName);
                if (category == null)
                {
                    return NotFound($"Categoría de gasto '{categoryName}' no encontrada");
                }

                return Ok(category.ToExpenseCategoryResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense category by name: {0}", categoryName);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea una nueva categoría de gasto (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseCategoryResponse>> Create([FromBody] ExpenseCategoryRequest categoryRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > ExpenseCategories > Create. User: {0}, Category: {1}", 
                    User.Identity?.Name, categoryRequest.Category);

                // Validar que no existe otra categoría con el mismo nombre
                var existingCategory = await _expenseCategoryRepository.GetByCategoryAsync(categoryRequest.Category);
                if (existingCategory != null)
                {
                    Log.Warning("ExpenseCategory creation failed - Category already exists: {0}", categoryRequest.Category);
                    return BadRequest($"Ya existe una categoría de gasto con el nombre '{categoryRequest.Category}'");
                }

                var category = categoryRequest.ToExpenseCategory();
                await _expenseCategoryRepository.AddAsync(category);
                
                Log.Information("ExpenseCategory created successfully. ID: {0}, Category: {1}", 
                    category.Id, category.Category);
                
                var createdCategory = await _expenseCategoryRepository.GetByIdAsync(category.Id);
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, 
                    createdCategory?.ToExpenseCategoryResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating expense category");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza una categoría de gasto existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseCategoryResponse>> Update(int id, [FromBody] ExpenseCategoryRequest categoryRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > ExpenseCategories > Update. User: {0}, ID: {1}, Category: {2}", 
                    User.Identity?.Name, id, categoryRequest.Category);

                if (id != categoryRequest.Id)
                {
                    return BadRequest("El ID del parámetro no coincide con el ID del objeto");
                }

                var existingCategory = await _expenseCategoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound($"Categoría de gasto con ID {id} no encontrada");
                }

                // Validar que no existe otra categoría con el mismo nombre (excepto la actual)
                var duplicateCategory = await _expenseCategoryRepository.GetByCategoryAsync(categoryRequest.Category);
                if (duplicateCategory != null && duplicateCategory.Id != id)
                {
                    Log.Warning("ExpenseCategory update failed - Category already exists: {0} (current ID: {1}, existing ID: {2})", 
                        categoryRequest.Category, id, duplicateCategory.Id);
                    return BadRequest($"Ya existe otra categoría de gasto con el nombre '{categoryRequest.Category}'");
                }

                // Actualizar las propiedades
                existingCategory.Category = categoryRequest.Category;
                existingCategory.Description = categoryRequest.Description;

                await _expenseCategoryRepository.UpdateAsync(existingCategory);
                
                Log.Information("ExpenseCategory updated successfully. ID: {0}, Category: {1}", id, categoryRequest.Category);

                var updatedCategory = await _expenseCategoryRepository.GetByIdAsync(id);
                return Ok(updatedCategory?.ToExpenseCategoryResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating expense category with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina una categoría de gasto (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > ExpenseCategories > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingCategory = await _expenseCategoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound($"Categoría de gasto con ID {id} no encontrada");
                }

                // Verificar que no tenga gastos relacionados
                if (existingCategory.Expenses?.Any() == true)
                {
                    return BadRequest("No se puede eliminar la categoría de gasto porque tiene gastos relacionados. " +
                                      "Elimine primero los gastos o asígnelos a otra categoría.");
                }

                await _expenseCategoryRepository.DeleteAsync(id);

                Log.Information("ExpenseCategory deleted successfully. ID: {0}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting expense category with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}