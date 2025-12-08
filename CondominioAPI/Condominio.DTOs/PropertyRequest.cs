namespace Condominio.DTOs
{
    public class PropertyRequest
    {
        public int Id { get; set; }

        public string? LegalId { get; set; }

        public string Tower { get; set; } = null!;

        public int Floor { get; set; }

        public string Code { get; set; } = null!;

        public int PropertyType { get; set; }

        public DateTime StartDate { get; set; }
    }

    public class FullPropertyRequest : PropertyRequest
    {
        public DateTime? EndDate { get; set; }
    }
}
