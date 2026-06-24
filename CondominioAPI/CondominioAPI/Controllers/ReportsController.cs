using Condominio.DTOs;
using Condominio.Models;
using Condominio.Repository.Repositories;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace CondominioAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ReportsController : ControllerBase
  {
    private readonly IReportRepository _reportRepository;
    private readonly IReportRoleRepository _reportRoleRepository;
    private readonly IRoleRepository _roleRepository;

    public ReportsController(
      IReportRepository reportRepository,
      IReportRoleRepository reportRoleRepository,
      IRoleRepository roleRepository)
    {
      _reportRepository = reportRepository;
      _reportRoleRepository = reportRoleRepository;
      _roleRepository = roleRepository;
    }

    /// <summary>
    /// Obtiene la lista de todos los reportes (solo ID y nombre)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super},{AppRoles.Director}")]
    public async Task<ActionResult<IEnumerable<ReportListResponse>>> GetAll()
    {
      Log.Information("GET > Reports > GetAll. User: {0}", this.User.Identity?.Name);
      
      var reports = await _reportRepository.GetAllAsync();
      var response = reports.Select(r => new ReportListResponse
      {
        Id = r.Id,
        Name = r.Name
      });

      return Ok(response);
    }

    /// <summary>
    /// Obtiene un reporte completo por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super},{AppRoles.Director}")]
    public async Task<ActionResult<ReportRequest>> GetById(int id)
    {
      Log.Information("GET > Reports > GetById. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var report = await _reportRepository.GetByIdAsync(id);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var response = new ReportRequest
      {
        Id = report.Id,
        Name = report.Name,
        HeaderQuery = report.HeaderQuery,
        BodyQuery = report.BodyQuery,
        FooterQuery = report.FooterQuery
      };

      return Ok(response);
    }

    /// <summary>
    /// Ejecuta un reporte y devuelve los datos en formato JSON
    /// </summary>
    [HttpPost("Data")]
    [Authorize(Roles = $"{AppRoles.Super},{AppRoles.Administrador},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Auxiliar},{AppRoles.Seguridad}")]
    public async Task<ActionResult<ReportDataResponse>> GetReportData([FromBody] ReportDataRequest request)
    {
      Log.Information("POST > Reports > Data. User: {0}, ReportId: {1}", 
        this.User.Identity?.Name, request.ReportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(request.ReportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      // Get user's roles from claims
      var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
      var userRoleIds = _roleRepository.GetAllAsync().Result
                                      .Where(x => userRoles.Any(ur => ur.Equals(x.RolName)))
                                      .Select(x => x.Id)
                                      .ToList();

      Log.Information("POST > Reports > Data > User roles: {0}", string.Join(", ", userRoles));


      //TODO: Add code to validate that the user has enough permissions to execute the report based on their roles and the report's assigned roles.
      IEnumerable<ReportRole> reportRoles = _reportRoleRepository.GetByReportIdAsync(report.Id).Result;

      bool haseExecutionPermissions = reportRoles.Any(x => userRoleIds.Contains(x.RoleId ));
      if (!haseExecutionPermissions)
        return Unauthorized();

      try
      {
        var response = new ReportDataResponse();

        // Execute header query if exists
        if (!string.IsNullOrWhiteSpace(report.HeaderQuery))
        {
          var headerResults = await _reportRepository.ExecuteQueryAsync(
            report.HeaderQuery, 
            request.Filters);
          response.Header = headerResults;
        }

        // Execute body query (required)
        var bodyResults = await _reportRepository.ExecuteQueryAsync(
          report.BodyQuery, 
          request.Filters);
        response.Body = bodyResults;

        // Execute footer query if exists
        if (!string.IsNullOrWhiteSpace(report.FooterQuery))
        {
          var footerResults = await _reportRepository.ExecuteQueryAsync(
            report.FooterQuery, 
            request.Filters);
          response.Footer = footerResults;
        }

        Log.Information("POST > Reports > Data > Successfully executed for ReportId: {0}", request.ReportId);
        return Ok(response);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error executing report {0}", request.ReportId);
        return StatusCode(500, new { message = "Error executing report query.", error = ex.Message });
      }
    }

    /// <summary>
    /// Crea un nuevo reporte
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportRequest>> Create([FromBody] ReportRequest request)
    {
      Log.Information("POST > Reports > Create. User: {0}", this.User.Identity?.Name);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Validar que no exista un reporte con el mismo nombre (ignorando case)
      var existingReport = await _reportRepository.GetByNameAsync(request.Name);
      if (existingReport != null)
      {
        Log.Warning("POST > Reports > Create. Report with name '{0}' already exists. User: {1}", 
          request.Name, this.User.Identity?.Name);
        return BadRequest(new { message = $"A report with the name '{request.Name}' already exists." });
      }

      var report = new Report
      {
        Name = request.Name,
        HeaderQuery = request.HeaderQuery,
        BodyQuery = request.BodyQuery,
        FooterQuery = request.FooterQuery
      };

      await _reportRepository.AddAsync(report);

      Log.Information("POST > Reports > Created successfully. ReportId: {0}", report.Id);
      return CreatedAtAction(nameof(GetById), new { id = report.Id }, new ReportRequest
      {
        Id = report.Id,
        Name = report.Name,
        HeaderQuery = report.HeaderQuery,
        BodyQuery = report.BodyQuery,
        FooterQuery = report.FooterQuery
      });
    }

    /// <summary>
    /// Actualiza un reporte existente
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportRequest request)
    {
      Log.Information("PUT > Reports > Update. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(id);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      // Validar que no exista otro reporte con el mismo nombre (ignorando case)
      if (report.Name.ToLower() != request.Name.ToLower())
      {
        var existingReport = await _reportRepository.GetByNameAsync(request.Name);
        if (existingReport != null)
        {
          Log.Warning("PUT > Reports > Update. Report with name '{0}' already exists. User: {1}", 
            request.Name, this.User.Identity?.Name);
          return BadRequest(new { message = $"A report with the name '{request.Name}' already exists." });
        }
      }

      report.Name = request.Name;
      report.HeaderQuery = request.HeaderQuery;
      report.BodyQuery = request.BodyQuery;
      report.FooterQuery = request.FooterQuery;

      await _reportRepository.UpdateAsync(report);

      Log.Information("PUT > Reports > Updated successfully. ReportId: {0}", id);
      return Ok(new { message = "Report updated successfully." });
    }

    /// <summary>
    /// Elimina un reporte
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> Delete(int id)
    {
      Log.Information("DELETE > Reports > User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var report = await _reportRepository.GetByIdAsync(id);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      await _reportRepository.DeleteAsync(id);

      Log.Information("DELETE > Reports > Deleted successfully. ReportId: {0}", id);
      return Ok(new { message = "Report deleted successfully." });
    }

    /// <summary>
    /// Obtiene la lista de roles asignados a un reporte
    /// </summary>
    [HttpGet("Roles")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportRoleResponse>>> GetReportRoles([FromQuery] int reportId)
    {
      Log.Information("GET > Reports > Roles. User: {0}, ReportId: {1}", 
        this.User.Identity?.Name, reportId);

      var reportRoles = await _reportRoleRepository.GetByReportIdAsync(reportId);
      var response = reportRoles.Select(rr => new ReportRoleResponse
      {
        RoleId = rr.RoleId,
        RoleName = rr.Role?.RolName ?? string.Empty
      });

      return Ok(response);
    }

    /// <summary>
    /// Asigna un rol a un reporte
    /// </summary>
    [HttpPost("Roles")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> AssignRole([FromBody] ReportRoleRequest request)
    {
      Log.Information("POST > Reports > Roles. User: {0}, ReportId: {1}, RoleId: {2}", 
        this.User.Identity?.Name, request.ReportId, request.RoleId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Check if report and role exist
      var report = await _reportRepository.GetByIdAsync(request.ReportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var role = await _roleRepository.GetByIdAsync(request.RoleId);
      if (role == null)
        return NotFound(new { message = "Role not found." });

      // Check if relationship already exists
      var existingRelation = await _reportRoleRepository.GetByReportIdAndRoleIdAsync(
        request.ReportId, 
        request.RoleId);
      
      if (existingRelation != null)
        return BadRequest(new { message = "This role is already assigned to the report." });

      var reportRole = new ReportRole
      {
        ReportId = request.ReportId,
        RoleId = request.RoleId
      };

      await _reportRoleRepository.AddAsync(reportRole);

      Log.Information("POST > Reports > Roles > Assigned successfully. ReportId: {0}, RoleId: {1}", 
        request.ReportId, request.RoleId);
      
      return Ok(new { message = "Role assigned to report successfully." });
    }

    /// <summary>
    /// Elimina la asignación de un rol de un reporte (hard delete)
    /// </summary>
    [HttpDelete("Roles")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> RemoveRole([FromBody] ReportRoleRequest request)
    {
      Log.Information("DELETE > Reports > Roles. User: {0}, ReportId: {1}, RoleId: {2}", 
        this.User.Identity?.Name, request.ReportId, request.RoleId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var reportRole = await _reportRoleRepository.GetByReportIdAndRoleIdAsync(
        request.ReportId, 
        request.RoleId);
      
      if (reportRole == null)
        return NotFound(new { message = "Report-Role assignment not found." });

      await _reportRoleRepository.DeleteByReportIdAndRoleIdAsync(request.ReportId, request.RoleId);

      Log.Information("DELETE > Reports > Roles > Removed successfully. ReportId: {0}, RoleId: {1}", 
        request.ReportId, request.RoleId);
      
      return Ok(new { message = "Role removed from report successfully." });
    }
  }
}
