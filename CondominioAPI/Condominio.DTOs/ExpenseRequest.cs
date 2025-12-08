using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class ExpenseRequest
{
    [Required(ErrorMessage = "El ID de la categoría es requerido")]
    public int CategoryId { get; set; }

    public int? PropertyId { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "La fecha límite de pago es requerida")]
    public DateTime PaymentLimitDate { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Amount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El monto de interés debe ser mayor o igual a 0")]
    public decimal? InterestAmount { get; set; }

    [Range(0, 100, ErrorMessage = "La tasa de interés debe estar entre 0 y 100")]
    public decimal? InterestRate { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = null!;

    public int StatusId { get; set; } = 0;
}

public class ExpenseResponse : ExpenseRequest
{
    public int Id { get; set; }
    public string? CategoryName { get; set; }
    public string? PropertyCode { get; set; }
    public string? PropertyTower { get; set; }
    public string? StatusDescription { get; set; }
    //public List<ExpensePaymentResponse> Payments { get; set; } = new List<ExpensePaymentResponse>();
}