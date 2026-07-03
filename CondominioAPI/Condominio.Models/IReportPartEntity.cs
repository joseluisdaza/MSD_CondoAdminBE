namespace Condominio.Models;

/// <summary>
/// Interface que define las propiedades comunes para las partes de un reporte (Header, Section, Footer).
/// Permite usar genéricos en métodos que trabajen con cualquier tipo de parte.
/// </summary>
public interface IReportPartEntity
{
  int DisplayOrder { get; }
  int StyleId { get; }
  string DisplayContent { get; }
  bool IsQuery { get; }
}
