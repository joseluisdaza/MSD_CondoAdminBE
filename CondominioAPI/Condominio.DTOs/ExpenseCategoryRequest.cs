using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class ExpenseCategoryRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La categoría es requerida")]
    [StringLength(100, ErrorMessage = "La categoría no puede exceder 100 caracteres")]
    public string Category { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = null!;
}

public class ExpenseCategoryResponse
{
    public int Id { get; set; }
    public string Category { get; set; } = null!;
    public string Description { get; set; } = null!;
    //public int TotalExpenses { get; set; }
}