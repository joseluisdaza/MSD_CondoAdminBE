namespace Condominio.Models;

public partial class ServiceExpense
{
    public int Id { get; set; }

    public int ServiceTypeId { get; set; }

    public string Description { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime PaymentLimitDate { get; set; }

    public decimal? InterestAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public int Status { get; set; }

    public DateTime ExpenseDate { get; set; }

    public int StatusId { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;

    public virtual PaymentStatus StatusNavigation { get; set; } = null!;

    public virtual ICollection<ServiceExpensePayment> ServiceExpensePayments { get; set; } = new List<ServiceExpensePayment>();
}