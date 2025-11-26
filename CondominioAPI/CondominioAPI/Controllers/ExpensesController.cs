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
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IExpenseCategoryRepository _categoryRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IPaymentStatusRepository _paymentStatusRepository;

        public ExpensesController(
            IExpenseRepository expenseRepository,
            IExpenseCategoryRepository categoryRepository,
            IPropertyRepository propertyRepository,
            IPaymentStatusRepository paymentStatusRepository)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _propertyRepository = propertyRepository;
            _paymentStatusRepository = paymentStatusRepository;
        }

        /// <summary>
        /// Obtiene todos los gastos (Solo Administradores y Directores)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetAll()
        {
            try
            {
                Log.Information("GET > Expenses > GetAll. User: {0}", User.Identity?.Name);
                
                var expenses = await _expenseRepository.GetAllWithRelationsAsync();
                var expenseResponses = expenses.Select(e => e.ToExpenseResponse()).ToList();
                
                return Ok(expenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting all expenses");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene un gasto por ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseResponse>> GetById(int id)
        {
            try
            {
                Log.Information("GET > Expenses > ById. User: {0}, Id: {1}", User.Identity?.Name, id);
                
                var expense = await _expenseRepository.GetByIdWithRelationsAsync(id);
                if (expense == null)
                {
                    return NotFound($"Gasto con ID {id} no encontrado");
                }

                return Ok(expense.ToExpenseResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expense by ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos por propiedad
        /// </summary>
        [HttpGet("property/{propertyId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetByPropertyId(int propertyId)
        {
            try
            {
                Log.Information("GET > Expenses > ByPropertyId. User: {0}, PropertyId: {1}", User.Identity?.Name, propertyId);
                
                var expenses = await _expenseRepository.GetByPropertyIdAsync(propertyId);
                var expenseResponses = expenses.Select(e => e.ToExpenseResponse()).ToList();
                
                return Ok(expenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expenses by property ID: {0}", propertyId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos por estado
        /// </summary>
        [HttpGet("status/{statusId}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetByStatusId(int statusId)
        {
            try
            {
                Log.Information("GET > Expenses > ByStatusId. User: {0}, StatusId: {1}", User.Identity?.Name, statusId);
                
                var expenses = await _expenseRepository.GetByStatusIdAsync(statusId);
                var expenseResponses = expenses.Select(e => e.ToExpenseResponse()).ToList();
                
                return Ok(expenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting expenses by status ID: {0}", statusId);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene gastos vencidos
        /// </summary>
        [HttpGet("overdue")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Super}")]
        public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetOverdueExpenses()
        {
            try
            {
                Log.Information("GET > Expenses > Overdue. User: {0}", User.Identity?.Name);
                
                var expenses = await _expenseRepository.GetOverdueExpensesAsync();
                var expenseResponses = expenses.Select(e => e.ToExpenseResponse()).ToList();
                
                return Ok(expenseResponses);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error getting overdue expenses");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Crea un nuevo gasto (Solo Administradores)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseResponse>> Create([FromBody] ExpenseRequest expenseRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("POST > Expenses > Create. User: {0}, Expense: {@Expense}", User.Identity?.Name, expenseRequest);

                // Validar que la categoría existe
                var category = await _categoryRepository.GetByIdAsync(expenseRequest.CategoryId);
                if (category == null)
                {
                    return BadRequest("La categoría especificada no existe");
                }

                // Validar que la propiedad existe (si se especificó)
                if (expenseRequest.PropertyId.HasValue)
                {
                    var property = await _propertyRepository.GetByIdAsync(expenseRequest.PropertyId.Value);
                    if (property == null)
                    {
                        return BadRequest("La propiedad especificada no existe");
                    }
                }

                // Validar que el estado existe
                var status = await _paymentStatusRepository.GetByIdAsync(expenseRequest.StatusId);
                if (status == null)
                {
                    return BadRequest("El estado especificado no existe");
                }

                var expense = expenseRequest.ToExpense();
                await _expenseRepository.AddAsync(expense);
                
                var createdExpense = await _expenseRepository.GetByIdWithRelationsAsync(expense.Id);
                return CreatedAtAction(nameof(GetById), new { id = expense.Id }, createdExpense?.ToExpenseResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating expense");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Actualiza un gasto existente (Solo Administradores)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult<ExpenseResponse>> Update(int id, [FromBody] ExpenseRequest expenseRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Log.Information("PUT > Expenses > Update. User: {0}, Id: {1}, Expense: {@Expense}", User.Identity?.Name, id, expenseRequest);

                var existingExpense = await _expenseRepository.GetByIdAsync(id);
                if (existingExpense == null)
                {
                    return NotFound($"Gasto con ID {id} no encontrado");
                }

                // Validar que la categoría existe
                var category = await _categoryRepository.GetByIdAsync(expenseRequest.CategoryId);
                if (category == null)
                {
                    return BadRequest("La categoría especificada no existe");
                }

                // Validar que la propiedad existe (si se especificó)
                if (expenseRequest.PropertyId.HasValue)
                {
                    var property = await _propertyRepository.GetByIdAsync(expenseRequest.PropertyId.Value);
                    if (property == null)
                    {
                        return BadRequest("La propiedad especificada no existe");
                    }
                }

                // Validar que el estado existe
                var status = await _paymentStatusRepository.GetByIdAsync(expenseRequest.StatusId);
                if (status == null)
                {
                    return BadRequest("El estado especificado no existe");
                }

                // Actualizar las propiedades
                existingExpense.CategoryId = expenseRequest.CategoryId;
                existingExpense.PropertyId = expenseRequest.PropertyId;
                existingExpense.StartDate = expenseRequest.StartDate;
                existingExpense.PaymentLimitDate = expenseRequest.PaymentLimitDate;
                existingExpense.Amount = expenseRequest.Amount;
                existingExpense.InterestAmount = expenseRequest.InterestAmount;
                existingExpense.InterestRate = expenseRequest.InterestRate;
                existingExpense.Description = expenseRequest.Description;
                existingExpense.StatusId = expenseRequest.StatusId;

                await _expenseRepository.UpdateAsync(existingExpense);

                var updatedExpense = await _expenseRepository.GetByIdWithRelationsAsync(id);
                return Ok(updatedExpense?.ToExpenseResponse());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating expense with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Elimina un gasto (Solo Administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Log.Information("DELETE > Expenses > Delete. User: {0}, Id: {1}", User.Identity?.Name, id);

                var existingExpense = await _expenseRepository.GetByIdAsync(id);
                if (existingExpense == null)
                {
                    return NotFound($"Gasto con ID {id} no encontrado");
                }

                await _expenseRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deleting expense with ID: {0}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}