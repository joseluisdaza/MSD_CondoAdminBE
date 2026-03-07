namespace Condominio.DTOs
{
  public class ResourceRequest
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public string Photo { get; set; } = null!;
  }

  public class FullResourceRequest : ResourceRequest
  {
    public DateTime? EndDate { get; set; }
  }
}
