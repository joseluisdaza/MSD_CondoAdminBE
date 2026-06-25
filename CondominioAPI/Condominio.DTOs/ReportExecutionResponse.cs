namespace Condominio.DTOs
{
  public class ReportExecutionResponse
  {
    public string Title { get; set; } = null!;
    public int StyleId { get; set; }
    public IEnumerable<ReportContentItem> Headers { get; set; } = new List<ReportContentItem>();
    public IEnumerable<ReportContentItem> Sections { get; set; } = new List<ReportContentItem>();
    public IEnumerable<ReportContentItem> Footers { get; set; } = new List<ReportContentItem>();
  }
}
