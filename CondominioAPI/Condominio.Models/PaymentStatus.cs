namespace Condominio.Models;

public partial class PaymentStatus
{
    public int Id { get; set; }

    public string StatusDescription { get; set; } = null!;

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public virtual ICollection<ServiceExpense> ServiceExpenses { get; set; } = new List<ServiceExpense>();

    public virtual ICollection<ServicePayment> ServicePayments { get; set; } = new List<ServicePayment>();
}