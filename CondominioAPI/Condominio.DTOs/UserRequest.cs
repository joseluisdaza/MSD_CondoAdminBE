using Condominio.DTOs.Validation;

namespace Condominio.DTOs
{
    public class UserRequest : UserBaseRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        [StrongPassword]
        public string Password { get; set; }
    }
}
