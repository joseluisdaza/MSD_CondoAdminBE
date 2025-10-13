using NUnit.Framework;
using Condominio.Utils;
using Condominio.Models;
using Condominio.DTOs;
using System;

namespace Condominio.Tests.Utils
{
    [TestFixture]
    public class DTOConverterTests
    {
        [Test]
        public void ToUserBaseRequest_MapsPropertiesCorrectly()
        {
            var user = new User { Id = 1, UserName = "TestUser", LastName = "Last", LegalId = "123", Login = "login" };
            var dto = user.ToUserBaseRequest();
            Assert.AreEqual(user.Id, dto.Id);
            Assert.AreEqual(user.UserName, dto.UserName);
            Assert.AreEqual(user.LastName, dto.LastName);
            Assert.AreEqual(user.LegalId, dto.LegalId);
            Assert.AreEqual(user.Login, dto.Login);
        }

        [Test]
        public void ToUserRequest_IncludeIdAndPassword()
        {
            var user = new User { Id = 2, UserName = "User2", LastName = "Last2", LegalId = "456", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1), Password = "pass", Login = "login2" };
            var dto = user.ToUserRequest(includeId: true, includePassword: true);
            Assert.AreEqual(user.Id, dto.Id);
            Assert.AreEqual(user.Password, dto.Password);
        }

        [Test]
        public void ToUserRequest_ExcludeIdAndPassword()
        {
            var user = new User { Id = 3, UserName = "User3", LastName = "Last3", LegalId = "789", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2), Password = "pass3", Login = "login3" };
            var dto = user.ToUserRequest();
            Assert.AreEqual(0, dto.Id);
            Assert.IsNull(dto.Password);
        }

        [Test]
        public void ToUser_MapsPropertiesCorrectly()
        {
            var dto = new UserRequest { UserName = "User4", LastName = "Last4", LegalId = "101", StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(3), Login = "login4" };
            var user = dto.ToUser();
            Assert.AreEqual(dto.UserName, user.UserName);
            Assert.AreEqual(dto.LastName, user.LastName);
            Assert.AreEqual(dto.LegalId, user.LegalId);
            Assert.AreEqual(dto.StartDate, user.StartDate);
            Assert.AreEqual(dto.EndDate, user.EndDate);
            Assert.AreEqual(dto.Login, user.Login);
        }

        [Test]
        public void ToRoleRequest_MapsPropertiesCorrectly()
        {
            var role = new Role { Id = 5, RolName = "Admin" };
            var dto = role.ToRoleRequest();
            Assert.AreEqual(role.Id, dto.Id);
            Assert.AreEqual(role.RolName, dto.Name);
        }

        [Test]
        public void ToUserRole_MapsPropertiesCorrectly()
        {
            var req = new UserRoleRequest { UserId = 6, RoleId = 7 };
            var ur = req.ToUserRole();
            Assert.AreEqual(req.UserId, ur.UserId);
            Assert.AreEqual(req.RoleId, ur.RoleId);
        }

        [Test]
        public void ToPropertyTypeRequest_MapsPropertiesCorrectly()
        {
            var pt = new PropertyType { Id = 8, Type = "House", Description = "Desc", Rooms = 3, Bathrooms = 2, WaterService = true, StartDate = DateTime.Today };
            var dto = pt.ToPropertyTypeRequest();
            Assert.AreEqual(pt.Id, dto.Id);
            Assert.AreEqual(pt.Type, dto.Type);
            Assert.AreEqual(pt.Description, dto.Description);
            Assert.AreEqual(pt.Rooms, dto.Rooms);
            Assert.AreEqual(pt.Bathrooms, dto.Bathrooms);
            Assert.AreEqual(pt.WaterService, dto.WaterService);
            Assert.AreEqual(pt.StartDate, dto.StartDate);
        }

        [Test]
        public void ToPropertyType_MapsPropertiesCorrectly()
        {
            var dto = new PropertyTypeRequest { Type = "Apt", Description = "Desc2", Rooms = 2, Bathrooms = 1, WaterService = false, StartDate = DateTime.Today };
            var pt = dto.ToPropertyType();
            Assert.AreEqual(dto.Type, pt.Type);
            Assert.AreEqual(dto.Description, pt.Description);
            Assert.AreEqual(dto.Rooms, pt.Rooms);
            Assert.AreEqual(dto.Bathrooms, pt.Bathrooms);
            Assert.AreEqual(dto.WaterService, pt.WaterService);
            Assert.AreEqual(dto.StartDate, pt.StartDate);
            Assert.IsNull(pt.EndDate);
        }

        [Test]
        public void ToProperty_MapsPropertiesCorrectly()
        {
            var dto = new PropertyRequest { Id = 9, LegalId = "LID", Tower = "T1", Floor = 4, Code = "C1", PropertyType = 10, StartDate = DateTime.Today };
            var p = dto.ToProperty();
            Assert.AreEqual(dto.Id, p.Id);
            Assert.AreEqual(dto.LegalId, p.LegalId);
            Assert.AreEqual(dto.Tower, p.Tower);
            Assert.AreEqual(dto.Floor, p.Floor);
            Assert.AreEqual(dto.Code, p.Code);
            Assert.AreEqual(dto.PropertyType, p.PropertyType);
            Assert.AreEqual(dto.StartDate, p.StartDate);
            Assert.IsNull(p.EndDate);
        }

        [Test]
        public void ToPropertyRequest_MapsPropertiesCorrectly()
        {
            var p = new Property { Id = 11, LegalId = "LID2", Tower = "T2", Floor = 5, Code = "C2", PropertyType = 12, StartDate = DateTime.Today };
            var dto = p.ToPropertyRequest();
            Assert.AreEqual(p.Id, dto.Id);
            Assert.AreEqual(p.LegalId, dto.LegalId);
            Assert.AreEqual(p.Tower, dto.Tower);
            Assert.AreEqual(p.Floor, dto.Floor);
            Assert.AreEqual(p.Code, dto.Code);
            Assert.AreEqual(p.PropertyType, dto.PropertyType);
            Assert.AreEqual(p.StartDate, dto.StartDate);
        }
    }
}
