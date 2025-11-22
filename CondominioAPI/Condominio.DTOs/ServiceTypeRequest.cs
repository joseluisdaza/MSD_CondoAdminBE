using System.ComponentModel.DataAnnotations;

namespace Condominio.DTOs;

public class ServiceTypeRequest
{
    [Required(ErrorMessage = "El nombre del servicio es requerido")]
    [StringLength(100, ErrorMessage = "El nombre del servicio no puede exceder 100 caracteres")]
    public string ServiceName { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string Description { get; set; } = null!;
}

public class ServiceTypeResponse : ServiceTypeRequest
{
    public int Id { get; set; }
    public int TotalServiceExpenses { get; set; }
}