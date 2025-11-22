namespace Condominio.Models;

public partial class ServiceExpensePayment
{
    public int Id { get; set; }

    public int ServiceExpenseId { get; set; }

    public int PaymentId { get; set; }

    public virtual ServiceExpense ServiceExpense { get; set; } = null!;

    public virtual ServicePayment Payment { get; set; } = null!;
}