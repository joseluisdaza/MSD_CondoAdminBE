namespace CondominioAPI.DTOs
{
    public class UserBaseRequest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string LegalId { get; set; }
        public string Login { get; set; }
    }
}
