namespace Condominio.Models;

public partial class IncidentType
{
  public int Id { get; set; }

  public string Type { get; set; } = null!;

  public string Description { get; set; } = null!;

  public virtual ICollection<IncidentCost> IncidentCosts { get; set; } = new List<IncidentCost>();

  public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
