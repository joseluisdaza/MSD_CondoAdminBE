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

        // Null checks
        [Test]
        public void ToUserBaseRequest_WithNullUser_ReturnsNull()
        {
            var result = ((User)null).ToUserBaseRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToUserRequest_WithNullUser_ReturnsNull()
        {
            var result = ((User)null).ToUserRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToUser_WithNullUserRequest_ReturnsNull()
        {
            var result = ((UserRequest)null).ToUser();
            Assert.IsNull(result);
        }

        [Test]
        public void ToUser_WithNullNewUserRequest_ReturnsNull()
        {
            var result = ((NewUserRequest)null).ToUser();
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateDataNewUserBaseRequest_WithNullOld_ThrowsException()
        {
            var request = new NewUserBaseRequest { UserName = "Test", LastName = "User", LegalId = "123" };
            Assert.Throws<Exception>(() => ((User)null).UpdateDataNewUserBaseRequest(request));
        }

        [Test]
        public void UpdateDataNewUserBaseRequest_WithNullRequest_ThrowsException()
        {
            var user = new User { Id = 1 };
            Assert.Throws<Exception>(() => user.UpdateDataNewUserBaseRequest(null));
        }

        [Test]
        public void UpdateData_WithNullOld_ThrowsException()
        {
            var request = new NewUserRequest { UserName = "Test", LastName = "User", LegalId = "123", Login = "login" };
            Assert.Throws<Exception>(() => ((User)null).UpdateData(request));
        }

        [Test]
        public void UpdateData_WithNullRequest_ThrowsException()
        {
            var user = new User { Id = 1 };
            Assert.Throws<Exception>(() => user.UpdateData(null));
        }

        [Test]
        public void ToRoleRequest_WithNullRole_ReturnsNull()
        {
            var result = ((Role)null).ToRoleRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToUserRole_WithNullUserRoleRequest_ReturnsNull()
        {
            var result = ((UserRoleRequest)null).ToUserRole();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPropertyTypeRequest_WithNullPropertyType_ReturnsNull()
        {
            var result = ((PropertyType)null).ToPropertyTypeRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPropertyType_WithNullPropertyTypeRequest_ReturnsNull()
        {
            var result = ((PropertyTypeRequest)null).ToPropertyType();
            Assert.IsNull(result);
        }

        [Test]
        public void ToProperty_WithNullPropertyRequest_ReturnsNull()
        {
            var result = ((PropertyRequest)null).ToProperty();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPropertyRequest_WithNullProperty_ReturnsNull()
        {
            var result = ((Property)null).ToPropertyRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToFullPropertyRequest_WithNullProperty_ReturnsNull()
        {
            var result = ((Property)null).ToFullPropertyRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToFullPropertyRequest_MapsPropertiesCorrectly()
        {
            var p = new Property { Id = 20, LegalId = "LID3", Tower = "T3", Floor = 6, Code = "C3", PropertyType = 13, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(5) };
            var dto = p.ToFullPropertyRequest();
            Assert.AreEqual(p.Id, dto.Id);
            Assert.AreEqual(p.LegalId, dto.LegalId);
            Assert.AreEqual(p.Tower, dto.Tower);
            Assert.AreEqual(p.Floor, dto.Floor);
            Assert.AreEqual(p.Code, dto.Code);
            Assert.AreEqual(p.PropertyType, dto.PropertyType);
            Assert.AreEqual(p.StartDate, dto.StartDate);
            Assert.AreEqual(p.EndDate, dto.EndDate);
        }

        // Expense Conversions
        [Test]
        public void ToExpenseResponse_WithNullExpense_ReturnsNull()
        {
            var result = ((Expense)null).ToExpenseResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToExpenseResponse_MapsPropertiesCorrectly()
        {
            var expense = new Expense 
            { 
                Id = 1, CategoryId = 1, PropertyId = 1, StartDate = DateTime.Today, 
                PaymentLimitDate = DateTime.Today.AddDays(30), Amount = 100, 
                InterestAmount = 10, InterestRate = 0.1m, Description = "Test", StatusId = 1,
                Category = new ExpenseCategory { Category = "Utilities" },
                Property = new Property { Code = "A101", Tower = "T1" },
                Status = new PaymentStatus { StatusDescription = "Pending" }
            };
            var result = expense.ToExpenseResponse();
            Assert.AreEqual(expense.Id, result.Id);
            Assert.AreEqual(expense.CategoryId, result.CategoryId);
            Assert.AreEqual(expense.PropertyId, result.PropertyId);
            Assert.AreEqual(expense.Amount, result.Amount);
        }

        [Test]
        public void ToExpense_WithNullExpenseRequest_ReturnsNull()
        {
            var result = ((ExpenseRequest)null).ToExpense();
            Assert.IsNull(result);
        }

        [Test]
        public void ToExpense_MapsPropertiesCorrectly()
        {
            var dto = new ExpenseRequest 
            { 
                CategoryId = 2, PropertyId = 2, StartDate = DateTime.Today, 
                PaymentLimitDate = DateTime.Today.AddDays(30), Amount = 200, 
                InterestAmount = 20, InterestRate = 0.15m, Description = "Test", StatusId = 1
            };
            var expense = dto.ToExpense();
            Assert.AreEqual(dto.CategoryId, expense.CategoryId);
            Assert.AreEqual(dto.PropertyId, expense.PropertyId);
            Assert.AreEqual(dto.Amount, expense.Amount);
        }

        // Payment Conversions
        [Test]
        public void ToPaymentResponse_WithNullPayment_ReturnsNull()
        {
            var result = ((Payment)null).ToPaymentResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPaymentResponse_MapsPropertiesCorrectly()
        {
            var payment = new Payment 
            { 
                Id = 1, ReceiveNumber = "RCV001", PaymentDate = DateTime.Today, 
                Amount = 500, Description = "Payment", ReceivePhoto = "photo.jpg",
                ExpensePayments = new List<ExpensePayment>()
            };
            var result = payment.ToPaymentResponse();
            Assert.AreEqual(payment.Id, result.Id);
            Assert.AreEqual(payment.ReceiveNumber, result.ReceiveNumber);
            Assert.AreEqual(payment.Amount, result.Amount);
        }

        [Test]
        public void ToPayment_WithNullPaymentRequest_ReturnsNull()
        {
            var result = ((PaymentRequest)null).ToPayment();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPayment_MapsPropertiesCorrectly()
        {
            var dto = new PaymentRequest 
            { 
                ReceiveNumber = "RCV002", PaymentDate = DateTime.Today, 
                Amount = 600, Description = "Payment", ReceivePhoto = "photo.jpg"
            };
            var payment = dto.ToPayment();
            Assert.AreEqual(dto.ReceiveNumber, payment.ReceiveNumber);
            Assert.AreEqual(dto.Amount, payment.Amount);
        }

        // PaymentStatus Model Conversions (PaymentStatus is the model, not the enum)
        [Test]
        public void ToPaymentStatusResponse_WithNullPaymentStatus_ReturnsNull()
        {
            var result = ((PaymentStatus)null).ToPaymentStatusResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPaymentStatusResponse_MapsPropertiesCorrectly()
        {
            var paymentStatus = new PaymentStatus 
            { 
                Id = 1, StatusDescription = "Pending",
                Expenses = new List<Expense> { new Expense { Id = 1 }, new Expense { Id = 2 } },
                ServiceExpenses = new List<ServiceExpense> { new ServiceExpense { Id = 1 } },
                ServicePayments = new List<ServicePayment> { new ServicePayment { Id = 1 } }
            };
            var result = paymentStatus.ToPaymentStatusResponse();
            Assert.AreEqual(paymentStatus.Id, result.Id);
            Assert.AreEqual(paymentStatus.StatusDescription, result.StatusDescription);
            Assert.AreEqual(2, result.TotalExpenses);
            Assert.AreEqual(1, result.TotalServiceExpenses);
        }

        [Test]
        public void ToPaymentStatus_WithNullPaymentStatusRequest_ReturnsNull()
        {
            var result = ((PaymentStatusRequest)null).ToPaymentStatus();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPaymentStatus_MapsPropertiesCorrectly()
        {
            var dto = new PaymentStatusRequest { Id = 2, StatusDescription = "Paid" };
            var paymentStatus = dto.ToPaymentStatus();
            Assert.AreEqual(dto.Id, paymentStatus.Id);
            Assert.AreEqual(dto.StatusDescription, paymentStatus.StatusDescription);
        }

        [Test]
        public void ToPaymentStatusSimpleResponse_WithNullPaymentStatus_ReturnsNull()
        {
            var result = ((PaymentStatus)null).ToPaymentStatusSimpleResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPaymentStatusSimpleResponse_MapsPropertiesCorrectly()
        {
            var paymentStatus = new PaymentStatus { Id = 3, StatusDescription = "Verified" };
            var result = paymentStatus.ToPaymentStatusSimpleResponse();
            Assert.AreEqual(paymentStatus.Id, result.Id);
            Assert.AreEqual(paymentStatus.StatusDescription, result.StatusDescription);
        }

        // ExpensePayment Conversions
        [Test]
        public void ToExpensePaymentResponse_WithNullExpensePayment_ReturnsNull()
        {
            var result = ((ExpensePayment)null).ToExpensePaymentResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToExpensePaymentResponse_MapsPropertiesCorrectly()
        {
            var expensePayment = new ExpensePayment 
            { 
                Id = 1, ExpenseId = 1, PaymentId = 1,
                Expense = new Expense { Description = "ExpenseDesc", Amount = 100 },
                Payment = new Payment { ReceiveNumber = "RCV001", Amount = 100, PaymentDate = DateTime.Today }
            };
            var result = expensePayment.ToExpensePaymentResponse();
            Assert.AreEqual(expensePayment.Id, result.Id);
            Assert.AreEqual(expensePayment.ExpenseId, result.ExpenseId);
            Assert.AreEqual(expensePayment.PaymentId, result.PaymentId);
        }

        [Test]
        public void ToExpensePayment_WithNullExpensePaymentRequest_ReturnsNull()
        {
            var result = ((ExpensePaymentRequest)null).ToExpensePayment();
            Assert.IsNull(result);
        }

        [Test]
        public void ToExpensePayment_MapsPropertiesCorrectly()
        {
            var dto = new ExpensePaymentRequest { ExpenseId = 2, PaymentId = 2 };
            var expensePayment = dto.ToExpensePayment();
            Assert.AreEqual(dto.ExpenseId, expensePayment.ExpenseId);
            Assert.AreEqual(dto.PaymentId, expensePayment.PaymentId);
        }

        // ServiceType Conversions
        [Test]
        public void ToServiceTypeResponse_WithNullServiceType_ReturnsNull()
        {
            var result = ((ServiceType)null).ToServiceTypeResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServiceTypeResponse_MapsPropertiesCorrectly()
        {
            var serviceType = new ServiceType 
            { 
                Id = 1, ServiceName = "Water", Description = "Water Service",
                ServiceExpenses = new List<ServiceExpense> { new ServiceExpense { Id = 1 }, new ServiceExpense { Id = 2 } }
            };
            var result = serviceType.ToServiceTypeResponse();
            Assert.AreEqual(serviceType.Id, result.Id);
            Assert.AreEqual(serviceType.ServiceName, result.ServiceName);
            Assert.AreEqual(2, result.TotalServiceExpenses);
        }

        [Test]
        public void ToServiceType_WithNullServiceTypeRequest_ReturnsNull()
        {
            var result = ((ServiceTypeRequest)null).ToServiceType();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServiceType_MapsPropertiesCorrectly()
        {
            var dto = new ServiceTypeRequest { ServiceName = "Electricity", Description = "Power Service" };
            var serviceType = dto.ToServiceType();
            Assert.AreEqual(dto.ServiceName, serviceType.ServiceName);
            Assert.AreEqual(dto.Description, serviceType.Description);
        }

        // ServiceExpense Conversions
        [Test]
        public void ToServiceExpenseResponse_WithNullServiceExpense_ReturnsNull()
        {
            var result = ((ServiceExpense)null).ToServiceExpenseResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServiceExpenseResponse_MapsPropertiesCorrectly()
        {
            var serviceExpense = new ServiceExpense 
            { 
                Id = 1, ServiceTypeId = 1, Description = "Test", Amount = 50, 
                StartDate = DateTime.Today, PaymentLimitDate = DateTime.Today.AddDays(30),
                InterestAmount = 5, TotalAmount = 55, Status = 1, ExpenseDate = DateTime.Today, StatusId = 1,
                ServiceType = new ServiceType { ServiceName = "Water" },
                StatusNavigation = new PaymentStatus { StatusDescription = "Pending" }
            };
            var result = serviceExpense.ToServiceExpenseResponse();
            Assert.AreEqual(serviceExpense.Id, result.Id);
            Assert.AreEqual(serviceExpense.ServiceTypeId, result.ServiceTypeId);
            Assert.AreEqual(serviceExpense.Amount, result.Amount);
        }

        [Test]
        public void ToServiceExpense_WithNullServiceExpenseRequest_ReturnsNull()
        {
            var result = ((ServiceExpenseRequest)null).ToServiceExpense();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServiceExpense_MapsPropertiesCorrectly()
        {
            var dto = new ServiceExpenseRequest 
            { 
                ServiceTypeId = 2, Description = "Test2", Amount = 60, 
                StartDate = DateTime.Today, PaymentLimitDate = DateTime.Today.AddDays(30),
                InterestAmount = 6, TotalAmount = 66, Status = 1, ExpenseDate = DateTime.Today, StatusId = 1
            };
            var serviceExpense = dto.ToServiceExpense();
            Assert.AreEqual(dto.ServiceTypeId, serviceExpense.ServiceTypeId);
            Assert.AreEqual(dto.Amount, serviceExpense.Amount);
        }

        // ServicePayment Conversions
        [Test]
        public void ToServicePaymentResponse_WithNullServicePayment_ReturnsNull()
        {
            var result = ((ServicePayment)null).ToServicePaymentResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServicePaymentResponse_MapsPropertiesCorrectly()
        {
            var servicePayment = new ServicePayment 
            { 
                Id = 1, ReceiveNumber = "SRV001", PaymentDate = DateTime.Today, 
                Amount = 50, Description = "Service Payment", ReceivePhoto = "photo.jpg", StatusId = 1,
                Status = new PaymentStatus { StatusDescription = "Paid" }
            };
            var result = servicePayment.ToServicePaymentResponse();
            Assert.AreEqual(servicePayment.Id, result.Id);
            Assert.AreEqual(servicePayment.ReceiveNumber, result.ReceiveNumber);
            Assert.AreEqual(servicePayment.Amount, result.Amount);
        }

        [Test]
        public void ToServicePayment_WithNullServicePaymentRequest_ReturnsNull()
        {
            var result = ((ServicePaymentRequest)null).ToServicePayment();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServicePayment_MapsPropertiesCorrectly()
        {
            var dto = new ServicePaymentRequest 
            { 
                ReceiveNumber = "SRV002", PaymentDate = DateTime.Today, 
                Amount = 60, Description = "Service Payment", ReceivePhoto = "photo.jpg", StatusId = 2
            };
            var servicePayment = dto.ToServicePayment();
            Assert.AreEqual(dto.ReceiveNumber, servicePayment.ReceiveNumber);
            Assert.AreEqual(dto.Amount, servicePayment.Amount);
        }

        // ServiceExpensePayment Conversions
        [Test]
        public void ToServiceExpensePaymentResponse_WithNullServiceExpensePayment_ReturnsNull()
        {
            var result = ((ServiceExpensePayment)null).ToServiceExpensePaymentResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServiceExpensePaymentResponse_MapsPropertiesCorrectly()
        {
            var serviceExpensePayment = new ServiceExpensePayment 
            { 
                Id = 1, ServiceExpenseId = 1, PaymentId = 1,
                ServiceExpense = new ServiceExpense { Description = "ServiceExpenseDesc", Amount = 100 },
                Payment = new ServicePayment { ReceiveNumber = "SRV001", Amount = 100, PaymentDate = DateTime.Today }
            };
            var result = serviceExpensePayment.ToServiceExpensePaymentResponse();
            Assert.AreEqual(serviceExpensePayment.Id, result.Id);
            Assert.AreEqual(serviceExpensePayment.ServiceExpenseId, result.ServiceExpenseId);
        }

        [Test]
        public void ToServiceExpensePayment_WithNullServiceExpensePaymentRequest_ReturnsNull()
        {
            var result = ((ServiceExpensePaymentRequest)null).ToServiceExpensePayment();
            Assert.IsNull(result);
        }

        [Test]
        public void ToServiceExpensePayment_MapsPropertiesCorrectly()
        {
            var dto = new ServiceExpensePaymentRequest { ServiceExpenseId = 2, PaymentId = 2 };
            var serviceExpensePayment = dto.ToServiceExpensePayment();
            Assert.AreEqual(dto.ServiceExpenseId, serviceExpensePayment.ServiceExpenseId);
            Assert.AreEqual(dto.PaymentId, serviceExpensePayment.PaymentId);
        }

        // DatabaseVersion Conversions
        [Test]
        public void ToDatabaseVersionResponse_WithNullDatabaseVersion_ReturnsNull()
        {
            var result = ((DatabaseVersion)null).ToDatabaseVersionResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToDatabaseVersionResponse_MapsPropertiesCorrectly()
        {
            var databaseVersion = new DatabaseVersion { VersionNumber = "1.0.0", LastUpdated = DateTime.Today };
            var result = databaseVersion.ToDatabaseVersionResponse();
            Assert.AreEqual(databaseVersion.VersionNumber, result.VersionNumber);
            Assert.AreEqual(databaseVersion.LastUpdated, result.LastUpdated);
        }

        [Test]
        public void ToDatabaseVersion_WithNullDatabaseVersionRequest_ReturnsNull()
        {
            var result = ((DatabaseVersionRequest)null).ToDatabaseVersion();
            Assert.IsNull(result);
        }

        [Test]
        public void ToDatabaseVersion_MapsPropertiesCorrectly()
        {
            var dto = new DatabaseVersionRequest { VersionNumber = "1.0.1", LastUpdated = DateTime.Today };
            var databaseVersion = dto.ToDatabaseVersion();
            Assert.AreEqual(dto.VersionNumber, databaseVersion.VersionNumber);
            Assert.AreEqual(dto.LastUpdated, databaseVersion.LastUpdated);
        }

        // ExpenseCategory Conversions
        [Test]
        public void ToExpenseCategoryResponse_WithNullExpenseCategory_ReturnsNull()
        {
            var result = ((ExpenseCategory)null).ToExpenseCategoryResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToExpenseCategoryResponse_MapsPropertiesCorrectly()
        {
            var expenseCategory = new ExpenseCategory { Id = 1, Category = "Utilities", Description = "Utility expenses" };
            var result = expenseCategory.ToExpenseCategoryResponse();
            Assert.AreEqual(expenseCategory.Id, result.Id);
            Assert.AreEqual(expenseCategory.Category, result.Category);
            Assert.AreEqual(expenseCategory.Description, result.Description);
        }

        [Test]
        public void ToExpenseCategory_WithNullExpenseCategoryRequest_ReturnsNull()
        {
            var result = ((ExpenseCategoryRequest)null).ToExpenseCategory();
            Assert.IsNull(result);
        }

        [Test]
        public void ToExpenseCategory_MapsPropertiesCorrectly()
        {
            var dto = new ExpenseCategoryRequest { Id = 2, Category = "Maintenance", Description = "Maintenance expenses" };
            var expenseCategory = dto.ToExpenseCategory();
            Assert.AreEqual(dto.Id, expenseCategory.Id);
            Assert.AreEqual(dto.Category, expenseCategory.Category);
        }

        // PropertyOwner Conversions
        [Test]
        public void ToPropertyOwnerResponse_WithNullPropertyOwner_ReturnsNull()
        {
            var result = ((PropertyOwner)null).ToPropertyOwnerResponse();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPropertyOwnerResponse_MapsPropertiesCorrectly()
        {
            var propertyOwner = new PropertyOwner 
            { 
                PropertyId = 1, UserId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddYears(1),
                Property = new Property { LegalId = "LID", Tower = "T1", Floor = 5, Code = "C1" },
                User = new User { UserName = "John", LastName = "Doe", Login = "jdoe", LegalId = "ID123" }
            };
            var result = propertyOwner.ToPropertyOwnerResponse();
            Assert.AreEqual(propertyOwner.PropertyId, result.PropertyId);
            Assert.AreEqual(propertyOwner.UserId, result.UserId);
            Assert.AreEqual("John", result.UserName);
        }

        [Test]
        public void ToPropertyOwner_WithNullPropertyOwnerRequest_ReturnsNull()
        {
            var result = ((PropertyOwnerRequest)null).ToPropertyOwner();
            Assert.IsNull(result);
        }

        [Test]
        public void ToPropertyOwner_MapsPropertiesCorrectly()
        {
            var dto = new PropertyOwnerRequest { PropertyId = 2, UserId = 2 };
            var propertyOwner = dto.ToPropertyOwner();
            Assert.AreEqual(dto.PropertyId, propertyOwner.PropertyId);
            Assert.AreEqual(dto.UserId, propertyOwner.UserId);
            Assert.IsNull(propertyOwner.EndDate);
        }

        // Resource Conversions
        [Test]
        public void ToResource_WithNullResourceRequest_ReturnsNull()
        {
            var result = ((ResourceRequest)null).ToResource();
            Assert.IsNull(result);
        }

        [Test]
        public void ToResource_MapsPropertiesCorrectly()
        {
            var dto = new ResourceRequest { Name = "Pool", Description = "Swimming Pool", StartDate = DateTime.Today, Photo = "pool.jpg" };
            var resource = dto.ToResource();
            Assert.AreEqual(dto.Name, resource.Name);
            Assert.AreEqual(dto.Description, resource.Description);
            Assert.IsNull(resource.EndDate);
        }

        [Test]
        public void ToResourceRequest_WithNullResource_ReturnsNull()
        {
            var result = ((Resource)null).ToResourceRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToResourceRequest_MapsPropertiesCorrectly()
        {
            var resource = new Resource { Id = 1, Name = "Gym", Description = "Fitness Center", StartDate = DateTime.Today, Photo = "gym.jpg" };
            var result = resource.ToResourceRequest();
            Assert.AreEqual(resource.Id, result.Id);
            Assert.AreEqual(resource.Name, result.Name);
        }

        [Test]
        public void ToFullResourceRequest_WithNullResource_ReturnsNull()
        {
            var result = ((Resource)null).ToFullResourceRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToFullResourceRequest_MapsPropertiesCorrectly()
        {
            var resource = new Resource { Id = 2, Name = "Park", Description = "Garden", StartDate = DateTime.Today, Photo = "park.jpg", EndDate = DateTime.Today.AddYears(1) };
            var result = resource.ToFullResourceRequest();
            Assert.AreEqual(resource.Id, result.Id);
            Assert.AreEqual(resource.EndDate, result.EndDate);
        }

        // ResourceCost Conversions
        [Test]
        public void ToResourceCost_WithNullResourceCostRequest_ReturnsNull()
        {
            var result = ((ResourceCostRequest)null).ToResourceCost();
            Assert.IsNull(result);
        }

        [Test]
        public void ToResourceCost_MapsPropertiesCorrectly()
        {
            var dto = new ResourceCostRequest { ResourceId = 1, BookingPrice = 100, BookingWarrantyCost = 50, StartDate = DateTime.Today };
            var resourceCost = dto.ToResourceCost();
            Assert.AreEqual(dto.ResourceId, resourceCost.ResourceId);
            Assert.AreEqual(dto.BookingPrice, resourceCost.BookingPrice);
            Assert.IsNull(resourceCost.EndDate);
        }

        [Test]
        public void ToResourceCostRequest_WithNullResourceCost_ReturnsNull()
        {
            var result = ((ResourceCost)null).ToResourceCostRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToResourceCostRequest_MapsPropertiesCorrectly()
        {
            var resourceCost = new ResourceCost { Id = 1, ResourceId = 2, BookingPrice = 150, BookingWarrantyCost = 75, StartDate = DateTime.Today };
            var result = resourceCost.ToResourceCostRequest();
            Assert.AreEqual(resourceCost.Id, result.Id);
            Assert.AreEqual(resourceCost.BookingPrice, result.BookingPrice);
        }

        [Test]
        public void ToFullResourceCostRequest_WithNullResourceCost_ReturnsNull()
        {
            var result = ((ResourceCost)null).ToFullResourceCostRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToFullResourceCostRequest_MapsPropertiesCorrectly()
        {
            var resourceCost = new ResourceCost { Id = 2, ResourceId = 3, BookingPrice = 200, BookingWarrantyCost = 100, StartDate = DateTime.Today, EndDate = DateTime.Today.AddYears(1) };
            var result = resourceCost.ToFullResourceCostRequest();
            Assert.AreEqual(resourceCost.Id, result.Id);
            Assert.AreEqual(resourceCost.EndDate, result.EndDate);
        }

        // ResourceBooking Conversions
        [Test]
        public void ToResourceBooking_WithNullResourceBookingRequest_ReturnsNull()
        {
            var result = ((ResourceBookingRequest)null).ToResourceBooking();
            Assert.IsNull(result);
        }

        [Test]
        public void ToResourceBooking_WithBookingEndDate_MapsPropertiesCorrectly()
        {
            var dto = new ResourceBookingRequest 
            { 
                ResourceId = 1, UserId = 1, PropertyId = 1, StatusId = 1,
                BookingDate = DateTime.Today, BookingEndDate = DateTime.Today.AddDays(1),
                BookingPrice = 100, BookingWarrantyCost = 50, BookingDescription = "Test", BookingPhoto = "photo.jpg"
            };
            var resourceBooking = dto.ToResourceBooking();
            Assert.AreEqual(dto.ResourceId, resourceBooking.ResourceId);
            Assert.AreEqual(dto.BookingEndDate, resourceBooking.BookingEndDate);
        }

        [Test]
        public void ToResourceBooking_WithoutBookingEndDate_SetsDefaultEndDate()
        {
            var bookingDate = new DateTime(2024, 1, 15, 10, 0, 0);
            var dto = new ResourceBookingRequest 
            { 
                ResourceId = 1, UserId = 1, PropertyId = 1, StatusId = 1,
                BookingDate = bookingDate, BookingEndDate = null,
                BookingPrice = 100, BookingWarrantyCost = 50, BookingDescription = "Test", BookingPhoto = "photo.jpg"
            };
            var resourceBooking = dto.ToResourceBooking();
            var expectedEndDate = bookingDate.Date.AddDays(1).AddSeconds(-1);
            Assert.AreEqual(expectedEndDate, resourceBooking.BookingEndDate);
        }

        [Test]
        public void ToResourceBookingRequest_WithNullResourceBooking_ReturnsNull()
        {
            var result = ((ResourceBooking)null).ToResourceBookingRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToResourceBookingRequest_MapsPropertiesCorrectly()
        {
            var resourceBooking = new ResourceBooking 
            { 
                Id = 1, ResourceId = 2, UserId = 2, PropertyId = 2, StatusId = 2,
                BookingDate = DateTime.Today, BookingEndDate = DateTime.Today.AddDays(1),
                BookingPrice = 150, BookingWarrantyCost = 75, BookingDescription = "Test2", BookingPhoto = "photo2.jpg"
            };
            var result = resourceBooking.ToResourceBookingRequest();
            Assert.AreEqual(resourceBooking.Id, result.Id);
            Assert.AreEqual(resourceBooking.BookingPrice, result.BookingPrice);
        }

        // IncidentType Conversions
        [Test]
        public void ToIncidentType_WithNullIncidentTypeRequest_ReturnsNull()
        {
            var result = ((IncidentTypeRequest)null).ToIncidentType();
            Assert.IsNull(result);
        }

        [Test]
        public void ToIncidentType_MapsPropertiesCorrectly()
        {
            var dto = new IncidentTypeRequest { Type = "Damage", Description = "Property Damage" };
            var incidentType = dto.ToIncidentType();
            Assert.AreEqual(dto.Type, incidentType.Type);
            Assert.AreEqual(dto.Description, incidentType.Description);
        }

        [Test]
        public void ToIncidentTypeRequest_WithNullIncidentType_ReturnsNull()
        {
            var result = ((IncidentType)null).ToIncidentTypeRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToIncidentTypeRequest_MapsPropertiesCorrectly()
        {
            var incidentType = new IncidentType { Id = 1, Type = "Noise", Description = "Noise Complaint" };
            var result = incidentType.ToIncidentTypeRequest();
            Assert.AreEqual(incidentType.Id, result.Id);
            Assert.AreEqual(incidentType.Type, result.Type);
        }

        // IncidentCost Conversions
        [Test]
        public void ToIncidentCost_WithNullIncidentCostRequest_ReturnsNull()
        {
            var result = ((IncidentCostRequest)null).ToIncidentCost();
            Assert.IsNull(result);
        }

        [Test]
        public void ToIncidentCost_MapsPropertiesCorrectly()
        {
            var dto = new IncidentCostRequest { IncidentTypeId = 1, Cost = 500, StartDate = DateTime.Today, Description = "Cost for damage" };
            var incidentCost = dto.ToIncidentCost();
            Assert.AreEqual(dto.IncidentTypeId, incidentCost.IncidentTypeId);
            Assert.AreEqual(dto.Cost, incidentCost.Cost);
            Assert.IsNull(incidentCost.EndDate);
        }

        [Test]
        public void ToIncidentCostRequest_WithNullIncidentCost_ReturnsNull()
        {
            var result = ((IncidentCost)null).ToIncidentCostRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToIncidentCostRequest_MapsPropertiesCorrectly()
        {
            var incidentCost = new IncidentCost { Id = 1, IncidentTypeId = 2, Cost = 600, StartDate = DateTime.Today, Description = "Cost" };
            var result = incidentCost.ToIncidentCostRequest();
            Assert.AreEqual(incidentCost.Id, result.Id);
            Assert.AreEqual(incidentCost.Cost, result.Cost);
        }

        [Test]
        public void ToFullIncidentCostRequest_WithNullIncidentCost_ReturnsNull()
        {
            var result = ((IncidentCost)null).ToFullIncidentCostRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToFullIncidentCostRequest_MapsPropertiesCorrectly()
        {
            var incidentCost = new IncidentCost { Id = 2, IncidentTypeId = 3, Cost = 700, StartDate = DateTime.Today, Description = "Cost", EndDate = DateTime.Today.AddYears(1) };
            var result = incidentCost.ToFullIncidentCostRequest();
            Assert.AreEqual(incidentCost.Id, result.Id);
            Assert.AreEqual(incidentCost.EndDate, result.EndDate);
        }

        // Incident Conversions
        [Test]
        public void ToIncident_WithNullIncidentRequest_ReturnsNull()
        {
            var result = ((IncidentRequest)null).ToIncident();
            Assert.IsNull(result);
        }

        [Test]
        public void ToIncident_MapsPropertiesCorrectly()
        {
            var dto = new IncidentRequest 
            { 
                IncidentTypeId = 1, UserId = 1, PropertyId = 1, StatusId = 1,
                IncidentDate = DateTime.Today, IncidentDescription = "Test", IncidentPhoto = "incident.jpg"
            };
            var incident = dto.ToIncident();
            Assert.AreEqual(dto.IncidentTypeId, incident.IncidentTypeId);
            Assert.AreEqual(dto.UserId, incident.UserId);
        }

        [Test]
        public void ToIncidentRequest_WithNullIncident_ReturnsNull()
        {
            var result = ((Incident)null).ToIncidentRequest();
            Assert.IsNull(result);
        }

        [Test]
        public void ToIncidentRequest_MapsPropertiesCorrectly()
        {
            var incident = new Incident 
            { 
                Id = 1, IncidentTypeId = 2, UserId = 2, PropertyId = 2, StatusId = 2,
                IncidentDate = DateTime.Today, IncidentDescription = "Test2", IncidentPhoto = "incident2.jpg"
            };
            var result = incident.ToIncidentRequest();
            Assert.AreEqual(incident.Id, result.Id);
            Assert.AreEqual(incident.IncidentTypeId, result.IncidentTypeId);
        }

        // Conversion with no password
        [Test]
        public void ToUser_NewUserRequest_WithNullPassword_HasNullPassword()
        {
            var dto = new NewUserRequest { UserName = "User", LastName = "Test", LegalId = "456", Login = "usertest", Password = null };
            var user = dto.ToUser();
            Assert.IsNull(user.Password);
        }

        [Test]
        public void ToUser_UserRequest_WithNullPassword_HasNullPassword()
        {
            var dto = new UserRequest { UserName = "User", LastName = "Test", LegalId = "789", Login = "usertest2", Password = null };
            var user = dto.ToUser();
            Assert.IsNull(user.Password);
        }

        // UpdateDataNewUserBaseRequest test
        [Test]
        public void UpdateDataNewUserBaseRequest_MapsPropertiesCorrectly()
        {
            var user = new User { Id = 1, UserName = "OldName", LastName = "OldLast", LegalId = "OldId" };
            var request = new NewUserBaseRequest { UserName = "NewName", LastName = "NewLast", LegalId = "NewId" };
            user.UpdateDataNewUserBaseRequest(request);
            Assert.AreEqual("NewName", user.UserName);
            Assert.AreEqual("NewLast", user.LastName);
            Assert.AreEqual("NewId", user.LegalId);
        }

        // UpdateData test
        [Test]
        public void UpdateData_MapsPropertiesCorrectly()
        {
            var user = new User { Id = 1, UserName = "OldName", LastName = "OldLast", LegalId = "OldId", Password = "OldPass" };
            var request = new NewUserRequest { UserName = "NewName2", LastName = "NewLast2", LegalId = "NewId2", Login = "newlogin", Password = "NewPass" };
            user.UpdateData(request);
            Assert.AreEqual("NewName2", user.UserName);
            Assert.AreEqual("NewLast2", user.LastName);
            Assert.AreNotEqual("OldPass", user.Password);
        }

        // Test ToUser with NewUserRequest and validate trimming
        [Test]
        public void ToUser_NewUserRequest_TrimsStrings()
        {
            var dto = new NewUserRequest { UserName = "  User  ", LastName = "  Test  ", LegalId = "  456  ", Login = "  login  ", Password = "pass" };
            var user = dto.ToUser();
            Assert.AreEqual("User", user.UserName);
            Assert.AreEqual("Test", user.LastName);
            Assert.AreEqual("456", user.LegalId); // LegalId is trimmed in ToUser implementation
            Assert.AreEqual("login", user.Login);
        }
    }
}
