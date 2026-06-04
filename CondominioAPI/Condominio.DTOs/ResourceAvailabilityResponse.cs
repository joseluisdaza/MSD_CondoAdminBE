namespace Condominio.DTOs
{
  public class ResourceAvailabilityResponse
  {
    public bool IsAvailable { get; set; }
    public string Message { get; set; } = null!;
  }
}
