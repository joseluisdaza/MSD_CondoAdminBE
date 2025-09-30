namespace Condominio.Models;

public partial class ExpenseCategory
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
