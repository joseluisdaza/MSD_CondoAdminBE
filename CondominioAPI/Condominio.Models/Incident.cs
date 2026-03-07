namespace Condominio.Models;

public partial class Incident
{
  public int Id { get; set; }

  public int IncidentTypeId { get; set; }

  public int UserId { get; set; }

  public int PropertyId { get; set; }

  public int StatusId { get; set; }

  public DateTime IncidentDate { get; set; }

  public string? IncidentDescription { get; set; }

  public string? IncidentPhoto { get; set; }

  public virtual IncidentType IncidentType { get; set; } = null!;

  public virtual User User { get; set; } = null!;

  public virtual Property Property { get; set; } = null!;

  public virtual PaymentStatus Status { get; set; } = null!;
}
