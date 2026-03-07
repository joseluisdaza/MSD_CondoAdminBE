namespace Condominio.DTOs
{
  public class IncidentRequest
  {
    public int Id { get; set; }

    public int IncidentTypeId { get; set; }

    public int UserId { get; set; }

    public int PropertyId { get; set; }

    public int StatusId { get; set; }

    public DateTime IncidentDate { get; set; }

    public string? IncidentDescription { get; set; }

    public string? IncidentPhoto { get; set; }
  }
}
