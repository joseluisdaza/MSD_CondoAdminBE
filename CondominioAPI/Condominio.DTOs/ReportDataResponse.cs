namespace Condominio.DTOs
{
  public class ReportDataResponse
  {
    public object? Header { get; set; }
    public object Body { get; set; } = null!;
    public object? Footer { get; set; }
  }
}
