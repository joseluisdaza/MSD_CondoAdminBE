namespace Condominio.DTOs
{
  public class IncidentTypeRequest
  {
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;
  }
}
