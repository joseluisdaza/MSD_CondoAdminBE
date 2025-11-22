namespace Condominio.Models;

public partial class ExpensePayment
{
    public int Id { get; set; }

    public int ExpenseId { get; set; }

    public int PaymentId { get; set; }

    public virtual Expense Expense { get; set; } = null!;

    public virtual Payment Payment { get; set; } = null!;
}