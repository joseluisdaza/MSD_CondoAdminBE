using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using Serilog;
using System.Collections;
using System.Text.Json;
using Condominio.Reports.Models;

namespace Condominio.Reports
{
  /// <summary>
  /// Servicio para generar archivos PDF basados en datos de reportes usando QuestPDF
  /// </summary>
  public class PdfReportGenerator : IReportGenerator
  {
    /// <summary>
    /// Directorio donde se almacenan los reportes generados
    /// </summary>
    private readonly string _reportsDirectory;

    public PdfReportGenerator()
    {
      _reportsDirectory = Path.Combine(AppContext.BaseDirectory, "Reports");

      // Crear el directorio si no existe
      if (!Directory.Exists(_reportsDirectory))
      {
        Directory.CreateDirectory(_reportsDirectory);
        Log.Information("Reports directory created at: {0}", _reportsDirectory);
      }

      // Configurar QuestPDF
      QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Genera un reporte en formato PDF
    /// </summary>
    /// <param name="reportData">Datos del reporte a generar</param>
    /// <returns>Ruta del archivo PDF generado</returns>
    public AbstractReportOutput Generate(ReportExecutionData reportData)
    {
      try
      {
        var fileName = $"Report_{reportData.Title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
        var filePath = Path.Combine(_reportsDirectory, fileName);

        // Crear documento PDF
        Document.Create(container =>
        {
          container.Page(page =>
          {
            page.Size(PageSizes.Letter);
            page.Margin(1, Unit.Centimetre);

            page.Header().Column(col =>
            {
              // Título principal
              col.Item().Text(reportData.Title).FontSize(18).Bold();

              col.Item().PaddingVertical(5);

              // Headers del reporte
              if (reportData.HeaderParts.Any())
              {
                foreach (var headerPart in reportData.HeaderParts.OrderBy(x => x.DisplayOrder))
                {
                  RenderPart(col, headerPart, reportData.AvailableStyles);
                }
              }
            });

            page.Content().Column(col =>
            {
              // Secciones principales
              foreach (var bodyPart in reportData.SectionParts.OrderBy(x => x.DisplayOrder))
              {
                RenderPart(col, bodyPart, reportData.AvailableStyles);
              }
            });

            page.Footer().Column(col =>
            {
              // Footers del reporte
              if (reportData.FooterParts.Any())
              {
                foreach (var footerPart in reportData.FooterParts.OrderBy(x => x.DisplayOrder))
                {
                  RenderPart(col, footerPart, reportData.AvailableStyles);
                }
              }

              col.Item().PaddingVertical(5);

              // Número de página
              col.Item().Text(x =>
              {
                x.Span("Página ");
                x.CurrentPageNumber();
                x.Span(" de ");
                x.TotalPages();
              });
            });
          });
        }).GeneratePdf(filePath);

        Log.Information("PDF Report generated successfully at: {0}", filePath);
        return new FileReportOutput
        {
          FilePath = filePath,
          FileName = fileName,
          Success = true
        };
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error generating PDF report: {0}", reportData.Title);
        throw;
      }
    }

    /// <summary>
    /// Renderiza una parte del reporte (header, body o footer)
    /// </summary>
    private void RenderPart(ColumnDescriptor col, ReportPartData part, IEnumerable<ReportStyleData> availableStyles)
    {
      var style = availableStyles.FirstOrDefault(s => s.Id == part.StyleId);

      if (part.IsTable && part.Content is IEnumerable<object> tableData && !(part.Content is string))
      {
        RenderTable(col, tableData.ToList(), style);
      }
      else
      {
        RenderParagraph(col, part.Content, style);
      }
    }

    /// <summary>
    /// Renderiza un párrafo con estilo aplicado
    /// </summary>
    private void RenderParagraph(ColumnDescriptor col, object? content, ReportStyleData? style)
    {
      if (content == null)
        return;

      var text = content is string str ? str : JsonSerializer.Serialize(content);
      var fontSize = style?.FontSize > 0 ? style.FontSize : 11;

      if (style?.Bold ?? false)
      {
        col.Item().Text(text).FontSize(fontSize).Bold();
      }
      else if (style?.Italic ?? false)
      {
        col.Item().Text(text).FontSize(fontSize).Italic();
      }
      else
      {
        col.Item().Text(text).FontSize(fontSize);
      }
    }

    /// <summary>
    /// Renderiza una tabla
    /// </summary>
    //private void RenderTable(ColumnDescriptor col, List<object> data, ReportStyleData? style)
    //{
    //  if (!data.Any())
    //    return;

    //  var firstItem = data.First();
    //  var properties = GetObjectProperties(firstItem);

    //  if (!properties.Any())
    //    return;

    //  col.Item().Table(table =>
    //  {
    //    // Definir columnas
    //    table.ColumnsDefinition(columns =>
    //    {
    //      for (int i = 0; i < properties.Count; i++)
    //      {
    //        columns.RelativeColumn(1);
    //      }
    //    });

    //    // Encabezado
    //    foreach (var prop in properties)
    //    {
    //      table.Header(header =>
    //      {
    //        header.Cell()
    //          .Background("E0E0E0")
    //          .Padding(5)
    //          .Text(prop.Key)
    //          .Bold()
    //          .FontSize(10);
    //      });
    //    }

    //    // Filas de datos
    //    foreach (var item in data)
    //    {
    //      foreach (var prop in properties)
    //      {
    //        table.Cell()
    //          .Padding(5)
    //          .Text(prop.Value(item)?.ToString() ?? "")
    //          .FontSize(9);
    //      }
    //    }
    //  });
    //}

    private void RenderTable(ColumnDescriptor col, List<object> data, ReportStyleData? style)
    {
      if (!data.Any())
        return;

      var firstItem = data.First();
      var properties = GetObjectProperties(firstItem);

      if (!properties.Any())
        return;

      col.Item().Table(table =>
      {
        // Definir columnas
        table.ColumnsDefinition(columns =>
        {
          for (int i = 0; i < properties.Count; i++)
          {
            columns.RelativeColumn(1);
          }
        });

        // Encabezado — UNA sola llamada a Header(), con todas las celdas dentro
        table.Header(header =>
        {
          foreach (var prop in properties)
          {
            header.Cell()
                .Background("E0E0E0")
                .Padding(5)
                .Text(prop.Key)
                .Bold()
                .FontSize(10);
          }
        });

        // Filas de datos
        foreach (var item in data)
        {
          foreach (var prop in properties)
          {
            table.Cell()
                .Padding(5)
                .Text(prop.Value(item)?.ToString() ?? "")
                .FontSize(9);
          }
        }
      });
    }

    /// <summary>
    /// Obtiene las propiedades de un objeto para usarlas como columnas de tabla
    /// </summary>
    private List<KeyValuePair<string, Func<object, object?>>> GetObjectProperties(object obj)
    {
      var properties = new List<KeyValuePair<string, Func<object, object?>>>();

      var type = obj.GetType();
      var props = type.GetProperties();

      foreach (var prop in props)
      {
        // Skip indexed properties (e.g., Item[int]) to avoid parameter count mismatch
        if (prop.GetIndexParameters().Length > 0)
          continue;

        var key = prop.Name;
        var getter = new Func<object, object?>(o => 
        {
          try
          {
            return prop.GetValue(o);
          }
          catch
          {
            // If property cannot be read, return null
            return null;
          }
        });
        properties.Add(new KeyValuePair<string, Func<object, object?>>(key, getter));
      }

      return properties;
    }
  }
}

