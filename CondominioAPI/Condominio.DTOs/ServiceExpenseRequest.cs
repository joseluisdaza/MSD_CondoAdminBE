using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class ServiceExpenseRequest
{
    [Required(ErrorMessage = "El ID del tipo de servicio es requerido")]
    public int ServiceTypeId { get; set; }

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "La fecha límite de pago es requerida")]
    public DateTime PaymentLimitDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El monto de interés debe ser mayor o igual a 0")]
    public decimal? InterestAmount { get; set; }

    [Required(ErrorMessage = "El monto total es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto total debe ser mayor a 0")]
    public decimal TotalAmount { get; set; }

    [Range(1, 4, ErrorMessage = "El estado debe estar entre 1 y 4")]
    public int Status { get; set; } = 1;

    [Required(ErrorMessage = "La fecha del gasto es requerida")]
    public DateTime ExpenseDate { get; set; }

    public int StatusId { get; set; } = 0;
}

public class ServiceExpenseResponse : ServiceExpenseRequest
{
    public int Id { get; set; }
    public string? ServiceTypeName { get; set; }
    public string? StatusDescription { get; set; }
    public List<ServiceExpensePaymentResponse> Payments { get; set; } = new List<ServiceExpensePaymentResponse>();
}