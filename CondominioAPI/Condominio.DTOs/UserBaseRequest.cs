namespace Condominio.DTOs
{
    public class UserBaseRequest : NewUserBaseRequest
    {
        public int Id { get; set; }
    }

    public class NewUserBaseRequest
    {
        public string UserName { get; set; }
        public string LastName { get; set; }
        public string LegalId { get; set; }
        public string Login { get; set; }
    }
}
