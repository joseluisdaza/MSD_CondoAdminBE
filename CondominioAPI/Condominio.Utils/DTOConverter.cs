using Condominio.Models;
using Condominio.DTOs;

namespace Condominio.Utils
{
    public static class DTOConverter
    {

        public static UserBaseRequest ToUserBaseRequest(this User user)
        {
            if (user == null) return null;

            return new UserBaseRequest
            {
                Id = user.Id,
                UserName = user.UserName,
                LastName = user.LastName,
                LegalId = user.LegalId,
                Login = user.Login
            };
        }

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
                Password = includePassword ? user.Password : null,
                Login = user.Login
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
                Password = null,
                Login = userRequest.Login
            };
        }

        public static RoleRequest ToRoleRequest(this Role role)
        {
            if (role == null) return null;
            
            return new RoleRequest
            {
                Id = role.Id,
                Name = role.RolName
            };
        }

        public static UserRole ToUserRole(this UserRoleRequest userRoleRequest)
        {
            if (userRoleRequest == null) return null;
            return new UserRole
            {
                UserId = userRoleRequest.UserId,
                RoleId = userRoleRequest.RoleId,
            };
        }
    }
}
