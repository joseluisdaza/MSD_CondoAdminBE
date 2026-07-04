using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;
using MySqlConnector;

namespace Condominio.Repository.Repositories
{
  public class ReportRepository : Repository<Report>, IReportRepository
  {
    public ReportRepository(CondominioContext context) : base(context) { }

    public async Task<Report?> GetByIdWithRolesAsync(int id)
    {
      return await _context.Reports
        .Include(r => r.ReportRoles)
        .ThenInclude(rr => rr.Role)
        .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Report?> GetByNameAsync(string name)
    {
      return await _context.Reports
        .FirstOrDefaultAsync(r => r.ReportName.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<object>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
      var results = new List<Dictionary<string, object>>();

      // Use the connection directly with proper MySql parameter handling
      var connection = _context.Database.GetDbConnection();

      try
      {
        if (connection.State != ConnectionState.Open)
        {
          await connection.OpenAsync();
        }

        using (var command = connection.CreateCommand())
        {
          command.CommandText = query;
          command.CommandType = CommandType.Text;
          command.CommandTimeout = 300; // 5 minutes timeout

          // Add parameters if provided
          if (parameters != null && parameters.Count > 0)
          {
            foreach (var param in parameters)
            {
              var dbParam = command.CreateParameter();
              dbParam.ParameterName = $"@{param.Key}";
              var convertedValue = ConvertParameterValue(param.Value);
              dbParam.Value = convertedValue ?? DBNull.Value;
              command.Parameters.Add(dbParam);
            }
          }

          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var row = new Dictionary<string, object>();
              for (int i = 0; i < reader.FieldCount; i++)
              {
                var value = reader.GetValue(i);
                row[reader.GetName(i)] = value == DBNull.Value ? null! : value;
              }
              results.Add(row);
            }
          }
        }
      }
      finally
      {
        if (connection?.State == ConnectionState.Open)
        {
          await connection.CloseAsync();
        }
      }

      return results.Cast<object>();
    }

    /// <summary>
    /// Convierte un valor de parámetro a un tipo soportado por MySqlConnector.
    /// Maneja la conversión de JsonElement a tipos primitivos.
    /// </summary>
    private object? ConvertParameterValue(object? value)
    {
      if (value == null)
        return null;

      // If it's already a primitive type, return as-is
      if (value is string or int or long or decimal or double or float or bool or byte or DateTime)
        return value;

      // Handle JsonElement
      if (value is JsonElement jsonElement)
      {
        return ConvertJsonElement(jsonElement);
      }

      // Return the value as-is for other types
      return value;
    }

    /// <summary>
    /// Convierte un JsonElement a su valor primitivo correspondiente.
    /// </summary>
    private object? ConvertJsonElement(JsonElement jsonElement)
    {
      return jsonElement.ValueKind switch
      {
        JsonValueKind.Null => null,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Number => jsonElement.TryGetInt32(out var intValue) 
          ? intValue 
          : (object?)jsonElement.GetDouble(),
        JsonValueKind.String => jsonElement.GetString(),
        JsonValueKind.Array => JsonSerializer.Serialize(jsonElement),
        JsonValueKind.Object => JsonSerializer.Serialize(jsonElement),
        JsonValueKind.Undefined => null,
        _ => null
      };
    }
  }
}
