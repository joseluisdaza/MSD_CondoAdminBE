namespace CondominioAPI.DTOs
{
    public class UserRequest : UserBaseRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Password { get; set; }
    }
}
