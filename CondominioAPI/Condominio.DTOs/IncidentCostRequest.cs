namespace Condominio.DTOs
{
  public class IncidentCostRequest
  {
    public int Id { get; set; }

    public int IncidentTypeId { get; set; }

    public decimal Cost { get; set; }

    public DateTime StartDate { get; set; }

    public string Description { get; set; } = null!;
  }

  public class FullIncidentCostRequest : IncidentCostRequest
  {
    public DateTime? EndDate { get; set; }
  }
}
