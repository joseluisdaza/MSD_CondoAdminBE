using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class ExpensePaymentRequest
{
    [Required(ErrorMessage = "El ID del gasto es requerido")]
    public int ExpenseId { get; set; }

    [Required(ErrorMessage = "El ID del pago es requerido")]
    public int PaymentId { get; set; }
}

public class ExpensePaymentResponse : ExpensePaymentRequest
{
    public int Id { get; set; }
    public string? ExpenseDescription { get; set; }
    public decimal? ExpenseAmount { get; set; }
    public string? PaymentReceiveNumber { get; set; }
    public decimal? PaymentAmount { get; set; }
    public DateTime? PaymentDate { get; set; }
}