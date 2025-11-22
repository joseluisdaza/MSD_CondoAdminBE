using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class ServiceExpensePaymentRequest
{
    [Required(ErrorMessage = "El ID del gasto de servicio es requerido")]
    public int ServiceExpenseId { get; set; }

    [Required(ErrorMessage = "El ID del pago es requerido")]
    public int PaymentId { get; set; }
}

public class ServiceExpensePaymentResponse : ServiceExpensePaymentRequest
{
    public int Id { get; set; }
    public string? ServiceExpenseDescription { get; set; }
    public decimal? ServiceExpenseAmount { get; set; }
    public string? PaymentReceiveNumber { get; set; }
    public decimal? PaymentAmount { get; set; }
    public DateTime? PaymentDate { get; set; }
}