using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class PaymentRequest
{
    [Required(ErrorMessage = "El número de recibo es requerido")]
    [StringLength(100, ErrorMessage = "El número de recibo no puede exceder 100 caracteres")]
    public string ReceiveNumber { get; set; } = null!;

    [Required(ErrorMessage = "La fecha de pago es requerida")]
    public DateTime PaymentDate { get; set; }

    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Amount { get; set; }

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "La foto del recibo es requerida")]
    [StringLength(1000, ErrorMessage = "La URL de la foto no puede exceder 1000 caracteres")]
    public string ReceivePhoto { get; set; } = null!;
}

public class PaymentResponse : PaymentRequest
{
    public int Id { get; set; }
    public List<ExpensePaymentResponse> Expenses { get; set; } = new List<ExpensePaymentResponse>();
}