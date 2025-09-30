using Condominio.Data.MySql.Models;
using Condominio.Models;
using CondominioAPI.DTOs;

namespace CondominioAPI.Utils
{
    public static class DTOConverter
    {
        public static UserRequest ToUserRequest(this User user, bool includeId = false, bool includePassword = false)
        {
            if (user == null) return null;

            return new UserRequest
            {
                Id = includeId ? user.Id : 0,
                UserName = user.UserName,
                LastName = user.LastName,
                LegalId = user.LegalId,
                StartDate = user.StartDate,
                EndDate = user.EndDate ?? default,
                Password = includePassword ? user.Password : null
            };
        }

        public static User ToUser(this UserRequest userRequest)
        {
            if (userRequest == null) return null;
            return new User
            {
                UserName = userRequest.UserName,
                LastName = userRequest.LastName,
                LegalId = userRequest.LegalId,
                StartDate = userRequest.StartDate,
                EndDate = userRequest.EndDate.HasValue ? userRequest.EndDate.Value : null,
                Password = userRequest.Password
            };
        }
    }
}
