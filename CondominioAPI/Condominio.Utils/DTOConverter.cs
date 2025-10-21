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
                // Hash the password before storing it
                Password = !string.IsNullOrEmpty(userRequest.Password) 
                    ? PasswordHasher.HashPassword(userRequest.Password) 
                    : null,
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

        public static PropertyTypeRequest ToPropertyTypeRequest(this PropertyType propertyType)
        {
            if (propertyType == null) return null;
            return new PropertyTypeRequest
            {
                Id = propertyType.Id,
                Type = propertyType.Type,
                Description = propertyType.Description,
                Rooms = propertyType.Rooms,
                Bathrooms = propertyType.Bathrooms,
                WaterService = propertyType.WaterService,
                StartDate = propertyType.StartDate,
            };
        }

        public static PropertyType ToPropertyType(this PropertyTypeRequest propertyTypeRequest)
        {
            if (propertyTypeRequest == null) return null;
            return new PropertyType
            {
                Type = propertyTypeRequest.Type,
                Description = propertyTypeRequest.Description,
                Rooms = propertyTypeRequest.Rooms,
                Bathrooms = propertyTypeRequest.Bathrooms,
                WaterService = propertyTypeRequest.WaterService,
                StartDate = propertyTypeRequest.StartDate,
                EndDate = null
            };
        }

        public static Property ToProperty(this PropertyRequest propertyRequest)
        {
            if (propertyRequest == null) return null;
            return new Property
            {
                Id = propertyRequest.Id,
                LegalId = propertyRequest.LegalId,
                Tower = propertyRequest.Tower,
                Floor = propertyRequest.Floor,
                Code = propertyRequest.Code,
                PropertyType = propertyRequest.PropertyType,
                StartDate = propertyRequest.StartDate,
                EndDate = null
            };
        }

        public static PropertyRequest ToPropertyRequest(this Property property)
        {
            if (property == null) return null;
            return new PropertyRequest
            {
                Id = property.Id,
                LegalId = property.LegalId,
                Tower = property.Tower,
                Floor = property.Floor,
                Code = property.Code,
                PropertyType = property.PropertyType,
                StartDate = property.StartDate,
            };
        }
    }
}
