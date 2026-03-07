namespace Condominio.Models;

public partial class IncidentCost
{
  public int Id { get; set; }

  public int IncidentTypeId { get; set; }

  public decimal Cost { get; set; }

  public DateTime StartDate { get; set; }

  public DateTime? EndDate { get; set; }

  public string Description { get; set; } = null!;

  public virtual IncidentType IncidentType { get; set; } = null!;
}
