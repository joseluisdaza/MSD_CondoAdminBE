namespace Condominio.Reports
{
  /// <summary>
  /// Interface para el servicio de ejecución de reportes.
  /// Define el contrato para generar reportes con diferentes formatos.
  /// </summary>
  public interface IReportExecutionService
  {
    /// <summary>
    /// Ejecuta un reporte con los datos proporcionados usando el generador especificado.
    /// </summary>
    /// <param name="reportData">Datos del reporte a ejecutar</param>
    /// <param name="format">Formato de salida (json, pdf, excel, html, etc.)</param>
    /// <returns>Reporte generado en el formato especificado</returns>
    object ExecuteReport(ReportExecutionData reportData, string format = "json");

    /// <summary>
    /// Registra un generador de reportes para un formato específico.
    /// </summary>
    /// <param name="format">Formato que manejará este generador</param>
    /// <param name="generator">Generador a registrar</param>
    void RegisterGenerator(string format, IReportGenerator generator);

    /// <summary>
    /// Obtiene los formatos de reportes soportados.
    /// </summary>
    /// <returns>Colección de formatos disponibles</returns>
    IEnumerable<string> GetSupportedFormats();
  }
}
