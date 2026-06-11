using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs
{
  public class ReportRoleRequest
  {
    [Required(ErrorMessage = "ReportId is required")]
    public int ReportId { get; set; }

    [Required(ErrorMessage = "RoleId is required")]
    public int RoleId { get; set; }
  }
}
