namespace Condominio.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string ReceiveNumber { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public string ReceivePhoto { get; set; } = null!;

    public virtual ICollection<ExpensePayment> ExpensePayments { get; set; } = new List<ExpensePayment>();
}
