namespace Condominio.DTOs
{
  public class ReportPdfResponse
  {
    public string FilePath { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public bool Success { get; set; }
    public string? Error { get; set; }
  }
}
