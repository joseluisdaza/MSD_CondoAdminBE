using Condominio.Data.Mysql.Models;
using Condominio.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
        .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<object>> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
      var results = new List<Dictionary<string, object>>();

      using (var command = _context.Database.GetDbConnection().CreateCommand())
      {
        command.CommandText = query;
        command.CommandType = CommandType.Text;

        // Add parameters if provided
        if (parameters != null)
        {
          foreach (var param in parameters)
          {
            var dbParam = command.CreateParameter();
            dbParam.ParameterName = $"@{param.Key}";
            dbParam.Value = param.Value ?? DBNull.Value;
            command.Parameters.Add(dbParam);
          }
        }

        // Ensure connection is open
        if (command.Connection?.State != ConnectionState.Open)
        {
          await _context.Database.OpenConnectionAsync();
        }

        try
        {
          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var row = new Dictionary<string, object>();
              for (int i = 0; i < reader.FieldCount; i++)
              {
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null! : reader.GetValue(i);
              }
              results.Add(row);
            }
          }
        }
        finally
        {
          if (command.Connection?.State == ConnectionState.Open)
          {
            await _context.Database.CloseConnectionAsync();
          }
        }
      }

      return results.Cast<object>();
    }
  }
}
