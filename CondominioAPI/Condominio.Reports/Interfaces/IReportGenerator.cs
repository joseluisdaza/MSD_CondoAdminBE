namespace Condominio.Reports
{
  /// <summary>
  /// Interface que define un generador de reportes.
  /// Las implementaciones deben ser agnósticas del formato de salida.
  /// </summary>
  public interface IReportGenerator
  {
    /// <summary>
    /// Genera un reporte con el contenido proporcionado.
    /// </summary>
    /// <param name="reportData">Datos del reporte a generar</param>
    /// <returns>Reporte generado</returns>
    object Generate(ReportExecutionData reportData);
  }
}
