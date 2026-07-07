using Condominio.Reports.Models;
using Serilog;

namespace Condominio.Reports
{
  /// <summary>
  /// Servicio que orquesta la ejecución de reportes.
  /// Mantiene un registro de generadores disponibles y delega la generación al generador apropriado.
  /// </summary>
  public class ReportExecutionService : IReportExecutionService
  {
    private readonly Dictionary<string, IReportGenerator> _generators;

    public ReportExecutionService()
    {
      _generators = new Dictionary<string, IReportGenerator>(StringComparer.OrdinalIgnoreCase)
      {
        { "json", new JsonReportGenerator() }
      };
    }

    /// <summary>
    /// Ejecuta un reporte usando el generador del formato especificado.
    /// </summary>
    public AbstractReportOutput ExecuteReport(ReportExecutionData reportData, string format = "json")
    {
      if (reportData == null)
        throw new ArgumentNullException(nameof(reportData), "Report data cannot be null");

      if (string.IsNullOrWhiteSpace(format))
        format = "json";

      if (!_generators.TryGetValue(format, out var generator))
      {
        var supportedFormats = string.Join(", ", _generators.Keys);
        throw new InvalidOperationException(
          $"No generator found for format '{format}'. Supported formats: {supportedFormats}");
      }

      try
      {
        Log.Information("Executing report generation with format: {Format}", format);
        return generator.Generate(reportData);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error generating report with format {Format}", format);
        throw;
      }
    }

    /// <summary>
    /// Registra un nuevo generador para un formato específico.
    /// </summary>
    public void RegisterGenerator(string format, IReportGenerator generator)
    {
      if (string.IsNullOrWhiteSpace(format))
        throw new ArgumentException("Format cannot be null or empty", nameof(format));

      if (generator == null)
        throw new ArgumentNullException(nameof(generator));

      _generators[format.ToLower()] = generator;
      Log.Information("Report generator registered for format: {Format}", format);
    }

    /// <summary>
    /// Retorna los formatos de reportes soportados.
    /// </summary>
    public IEnumerable<string> GetSupportedFormats()
    {
      return _generators.Keys.ToList();
    }
  }
}
