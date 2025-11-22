namespace Condominio.Models;

public partial class ServicePayment
{
    public int Id { get; set; }

    public string ReceiveNumber { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string ReceivePhoto { get; set; } = null!;

    public int StatusId { get; set; }

    public virtual PaymentStatus Status { get; set; } = null!;

    public virtual ICollection<ServiceExpensePayment> ServiceExpensePayments { get; set; } = new List<ServiceExpensePayment>();
}