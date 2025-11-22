using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class PaymentStatusRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La descripción del estado es requerida")]
    [StringLength(100, ErrorMessage = "La descripción del estado no puede exceder 100 caracteres")]
    public string StatusDescription { get; set; } = null!;
}

public class PaymentStatusResponse : PaymentStatusRequest
{
    public int TotalExpenses { get; set; }
    public int TotalServiceExpenses { get; set; }
    public int TotalServicePayments { get; set; }
}