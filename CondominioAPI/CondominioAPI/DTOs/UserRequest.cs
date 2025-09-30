namespace CondominioAPI.DTOs
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string LegalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Password { get; set; }
    }
}
