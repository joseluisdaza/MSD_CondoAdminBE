using Condominio.DTOs;

namespace Condominio.Reports
{
  /// <summary>
  /// Implementación de generador de reportes que produce salida en formato JSON (ReportExecutionResponse).
  /// Esta clase es agnóstica de la base de datos y solo trabaja con datos proporcionados.
  /// </summary>
  public class JsonReportGenerator : IReportGenerator
  {
    /// <summary>
    /// Genera un reporte en formato JSON a partir de los datos proporcionados.
    /// </summary>
    /// <param name="reportData">Datos del reporte a generar</param>
    /// <returns>ReportExecutionResponse con la estructura del reporte</returns>
    public object Generate(ReportExecutionData reportData)
    {
      if (reportData == null)
        throw new ArgumentNullException(nameof(reportData));

      var response = new ReportExecutionResponse
      {
        Title = reportData.Title,
        StyleId = reportData.TitleStyleId,
        Headers = ConvertPartsToContentItems(reportData.HeaderParts),
        Sections = ConvertPartsToContentItems(reportData.SectionParts),
        Footers = ConvertPartsToContentItems(reportData.FooterParts)
      };

      return response;
    }

    /// <summary>
    /// Convierte una colección de ReportPartData a ReportContentItem para la respuesta.
    /// </summary>
    private IEnumerable<ReportContentItem> ConvertPartsToContentItems(IEnumerable<ReportPartData> parts)
    {
      if (parts == null)
        return new List<ReportContentItem>();

      return parts.Select(part => new ReportContentItem
      {
        Text = part.Content,
        StyleId = part.StyleId,
        IsTable = part.IsTable
      }).OrderBy(item => parts.FirstOrDefault(p => p.Content == ((ReportContentItem)item)?.Text)?.DisplayOrder ?? 0);
    }
  }
}
