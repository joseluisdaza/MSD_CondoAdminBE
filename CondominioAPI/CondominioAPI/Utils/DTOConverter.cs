using Condominio.Data.MySql.Models;
using CondominioAPI.DTOs;

namespace CondominioAPI.Utils
{
    public static class DTOConverter
    {
        public static UserRequest ToUserRequest(this User user)
        {
            if (user == null) return null;

            return new UserRequest
            {
                UserName = user.UserName,
                LastName = user.LastName,
                LegalId = user.LegalId,
                StartDate = user.StartDate,
                EndDate = user.EndDate ?? default,
                Password = user.Password
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
                EndDate = userRequest.EndDate == default ? null : userRequest.EndDate,
                Password = userRequest.Password
            };
        }
    }
}
