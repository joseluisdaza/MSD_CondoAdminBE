using Condominio.DTOs;

namespace Condominio.Reports.Models
{
  public abstract class AbstractReportOutput
  {
    public bool Success { get; set; }
  }

  public class JsonReportOutput : AbstractReportOutput
  {
    public ReportExecutionResponse Content { get; set; }
  }

  public class FileReportOutput : AbstractReportOutput
  {
    public string FilePath { get; set; }
    public string FileName { get; set; }
  }
}
