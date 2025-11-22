namespace Condominio.Models;

public partial class ServiceType
{
    public int Id { get; set; }

    public string ServiceName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<ServiceExpense> ServiceExpenses { get; set; } = new List<ServiceExpense>();
}