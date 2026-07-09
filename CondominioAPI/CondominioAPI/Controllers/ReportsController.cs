using Condominio.DTOs;
using Condominio.Models;
using Condominio.Repository.Repositories;
using Condominio.Reports;
using Condominio.Utils.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using Condominio.Reports.Models;

namespace CondominioAPI.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class ReportsController : ControllerBase
  {
    private readonly IReportRepository _reportRepository;
    private readonly IReportRoleRepository _reportRoleRepository;
    private readonly IReportHeaderRepository _reportHeaderRepository;
    private readonly IReportSectionRepository _reportSectionRepository;
    private readonly IReportFooterRepository _reportFooterRepository;
    private readonly IReportAuditRepository _reportAuditRepository;
    private readonly IReportParamRepository _reportParamRepository;
    private readonly IStyleRepository _styleRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IReportExecutionService _reportExecutionService;

    public ReportsController(
      IReportRepository reportRepository,
      IReportRoleRepository reportRoleRepository,
      IReportHeaderRepository reportHeaderRepository,
      IReportSectionRepository reportSectionRepository,
      IReportFooterRepository reportFooterRepository,
      IReportAuditRepository reportAuditRepository,
      IReportParamRepository reportParamRepository,
      IStyleRepository styleRepository,
      IRoleRepository roleRepository,
      IReportExecutionService reportExecutionService)
    {
      _reportRepository = reportRepository;
      _reportRoleRepository = reportRoleRepository;
      _reportHeaderRepository = reportHeaderRepository;
      _reportSectionRepository = reportSectionRepository;
      _reportFooterRepository = reportFooterRepository;
      _reportAuditRepository = reportAuditRepository;
      _reportParamRepository = reportParamRepository;
      _styleRepository = styleRepository;
      _roleRepository = roleRepository;
      _reportExecutionService = reportExecutionService;
    }

    #region REPORTS ENDPOINTS

    /// <summary>
    /// Obtiene la lista de todos los reportes (solo ID, nombre y display name)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Auxiliar},{AppRoles.Seguridad}")]
    public async Task<ActionResult<IEnumerable<ReportListResponse>>> GetAll()
    {
      Log.Information("GET > Reports > GetAll. User: {0}", this.User.Identity?.Name);

      var reports = await _reportRepository.GetAllAsync();
      var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

      if (userRoles.Any(x => x.ToLower() == "super"))
      {
        return Ok(reports.Select(report => new ReportListResponse
        {
          Id = report.Id,
          ReportName = report.ReportName,
          DisplayName = report.DisplayName,
          Params = _reportParamRepository.GetByReportIdAsync(report.Id).Result
                      .Select(p => new ReportParamLightResponse
                      {
                        ParamName = p.ParamName,
                        ParamType = p.ParamType,
                        ParamDescription = p.ParamDescription
                      }).ToList()
        }));
      }

      var response = new List<ReportListResponse>();
      foreach (Report report in reports)
      {
        bool hasPermissions = _reportRoleRepository.GetByReportIdAsync(report.Id).Result
                            .Any(x => userRoles.Any(y => x.Role.RolName == y));
        if (hasPermissions)
        {
          response.Add(new ReportListResponse
          {
            Id = report.Id,
            ReportName = report.ReportName,
            DisplayName = report.DisplayName,
            Params = _reportParamRepository.GetByReportIdAsync(report.Id).Result
                      .Select(p => new ReportParamLightResponse
                      {
                        ParamName = p.ParamName,
                        ParamType = p.ParamType,
                        ParamDescription = p.ParamDescription
                      }).ToList()
          });
        }
      }
      
      return Ok(response);
    }

    /// <summary>
    /// Obtiene un reporte completo por ID incluyendo headers, sections, footers, roles y parámetros
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportDetailResponse>> GetById(int id)
    {
      Log.Information("GET > Reports > GetById. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var report = await _reportRepository.GetByIdAsync(id);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var headers = await _reportHeaderRepository.GetByReportIdOrderedAsync(id);
      var sections = await _reportSectionRepository.GetByReportIdOrderedAsync(id);
      var footers = await _reportFooterRepository.GetByReportIdOrderedAsync(id);
      var parameters = await _reportParamRepository.GetByReportIdAsync(id);

      var response = new ReportDetailResponse
      {
        Id = report.Id,
        ReportName = report.ReportName,
        DisplayName = report.DisplayName,
        TitleStyleId = report.TitleStyleId,
        DisplayHeader = report.DisplayHeader,
        DisplayFooter = report.DisplayFooter,
        TitleStyle = report.TitleStyle != null ? MapStyleToResponse(report.TitleStyle) : null,
        Headers = headers.Select(h => MapHeaderToResponse(h)),
        Sections = sections.Select(s => MapSectionToResponse(s)),
        Footers = footers.Select(f => MapFooterToResponse(f)),
        Params = parameters.Select(p => MapParamToResponse(p))
      };

      return Ok(response);
    }

    /// <summary>
    /// Obtiene un reporte por nombre incluyendo headers, sections, footers, roles y parámetros
    /// </summary>
    [HttpGet("ByName/{reportName}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super},{AppRoles.Auxiliar},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Seguridad}")]
    public async Task<ActionResult<ReportLightResponse>> GetByName(string reportName)
    {
      Log.Information("GET > Reports > GetByName. User: {0}, Name: {1}", this.User.Identity?.Name, reportName);

      var report = await _reportRepository.GetByNameAsync(reportName);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
      bool hasPermissions = _reportRoleRepository.GetByReportIdAsync(report.Id).Result
                              .Any(x => userRoles.Any(y => x.Role.RolName == y));
      if (!hasPermissions)
        return Unauthorized();

      var @params = await _reportParamRepository.GetByReportIdAsync(report.Id);

      var response = new ReportLightResponse
      {
        Id = report.Id,
        DisplayFooter = report.DisplayFooter,
        DisplayHeader = report.DisplayHeader,
        DisplayName = report.DisplayName,
        ReportName = report.ReportName,
        TitleStyleId = report.TitleStyleId,
        Params = @params.Select(p => MapParamToResponse(p))
      };

      return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo reporte
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportLightResponse>> Create([FromBody] ReportLightRequest request)
    {
      Log.Information("POST > Reports > Create. User: {0}", this.User.Identity?.Name);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      // Validar que no exista un reporte con el mismo nombre (ignorando case)
      var existingReport = await _reportRepository.GetByNameAsync(request.ReportName);
      if (existingReport != null)
      {
        Log.Warning("POST > Reports > Create. Report with name '{0}' already exists. User: {1}",
          request.ReportName, this.User.Identity?.Name);
        return BadRequest(new { message = $"A report with the name '{request.ReportName}' already exists." });
      }

      var report = new Report
      {
        ReportName = request.ReportName,
        DisplayName = request.DisplayName,
        TitleStyleId = request.TitleStyleId,
        DisplayHeader = request.DisplayHeader,
        DisplayFooter = request.DisplayFooter,
        StartDate = DateTime.Now,
        EndDate = null
      };

      await _reportRepository.AddAsync(report);
      await CreateHeadersAsync(report.Id, request.Headers);
      await CreateSectionsAsync(report.Id, request.Sections);
      await CreateFootersAsync(report.Id, request.Footers);
      await CreateParamsAsync(report.Id, request.Params);

      Log.Information("POST > Reports > Created successfully. ReportId: {0}", report.Id);
      return CreatedAtAction(nameof(GetById), new { id = report.Id }, new ReportLightResponse
      {
        Id = report.Id,
        ReportName = report.ReportName,
        DisplayName = report.DisplayName,
        TitleStyleId = report.TitleStyleId,
        DisplayHeader = report.DisplayHeader,
        DisplayFooter = report.DisplayFooter,
        Params = request.Params.Select(p => new ReportParamResponse 
        { 
          ParamName = p.ParamName, 
          ParamType = p.ParamType, 
          ParamDescription = p.ParamDescription,
        })
      });
    }

    private async Task CreateHeadersAsync(int reportId, IEnumerable<ReportHeaderLightRequest> headers)
    {
      if (headers == null || !headers.Any())
        return;

      foreach (var headerRequest in headers.Where(x => x != null))
      {
        await _reportHeaderRepository.AddAsync(new ReportHeader
        {
          ReportId = reportId,
          DisplayOrder = headerRequest.DisplayOrder,
          StyleId = headerRequest.StyleId,
          DisplayContent = headerRequest.DisplayContent,
          IsQuery = headerRequest.IsQuery,
          StartDate = DateTime.Now,
          EndDate = null
        });
      }
    }


    private async Task CreateSectionsAsync(int reportId, IEnumerable<ReportSectionLightRequest> sections)
    {
      if (sections == null || !sections.Any()) return;

      foreach (var sectionRequest in sections.Where(x => x != null))
      {
        await _reportSectionRepository.AddAsync(new ReportSection
        {
          ReportId = reportId,
          DisplayOrder = sectionRequest.DisplayOrder,
          StyleId = sectionRequest.StyleId,
          HeaderStyleId = sectionRequest.HeaderStyleId,
          DisplayContent = sectionRequest.DisplayContent,
          IsQuery = sectionRequest.IsQuery,
          StartDate = DateTime.Now,
          EndDate = null
        });
      }
    }

    private async Task CreateFootersAsync(int reportId, IEnumerable<ReportFooterLightRequest> footers)
    {
      if (footers == null || !footers.Any()) return;

      foreach (var footerRequest in footers.Where(x => x != null))
      {
        await _reportFooterRepository.AddAsync(new ReportFooter
        {
          ReportId = reportId,
          DisplayOrder = footerRequest.DisplayOrder,
          StyleId = footerRequest.StyleId,
          DisplayContent = footerRequest.DisplayContent,
          IsQuery = footerRequest.IsQuery,
          StartDate = DateTime.Now,
          EndDate = null
        });
      }
    }

    private async Task CreateParamsAsync(int reportId, IEnumerable<ReportParamLightRequest> @params)
    {
      if (@params == null || !@params.Any())
        return;

      var startDate = DateTime.Now;
      foreach (var paramRequest in @params.Where(x => x != null))
      {
        await _reportParamRepository.AddAsync(new ReportParam
        {
          ReportId = reportId,
          ParamName = paramRequest.ParamName,
          ParamType = paramRequest.ParamType,
          ParamDescription = paramRequest.ParamDescription,
          StartDate = startDate,
          EndDate = null
        });
      }
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
      if (report.ReportName.ToLower() != request.ReportName.ToLower())
      {
        var existingReport = await _reportRepository.GetByNameAsync(request.ReportName);
        if (existingReport != null)
        {
          Log.Warning("PUT > Reports > Update. Report with name '{0}' already exists. User: {1}",
            request.ReportName, this.User.Identity?.Name);
          return BadRequest(new { message = $"A report with the name '{request.ReportName}' already exists." });
        }
      }

      report.ReportName = request.ReportName;
      report.DisplayName = request.DisplayName;
      report.TitleStyleId = request.TitleStyleId;
      report.DisplayHeader = request.DisplayHeader;
      report.DisplayFooter = request.DisplayFooter;
      report.EndDate = request.EndDate;

      await _reportRepository.UpdateAsync(report);

      // Update parameters if provided
      if (request.Params != null && request.Params.Any())
      {
        // Get existing params for this report
        var existingParams = await _reportParamRepository.GetByReportIdAsync(id);

        // Delete params that are not in the request
        foreach (var existingParam in existingParams)
        {
          var paramInRequest = request.Params.FirstOrDefault(p => 
            p.ParamName.ToLower() == existingParam.ParamName.ToLower());

          if (paramInRequest == null)
          {
            await _reportParamRepository.DeleteAsync(existingParam.Id);
          }
        }

        // Add or update params from request
        foreach (var paramRequest in request.Params.Where(x => x != null))
        {
          var existingParam = existingParams.FirstOrDefault(p => 
            p.ParamName.ToLower() == paramRequest.ParamName.ToLower());

          if (existingParam != null)
          {
            // Update existing param
            existingParam.ParamType = paramRequest.ParamType;
            existingParam.ParamDescription = paramRequest.ParamDescription;
            existingParam.EndDate = paramRequest.EndDate;
            await _reportParamRepository.UpdateAsync(existingParam);
          }
          else
          {
            // Create new param
            await _reportParamRepository.AddAsync(new ReportParam
            {
              ReportId = id,
              ParamName = paramRequest.ParamName,
              ParamType = paramRequest.ParamType,
              ParamDescription = paramRequest.ParamDescription,
              StartDate = paramRequest.StartDate,
              EndDate = paramRequest.EndDate
            });
          }
        }
      }

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
    /// Ejecuta un reporte completo con headers, sections y footers aplicando los filtros proporcionados
    /// </summary>
    [HttpPost("{reportId}/Execute")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Auxiliar},{AppRoles.Seguridad}")]
    public async Task<ActionResult<ReportExecutionResponse>> ExecuteReport(int reportId, [FromBody] ReportExecutionRequest request)
    {
      Log.Information("POST > Reports > Execute. User: {0}, ReportId: {1}", 
        this.User.Identity?.Name, reportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(reportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      try
      {
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Log.Information("POST > Reports > Execute > User roles: {0}", string.Join(", ", userRoles));

        bool hasPermissions = _reportRoleRepository.GetByReportIdAsync(reportId).Result
                              .Any(x => userRoles.Any(y => x.Role.RolName == y));
        if (!hasPermissions)
          return Unauthorized();

        // Retrieve data from database
        var reportData = await BuildReportExecutionData(report, reportId, request);

        // Generate report using the execution service (format-agnostic)
        var result = _reportExecutionService.ExecuteReport(reportData, "json");
        var response = (result as JsonReportOutput).Content;

        Log.Information("POST > Reports > Execute > Successfully executed for ReportId: {0}", reportId);
        return Ok(response);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error executing report {0}", reportId);
        return StatusCode(500, new { message = "Error executing report.", error = ex.Message });
      }
    }

    /// <summary>
    /// Construye los datos necesarios para ejecutar un reporte.
    /// Esta separación permite que la lógica de recuperación de datos esté aislada de la generación.
    /// </summary>
    private async Task<ReportExecutionData> BuildReportExecutionData(
      Report report,
      int reportId,
      ReportExecutionRequest request)
    {
      // Get available styles
      var styles = await _styleRepository.GetAllAsync();
      var styleDataList = styles.Select(s => MapStyleToReportStyleData(s)).ToList();

      // Get headers data
      var headerParts = new List<ReportPartData>();
      if (report.DisplayHeader)
      {
        var headers = await _reportHeaderRepository.GetByReportIdOrderedAsync(reportId);
        headerParts = await BuildReportHeaderParts(headers, request.Filters);
      }

      // Get sections data
      var sections = await _reportSectionRepository.GetByReportIdOrderedAsync(reportId);
      var sectionParts = await BuildReportParts(sections, request.Filters);

      // Get footers data
      var footerParts = new List<ReportPartData>();
      if (report.DisplayFooter)
      {
        var footers = await _reportFooterRepository.GetByReportIdOrderedAsync(reportId);
        footerParts = await BuildReportParts(footers, request.Filters);
      }

      // Build the report execution data
      var reportData = new ReportExecutionData
      {
        Title = report.DisplayName ?? report.ReportName,
        TitleStyleId = report.TitleStyleId,
        AvailableStyles = styleDataList,
        HeaderParts = headerParts,
        SectionParts = sectionParts,
        FooterParts = footerParts
      };

      return reportData;
    }

    /// <summary>
    /// Construye las partes del reporte a partir de headers, sections o footers.
    /// Ejecuta queries si es necesario.
    /// </summary>
    private async Task<List<ReportPartData>> BuildReportParts<T>(
      IEnumerable<T> parts,
      Dictionary<string, object>? filters)
      where T : class, IReportPartEntity
    {
      var reportParts = new List<ReportPartData>();

      foreach (var part in parts.OrderBy(p => p.DisplayOrder))
      {
        object? content = null;

        if (part.IsQuery)
        {
          // Execute query with provided filters
          content = await _reportRepository.ExecuteQueryAsync(part.DisplayContent, filters);
        }
        else
        {
          // Use static content
          content = part.DisplayContent;
        }

        reportParts.Add(new ReportPartData
        {
          Content = content,
          StyleId = part.StyleId,
          IsTable = part.IsQuery,
          DisplayOrder = part.DisplayOrder
        });
      }

      return reportParts;
    }

    private async Task<List<ReportPartData>> BuildReportHeaderParts<T>(
      IEnumerable<T> parts,
      Dictionary<string, object>? filters)
      where T : class, IReportPartEntity
    {
      var reportParts = new List<ReportPartData>();

      foreach (var part in parts.OrderBy(p => p.DisplayOrder))
      {
        object? content = null;

        if (part.IsQuery)
        {
          // Execute query with provided filters
          content = await _reportRepository.ExecuteQueryAsync(part.DisplayContent, filters);
        }
        else
        {
          // Use static content
          content = part.DisplayContent;
        }

        reportParts.Add(new ReportPartData
        {
          Content = content,
          StyleId = part.StyleId,
          IsTable = part.IsQuery,
          DisplayOrder = part.DisplayOrder
        });
      }

      return reportParts;
    }

    /// <summary>
    /// Mapea un Style a ReportStyleData para usar en la capa de reportes.
    /// </summary>
    private ReportStyleData MapStyleToReportStyleData(Style style)
    {
      return new ReportStyleData
      {
        Id = style.Id,
        StyleName = style.StyleName,
        Bold = style.Bold,
        Italic = style.Italic,
        Underline = style.Underline,
        FontSize = style.FontSize,
        FontColor = style.FontColor,
        BackgroundColor = style.BackgroundColor,
        HorizontalAlignment = style.HorizontalAlignment,
        VerticalAlignment = style.VerticalAlignment,
        WidthPercentage = style.WidthPercentage
      };
    }

    /// <summary>
    /// Ejecuta un reporte y lo genera en formato PDF, retornando la ubicación del archivo
    /// </summary>
    [HttpPost("{reportId}/Execute/Pdf")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super},{AppRoles.Director},{AppRoles.Habitante},{AppRoles.Auxiliar},{AppRoles.Seguridad}")]
    public async Task<ActionResult<ReportPdfResponse>> ExecuteReportPdf(int reportId, [FromBody] ReportExecutionRequest request)
    {
      Log.Information("POST > Reports > Execute > PDF. User: {0}, ReportId: {1}",
        this.User.Identity?.Name, reportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(reportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      try
      {
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        Log.Information("POST > Reports > Execute > PDF > User roles: {0}", string.Join(", ", userRoles));

        bool hasPermissions = _reportRoleRepository.GetByReportIdAsync(reportId).Result
                              .Any(x => userRoles.Any(y => x.Role.RolName == y));
        if (!hasPermissions)
          return Unauthorized();

        // Retrieve data from database
        var reportData = await BuildReportExecutionData(report, reportId, request);

        // Generate PDF report using the PDF generator
        var pdfGenerator = new PdfReportGenerator();
        var  result = pdfGenerator.Generate(reportData) as FileReportOutput;

        
        var response = new ReportPdfResponse
        {
          FilePath = result.FilePath ?? "",
          FileName = result.FileName ?? "",
          Success = result.Success
        };

        Log.Information("POST > Reports > Execute > PDF > Successfully generated for ReportId: {0}, FileName: {1}", 
          reportId, response.FileName);

        return Ok(response);
        
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error generating PDF report {0}", reportId);
        return StatusCode(500, new ReportPdfResponse
        {
          Success = false,
          Error = ex.Message
        });
      }
    }

    #endregion

    #region REPORT ROLES ENDPOINTS

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
    /// Elimina la asignación de un rol de un reporte
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

    #endregion

    #region STYLES ENDPOINTS

    /// <summary>
    /// Obtiene todos los estilos
    /// </summary>
    [HttpGet("Styles")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<StyleResponse>>> GetAllStyles()
    {
      Log.Information("GET > Reports > Styles > GetAll. User: {0}", this.User.Identity?.Name);

      var styles = await _styleRepository.GetAllAsync();
      var response = styles.Select(s => MapStyleToResponse(s));

      return Ok(response);
    }

    /// <summary>
    /// Obtiene un estilo por ID
    /// </summary>
    [HttpGet("Styles/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<StyleResponse>> GetStyleById(int id)
    {
      Log.Information("GET > Reports > Styles > GetById. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var style = await _styleRepository.GetByIdAsync(id);
      if (style == null)
        return NotFound(new { message = "Style not found." });

      return Ok(MapStyleToResponse(style));
    }

    /// <summary>
    /// Crea un nuevo estilo
    /// </summary>
    [HttpPost("Styles")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<StyleResponse>> CreateStyle([FromBody] StyleRequest request)
    {
      Log.Information("POST > Reports > Styles > Create. User: {0}", this.User.Identity?.Name);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var existingStyle = await _styleRepository.GetByNameAsync(request.StyleName);
      if (existingStyle != null)
        return BadRequest(new { message = $"A style with the name '{request.StyleName}' already exists." });

      var style = new Style
      {
        StyleName = request.StyleName,
        Bold = request.Bold,
        Italic = request.Italic,
        Underline = request.Underline,
        FontSize = request.FontSize,
        FontColor = request.FontColor,
        BackgroundColor = request.BackgroundColor,
        HorizontalAlignment = request.HorizontalAlignment,
        VerticalAlignment = request.VerticalAlignment,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        WidthPercentage = request.WidthPercentage
      };

      await _styleRepository.AddAsync(style);

      Log.Information("POST > Reports > Styles > Created successfully. StyleId: {0}", style.Id);
      return CreatedAtAction(nameof(GetStyleById), new { id = style.Id }, MapStyleToResponse(style));
    }

    /// <summary>
    /// Actualiza un estilo existente
    /// </summary>
    [HttpPut("Styles/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> UpdateStyle(int id, [FromBody] StyleRequest request)
    {
      Log.Information("PUT > Reports > Styles > Update. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var style = await _styleRepository.GetByIdAsync(id);
      if (style == null)
        return NotFound(new { message = "Style not found." });

      style.StyleName = request.StyleName;
      style.Bold = request.Bold;
      style.Italic = request.Italic;
      style.Underline = request.Underline;
      style.FontSize = request.FontSize;
      style.FontColor = request.FontColor;
      style.BackgroundColor = request.BackgroundColor;
      style.HorizontalAlignment = request.HorizontalAlignment;
      style.VerticalAlignment = request.VerticalAlignment;
      style.EndDate = request.EndDate;
      style.WidthPercentage = request.WidthPercentage;

      await _styleRepository.UpdateAsync(style);

      Log.Information("PUT > Reports > Styles > Updated successfully. StyleId: {0}", id);
      return Ok(new { message = "Style updated successfully." });
    }

    /// <summary>
    /// Elimina un estilo
    /// </summary>
    [HttpDelete("Styles/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> DeleteStyle(int id)
    {
      Log.Information("DELETE > Reports > Styles. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var style = await _styleRepository.GetByIdAsync(id);
      if (style == null)
        return NotFound(new { message = "Style not found." });

      await _styleRepository.DeleteAsync(id);

      Log.Information("DELETE > Reports > Styles > Deleted successfully. StyleId: {0}", id);
      return Ok(new { message = "Style deleted successfully." });
    }

    #endregion

    #region REPORT HEADERS ENDPOINTS

    /// <summary>
    /// Obtiene los headers de un reporte
    /// </summary>
    [HttpGet("{reportId}/Headers")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportHeaderResponse>>> GetReportHeaders(int reportId)
    {
      Log.Information("GET > Reports > Headers. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      var headers = await _reportHeaderRepository.GetByReportIdOrderedAsync(reportId);
      var response = headers.Select(h => MapHeaderToResponse(h));

      return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo header para un reporte
    /// </summary>
    [HttpPost("{reportId}/Headers")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportHeaderResponse>> CreateHeader(int reportId, [FromBody] ReportHeaderRequest request)
    {
      Log.Information("POST > Reports > Headers. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(reportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var header = new ReportHeader
      {
        ReportId = reportId,
        DisplayOrder = request.DisplayOrder,
        StyleId = request.StyleId,
        DisplayContent = request.DisplayContent,
        IsQuery = request.IsQuery,
        StartDate = request.StartDate,
        EndDate = request.EndDate
      };

      await _reportHeaderRepository.AddAsync(header);

      Log.Information("POST > Reports > Headers > Created successfully. HeaderId: {0}", header.Id);
      return CreatedAtAction(nameof(GetReportHeaders), new { reportId }, MapHeaderToResponse(header));
    }

    /// <summary>
    /// Actualiza un header de reporte
    /// </summary>
    [HttpPut("Headers/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> UpdateHeader(int id, [FromBody] ReportHeaderRequest request)
    {
      Log.Information("PUT > Reports > Headers > Update. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var header = await _reportHeaderRepository.GetByIdAsync(id);
      if (header == null)
        return NotFound(new { message = "Header not found." });

      header.DisplayOrder = request.DisplayOrder;
      header.StyleId = request.StyleId;
      header.DisplayContent = request.DisplayContent;
      header.IsQuery = request.IsQuery;
      header.EndDate = request.EndDate;

      await _reportHeaderRepository.UpdateAsync(header);

      Log.Information("PUT > Reports > Headers > Updated successfully. HeaderId: {0}", id);
      return Ok(new { message = "Header updated successfully." });
    }

    /// <summary>
    /// Elimina un header de reporte
    /// </summary>
    [HttpDelete("Headers/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> DeleteHeader(int id)
    {
      Log.Information("DELETE > Reports > Headers. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var header = await _reportHeaderRepository.GetByIdAsync(id);
      if (header == null)
        return NotFound(new { message = "Header not found." });

      await _reportHeaderRepository.DeleteAsync(id);

      Log.Information("DELETE > Reports > Headers > Deleted successfully. HeaderId: {0}", id);
      return Ok(new { message = "Header deleted successfully." });
    }

    #endregion

    #region REPORT SECTIONS ENDPOINTS

    /// <summary>
    /// Obtiene las secciones de un reporte
    /// </summary>
    [HttpGet("{reportId}/Sections")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportSectionResponse>>> GetReportSections(int reportId)
    {
      Log.Information("GET > Reports > Sections. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      var sections = await _reportSectionRepository.GetByReportIdOrderedAsync(reportId);
      var response = sections.Select(s => MapSectionToResponse(s));

      return Ok(response);
    }

    /// <summary>
    /// Crea una nueva sección para un reporte
    /// </summary>
    [HttpPost("{reportId}/Sections")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportSectionResponse>> CreateSection(int reportId, [FromBody] ReportSectionRequest request)
    {
      Log.Information("POST > Reports > Sections. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(reportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var section = new ReportSection
      {
        ReportId = reportId,
        DisplayOrder = request.DisplayOrder,
        StyleId = request.StyleId,
        HeaderStyleId = request.HeaderStyleId,
        DisplayContent = request.DisplayContent,
        IsQuery = request.IsQuery,
        StartDate = request.StartDate,
        EndDate = request.EndDate
      };

      await _reportSectionRepository.AddAsync(section);

      Log.Information("POST > Reports > Sections > Created successfully. SectionId: {0}", section.Id);
      return CreatedAtAction(nameof(GetReportSections), new { reportId }, MapSectionToResponse(section));
    }

    /// <summary>
    /// Actualiza una sección de reporte
    /// </summary>
    [HttpPut("Sections/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> UpdateSection(int id, [FromBody] ReportSectionRequest request)
    {
      Log.Information("PUT > Reports > Sections > Update. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var section = await _reportSectionRepository.GetByIdAsync(id);
      if (section == null)
        return NotFound(new { message = "Section not found." });

      section.DisplayOrder = request.DisplayOrder;
      section.StyleId = request.StyleId;
      section.HeaderStyleId = request.HeaderStyleId;
      section.DisplayContent = request.DisplayContent;
      section.IsQuery = request.IsQuery;
      section.EndDate = request.EndDate;

      await _reportSectionRepository.UpdateAsync(section);

      Log.Information("PUT > Reports > Sections > Updated successfully. SectionId: {0}", id);
      return Ok(new { message = "Section updated successfully." });
    }

    /// <summary>
    /// Elimina una sección de reporte
    /// </summary>
    [HttpDelete("Sections/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> DeleteSection(int id)
    {
      Log.Information("DELETE > Reports > Sections. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var section = await _reportSectionRepository.GetByIdAsync(id);
      if (section == null)
        return NotFound(new { message = "Section not found." });

      await _reportSectionRepository.DeleteAsync(id);

      Log.Information("DELETE > Reports > Sections > Deleted successfully. SectionId: {0}", id);
      return Ok(new { message = "Section deleted successfully." });
    }

    #endregion

    #region REPORT FOOTERS ENDPOINTS

    /// <summary>
    /// Obtiene los footers de un reporte
    /// </summary>
    [HttpGet("{reportId}/Footers")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportFooterResponse>>> GetReportFooters(int reportId)
    {
      Log.Information("GET > Reports > Footers. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      var footers = await _reportFooterRepository.GetByReportIdOrderedAsync(reportId);
      var response = footers.Select(f => MapFooterToResponse(f));

      return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo footer para un reporte
    /// </summary>
    [HttpPost("{reportId}/Footers")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportFooterResponse>> CreateFooter(int reportId, [FromBody] ReportFooterRequest request)
    {
      Log.Information("POST > Reports > Footers. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(reportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var footer = new ReportFooter
      {
        ReportId = reportId,
        DisplayOrder = request.DisplayOrder,
        StyleId = request.StyleId,
        DisplayContent = request.DisplayContent,
        IsQuery = request.IsQuery,
        StartDate = request.StartDate,
        EndDate = request.EndDate
      };

      await _reportFooterRepository.AddAsync(footer);

      Log.Information("POST > Reports > Footers > Created successfully. FooterId: {0}", footer.Id);
      return CreatedAtAction(nameof(GetReportFooters), new { reportId }, MapFooterToResponse(footer));
    }

    /// <summary>
    /// Actualiza un footer de reporte
    /// </summary>
    [HttpPut("Footers/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> UpdateFooter(int id, [FromBody] ReportFooterRequest request)
    {
      Log.Information("PUT > Reports > Footers > Update. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var footer = await _reportFooterRepository.GetByIdAsync(id);
      if (footer == null)
        return NotFound(new { message = "Footer not found." });

      footer.DisplayOrder = request.DisplayOrder;
      footer.StyleId = request.StyleId;
      footer.DisplayContent = request.DisplayContent;
      footer.IsQuery = request.IsQuery;
      footer.EndDate = request.EndDate;

      await _reportFooterRepository.UpdateAsync(footer);

      Log.Information("PUT > Reports > Footers > Updated successfully. FooterId: {0}", id);
      return Ok(new { message = "Footer updated successfully." });
    }

    /// <summary>
    /// Elimina un footer de reporte
    /// </summary>
    [HttpDelete("Footers/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> DeleteFooter(int id)
    {
      Log.Information("DELETE > Reports > Footers. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var footer = await _reportFooterRepository.GetByIdAsync(id);
      if (footer == null)
        return NotFound(new { message = "Footer not found." });

      await _reportFooterRepository.DeleteAsync(id);

      Log.Information("DELETE > Reports > Footers > Deleted successfully. FooterId: {0}", id);
      return Ok(new { message = "Footer deleted successfully." });
    }

    #endregion

    #region REPORT AUDITS ENDPOINTS

    /// <summary>
    /// Obtiene los audits de un reporte
    /// </summary>
    [HttpGet("{reportId}/Audits")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportAuditResponse>>> GetReportAudits(int reportId)
    {
      Log.Information("GET > Reports > Audits. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      var audits = await _reportAuditRepository.GetByReportIdAsync(reportId);
      var response = audits.Select(a => new ReportAuditResponse
      {
        Id = a.Id,
        ReportId = a.ReportId,
        UserId = a.UserId,
        UserName = a.User?.UserName ?? string.Empty,
        Parameters = a.Parameters,
        ExecutionDate = a.ExecutionDate
      });

      return Ok(response);
    }

    /// <summary>
    /// Obtiene los audits de un usuario
    /// </summary>
    [HttpGet("Audits/User/{userId}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportAuditResponse>>> GetUserAudits(int userId)
    {
      Log.Information("GET > Reports > Audits > User. User: {0}, UserId: {1}", this.User.Identity?.Name, userId);

      var audits = await _reportAuditRepository.GetByUserIdAsync(userId);
      var response = audits.Select(a => new ReportAuditResponse
      {
        Id = a.Id,
        ReportId = a.ReportId,
        UserId = a.UserId,
        UserName = a.User?.UserName ?? string.Empty,
        Parameters = a.Parameters,
        ExecutionDate = a.ExecutionDate
      });

      return Ok(response);
    }

    /// <summary>
    /// Obtiene los audits de un reporte dentro de un rango de fechas
    /// </summary>
    [HttpGet("{reportId}/Audits/DateRange")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportAuditResponse>>> GetAuditsByDateRange(
      int reportId,
      [FromQuery] DateTime startDate,
      [FromQuery] DateTime endDate)
    {
      Log.Information("GET > Reports > Audits > DateRange. User: {0}, ReportId: {1}, StartDate: {2}, EndDate: {3}",
        this.User.Identity?.Name, reportId, startDate, endDate);

      var audits = await _reportAuditRepository.GetByReportIdAndDateRangeAsync(reportId, startDate, endDate);
      var response = audits.Select(a => new ReportAuditResponse
      {
        Id = a.Id,
        ReportId = a.ReportId,
        UserId = a.UserId,
        UserName = a.User?.UserName ?? string.Empty,
        Parameters = a.Parameters,
        ExecutionDate = a.ExecutionDate
      });

      return Ok(response);
    }

    ///// <summary>
    ///// Registra la ejecución de un reporte (auditoría)
    ///// </summary>
    //[HttpPost("{reportId}/Audits")]
    //[Authorize]
    //public async Task<IActionResult> LogReportExecution(int reportId, [FromBody] ReportAuditRequest request)
    //{
    //  Log.Information("POST > Reports > Audits. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

    //  var report = await _reportRepository.GetByIdAsync(reportId);
    //  if (report == null)
    //    return NotFound(new { message = "Report not found." });

    //  var audit = new ReportAudit
    //  {
    //    ReportId = reportId,
    //    UserId = request.UserId,
    //    Parameters = request.Parameters,
    //    ExecutionDate = DateTime.Now
    //  };

    //  await _reportAuditRepository.AddAsync(audit);

    //  Log.Information("POST > Reports > Audits > Logged successfully. AuditId: {0}", audit.Id);
    //  return Ok(new { message = "Report execution logged successfully." });
    //}

    #endregion

    #region REPORT PARAMS ENDPOINTS

    /// <summary>
    /// Obtiene los parámetros de un reporte
    /// </summary>
    [HttpGet("{reportId}/Params")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<IEnumerable<ReportParamResponse>>> GetReportParams(int reportId)
    {
      Log.Information("GET > Reports > Params. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      var @params = await _reportParamRepository.GetByReportIdAsync(reportId);
      var response = @params.Select(p => MapParamToResponse(p));

      return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo parámetro para un reporte
    /// </summary>
    [HttpPost("{reportId}/Params")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<ActionResult<ReportParamResponse>> CreateParam(int reportId, [FromBody] ReportParamRequest request)
    {
      Log.Information("POST > Reports > Params. User: {0}, ReportId: {1}", this.User.Identity?.Name, reportId);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var report = await _reportRepository.GetByIdAsync(reportId);
      if (report == null)
        return NotFound(new { message = "Report not found." });

      var param = new ReportParam
      {
        ReportId = reportId,
        ParamName = request.ParamName,
        ParamType = request.ParamType,
        ParamDescription = request.ParamDescription,
        StartDate = request.StartDate,
        EndDate = request.EndDate
      };

      await _reportParamRepository.AddAsync(param);

      Log.Information("POST > Reports > Params > Created successfully. ParamId: {0}", param.Id);
      return CreatedAtAction(nameof(GetReportParams), new { reportId }, MapParamToResponse(param));
    }

    /// <summary>
    /// Actualiza un parámetro de reporte
    /// </summary>
    [HttpPut("Params/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> UpdateParam(int id, [FromBody] ReportParamRequest request)
    {
      Log.Information("PUT > Reports > Params > Update. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      var param = await _reportParamRepository.GetByIdAsync(id);
      if (param == null)
        return NotFound(new { message = "Parameter not found." });

      param.ParamName = request.ParamName;
      param.ParamType = request.ParamType;
      param.ParamDescription = request.ParamDescription;
      param.EndDate = request.EndDate;

      await _reportParamRepository.UpdateAsync(param);

      Log.Information("PUT > Reports > Params > Updated successfully. ParamId: {0}", id);
      return Ok(new { message = "Parameter updated successfully." });
    }

    /// <summary>
    /// Elimina un parámetro de reporte
    /// </summary>
    [HttpDelete("Params/{id}")]
    [Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
    public async Task<IActionResult> DeleteParam(int id)
    {
      Log.Information("DELETE > Reports > Params. User: {0}, Id: {1}", this.User.Identity?.Name, id);

      var param = await _reportParamRepository.GetByIdAsync(id);
      if (param == null)
        return NotFound(new { message = "Parameter not found." });

      await _reportParamRepository.DeleteAsync(id);

      Log.Information("DELETE > Reports > Params > Deleted successfully. ParamId: {0}", id);
      return Ok(new { message = "Parameter deleted successfully." });
    }

    #endregion

    #region HELPER METHODS

    private StyleResponse MapStyleToResponse(Style style)
    {
      return new StyleResponse
      {
        Id = style.Id,
        StyleName = style.StyleName,
        Bold = style.Bold,
        Italic = style.Italic,
        Underline = style.Underline,
        FontSize = style.FontSize,
        FontColor = style.FontColor,
        BackgroundColor = style.BackgroundColor,
        HorizontalAlignment = style.HorizontalAlignment,
        VerticalAlignment = style.VerticalAlignment,
        WidthPercentage = style.WidthPercentage
      };
    }

    private ReportHeaderResponse MapHeaderToResponse(ReportHeader header)
    {
      return new ReportHeaderResponse
      {
        Id = header.Id,
        ReportId = header.ReportId,
        DisplayOrder = header.DisplayOrder,
        StyleId = header.StyleId,
        DisplayContent = header.DisplayContent,
        IsQuery = header.IsQuery,
        Style = header.Style != null ? MapStyleToResponse(header.Style) : null
      };
    }

    private ReportSectionResponse MapSectionToResponse(ReportSection section)
    {
      return new ReportSectionResponse
      {
        Id = section.Id,
        ReportId = section.ReportId,
        DisplayOrder = section.DisplayOrder,
        StyleId = section.StyleId,
        HeaderStyleId = section.HeaderStyleId,
        DisplayContent = section.DisplayContent,
        IsQuery = section.IsQuery,
        Style = section.Style != null ? MapStyleToResponse(section.Style) : null,
        HeaderStyle = section.HeaderStyle != null ? MapStyleToResponse(section.HeaderStyle) : null
      };
    }

    private ReportFooterResponse MapFooterToResponse(ReportFooter footer)
    {
      return new ReportFooterResponse
      {
        Id = footer.Id,
        ReportId = footer.ReportId,
        DisplayOrder = footer.DisplayOrder,
        StyleId = footer.StyleId,
        DisplayContent = footer.DisplayContent,
        IsQuery = footer.IsQuery,
        Style = footer.Style != null ? MapStyleToResponse(footer.Style) : null
      };
    }

    private ReportParamResponse MapParamToResponse(ReportParam param)
    {
      return new ReportParamResponse
      {
        Id = param.Id,
        ReportId = param.ReportId,
        ParamName = param.ParamName,
        ParamType = param.ParamType,
        ParamDescription = param.ParamDescription
      };
    }

    #endregion
  }
}

