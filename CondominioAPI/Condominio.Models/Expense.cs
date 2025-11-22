namespace Condominio.Models;

public partial class Expense
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public int? PropertyId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime PaymentLimitDate { get; set; }

    public decimal Amount { get; set; }

    public decimal? InterestAmount { get; set; }

    public decimal? InterestRate { get; set; }

    public string Description { get; set; } = null!;

    public int StatusId { get; set; }

    public virtual ExpenseCategory Category { get; set; } = null!;

    public virtual Property? Property { get; set; }

    public virtual PaymentStatus Status { get; set; } = null!;

    public virtual ICollection<ExpensePayment> ExpensePayments { get; set; } = new List<ExpensePayment>();
}
