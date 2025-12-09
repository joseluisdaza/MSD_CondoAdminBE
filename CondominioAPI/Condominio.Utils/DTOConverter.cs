using Condominio.Models;
using Condominio.DTOs;
using System.Linq;

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
                Password = HashPassword(userRequest.Password),
                Login = userRequest.Login
            };
        }
        
        private static string HashPassword(string password)
        {
            return !string.IsNullOrEmpty(password)
                    ? PasswordHasher.HashPassword(password)
                    : null;
        }


        public static User ToUser(this NewUserRequest userRequest)
        {
            if (userRequest == null) return null;
            return new User
            {
                UserName = userRequest.UserName.Trim(),
                LastName = userRequest.LastName.Trim(),
                LegalId = userRequest.LegalId.Trim(),
                StartDate = DateTime.Now,
                EndDate = null,
                Password = HashPassword(userRequest.Password),
                Login = userRequest.Login.Trim()
            };
        }

        public static void UpdateDataNewUserBaseRequest(this User old, NewUserBaseRequest request)
        {
            if (old == null || request == null) throw new Exception("Users cannot be null.");

            old.UserName = request.UserName.Trim();
            old.LastName = request.LastName.Trim();
            old.LegalId = request.LegalId;
        }

        public static void UpdateData(this User old, NewUserRequest request)
        {
            if (old == null || request == null) throw new Exception("Users cannot be null.");

            old.UserName = request.UserName.Trim();
            old.LastName = request.LastName.Trim();
            old.LegalId = request.LegalId;
            old.Password = HashPassword(request.Password);
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
                Id = propertyTypeRequest.Id,
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

        public static FullPropertyRequest ToFullPropertyRequest(this Property property)
        {
            if (property == null) return null;
            return new FullPropertyRequest
            {
                Id = property.Id,
                LegalId = property.LegalId,
                Tower = property.Tower,
                Floor = property.Floor,
                Code = property.Code,
                PropertyType = property.PropertyType,
                StartDate = property.StartDate,
                EndDate = property.EndDate
            };
        }

        // Expense conversions
        public static ExpenseResponse ToExpenseResponse(this Expense expense)
        {
            if (expense == null) return null;

            return new ExpenseResponse
            {
                Id = expense.Id,
                CategoryId = expense.CategoryId,
                PropertyId = expense.PropertyId,
                StartDate = expense.StartDate,
                PaymentLimitDate = expense.PaymentLimitDate,
                Amount = expense.Amount,
                InterestAmount = expense.InterestAmount,
                InterestRate = expense.InterestRate,
                Description = expense.Description,
                StatusId = expense.StatusId,
                CategoryName = expense.Category?.Category,
                PropertyCode = expense.Property?.Code,
                PropertyTower = expense.Property?.Tower,
                StatusDescription = expense.Status?.StatusDescription,
                //Payments = expense.ExpensePayments?.Select(ep => ep.ToExpensePaymentResponse()).ToList() ?? new List<ExpensePaymentResponse>()
            };
        }

        public static Expense ToExpense(this ExpenseRequest expenseRequest)
        {
            if (expenseRequest == null) return null;

            return new Expense
            {
                CategoryId = expenseRequest.CategoryId,
                PropertyId = expenseRequest.PropertyId,
                StartDate = expenseRequest.StartDate,
                PaymentLimitDate = expenseRequest.PaymentLimitDate,
                Amount = expenseRequest.Amount,
                InterestAmount = expenseRequest.InterestAmount,
                InterestRate = expenseRequest.InterestRate,
                Description = expenseRequest.Description,
                StatusId = expenseRequest.StatusId
            };
        }

        // Payment conversions
        public static PaymentResponse ToPaymentResponse(this Payment payment)
        {
            if (payment == null) return null;

            return new PaymentResponse
            {
                Id = payment.Id,
                ReceiveNumber = payment.ReceiveNumber,
                PaymentDate = payment.PaymentDate,
                Amount = payment.Amount,
                Description = payment.Description,
                ReceivePhoto = payment.ReceivePhoto,
                Expenses = payment.ExpensePayments?.Select(ep => ep.ToExpensePaymentResponse()).ToList() ?? new List<ExpensePaymentResponse>()
            };
        }

        public static Payment ToPayment(this PaymentRequest paymentRequest)
        {
            if (paymentRequest == null) return null;

            return new Payment
            {
                ReceiveNumber = paymentRequest.ReceiveNumber,
                PaymentDate = paymentRequest.PaymentDate,
                Amount = paymentRequest.Amount,
                Description = paymentRequest.Description,
                ReceivePhoto = paymentRequest.ReceivePhoto
            };
        }

        // PaymentStatus conversions
        public static PaymentStatusResponse ToPaymentStatusResponse(this PaymentStatus paymentStatus)
        {
            if (paymentStatus == null) return null;

            return new PaymentStatusResponse
            {
                Id = paymentStatus.Id,
                StatusDescription = paymentStatus.StatusDescription,
                TotalExpenses = paymentStatus.Expenses?.Count ?? 0,
                TotalServiceExpenses = paymentStatus.ServiceExpenses?.Count ?? 0,
                TotalServicePayments = paymentStatus.ServicePayments?.Count ?? 0
            };
        }

        public static PaymentStatus ToPaymentStatus(this PaymentStatusRequest paymentStatusRequest)
        {
            if (paymentStatusRequest == null) return null;

            return new PaymentStatus
            {
                Id = paymentStatusRequest.Id,
                StatusDescription = paymentStatusRequest.StatusDescription
            };
        }

        // PaymentStatus simple conversion (only Id and StatusDescription)
        public static PaymentStatusSimpleResponse ToPaymentStatusSimpleResponse(this PaymentStatus paymentStatus)
        {
            if (paymentStatus == null) return null;

            return new PaymentStatusSimpleResponse
            {
                Id = paymentStatus.Id,
                StatusDescription = paymentStatus.StatusDescription
            };
        }

        // ExpensePayment conversions
        public static ExpensePaymentResponse ToExpensePaymentResponse(this ExpensePayment expensePayment)
        {
            if (expensePayment == null) return null;

            return new ExpensePaymentResponse
            {
                Id = expensePayment.Id,
                ExpenseId = expensePayment.ExpenseId,
                PaymentId = expensePayment.PaymentId,
                ExpenseDescription = expensePayment.Expense?.Description,
                ExpenseAmount = expensePayment.Expense?.Amount,
                PaymentReceiveNumber = expensePayment.Payment?.ReceiveNumber,
                PaymentAmount = expensePayment.Payment?.Amount,
                PaymentDate = expensePayment.Payment?.PaymentDate
            };
        }

        public static ExpensePayment ToExpensePayment(this ExpensePaymentRequest expensePaymentRequest)
        {
            if (expensePaymentRequest == null) return null;

            return new ExpensePayment
            {
                ExpenseId = expensePaymentRequest.ExpenseId,
                PaymentId = expensePaymentRequest.PaymentId
            };
        }

        // ServiceType conversions
        public static ServiceTypeResponse ToServiceTypeResponse(this ServiceType serviceType)
        {
            if (serviceType == null) return null;

            return new ServiceTypeResponse
            {
                Id = serviceType.Id,
                ServiceName = serviceType.ServiceName,
                Description = serviceType.Description,
                TotalServiceExpenses = serviceType.ServiceExpenses?.Count ?? 0
            };
        }

        public static ServiceType ToServiceType(this ServiceTypeRequest serviceTypeRequest)
        {
            if (serviceTypeRequest == null) return null;

            return new ServiceType
            {
                ServiceName = serviceTypeRequest.ServiceName,
                Description = serviceTypeRequest.Description
            };
        }

        // ServiceExpense conversions
        public static ServiceExpenseResponse ToServiceExpenseResponse(this ServiceExpense serviceExpense)
        {
            if (serviceExpense == null) return null;

            return new ServiceExpenseResponse
            {
                Id = serviceExpense.Id,
                ServiceTypeId = serviceExpense.ServiceTypeId,
                Description = serviceExpense.Description,
                Amount = serviceExpense.Amount,
                StartDate = serviceExpense.StartDate,
                PaymentLimitDate = serviceExpense.PaymentLimitDate,
                InterestAmount = serviceExpense.InterestAmount,
                TotalAmount = serviceExpense.TotalAmount,
                Status = serviceExpense.Status,
                ExpenseDate = serviceExpense.ExpenseDate,
                StatusId = serviceExpense.StatusId,
                ServiceTypeName = serviceExpense.ServiceType?.ServiceName,
                StatusDescription = serviceExpense.StatusNavigation?.StatusDescription,
                //Payments = serviceExpense.ServiceExpensePayments?.Select(sep => sep.ToServiceExpensePaymentResponse()).ToList() ?? new List<ServiceExpensePaymentResponse>()
            };
        }

        public static ServiceExpense ToServiceExpense(this ServiceExpenseRequest serviceExpenseRequest)
        {
            if (serviceExpenseRequest == null) return null;

            return new ServiceExpense
            {
                ServiceTypeId = serviceExpenseRequest.ServiceTypeId,
                Description = serviceExpenseRequest.Description,
                Amount = serviceExpenseRequest.Amount,
                StartDate = serviceExpenseRequest.StartDate,
                PaymentLimitDate = serviceExpenseRequest.PaymentLimitDate,
                InterestAmount = serviceExpenseRequest.InterestAmount,
                TotalAmount = serviceExpenseRequest.TotalAmount,
                Status = serviceExpenseRequest.Status,
                ExpenseDate = serviceExpenseRequest.ExpenseDate,
                StatusId = serviceExpenseRequest.StatusId
            };
        }

        // ServicePayment conversions
        public static ServicePaymentResponse ToServicePaymentResponse(this ServicePayment servicePayment)
        {
            if (servicePayment == null) return null;

            return new ServicePaymentResponse
            {
                Id = servicePayment.Id,
                ReceiveNumber = servicePayment.ReceiveNumber,
                PaymentDate = servicePayment.PaymentDate,
                Amount = servicePayment.Amount,
                Description = servicePayment.Description,
                ReceivePhoto = servicePayment.ReceivePhoto,
                StatusId = servicePayment.StatusId,
                StatusDescription = servicePayment.Status?.StatusDescription,
                //ServiceExpenses = servicePayment.ServiceExpensePayments?.Select(sep => sep.ToServiceExpensePaymentResponse()).ToList() ?? new List<ServiceExpensePaymentResponse>()
            };
        }

        public static ServicePayment ToServicePayment(this ServicePaymentRequest servicePaymentRequest)
        {
            if (servicePaymentRequest == null) return null;

            return new ServicePayment
            {
                ReceiveNumber = servicePaymentRequest.ReceiveNumber,
                PaymentDate = servicePaymentRequest.PaymentDate,
                Amount = servicePaymentRequest.Amount,
                Description = servicePaymentRequest.Description,
                ReceivePhoto = servicePaymentRequest.ReceivePhoto,
                StatusId = servicePaymentRequest.StatusId
            };
        }

        // ServiceExpensePayment conversions
        public static ServiceExpensePaymentResponse ToServiceExpensePaymentResponse(this ServiceExpensePayment serviceExpensePayment)
        {
            if (serviceExpensePayment == null) return null;

            return new ServiceExpensePaymentResponse
            {
                Id = serviceExpensePayment.Id,
                ServiceExpenseId = serviceExpensePayment.ServiceExpenseId,
                PaymentId = serviceExpensePayment.PaymentId,
                ServiceExpenseDescription = serviceExpensePayment.ServiceExpense?.Description,
                ServiceExpenseAmount = serviceExpensePayment.ServiceExpense?.Amount,
                PaymentReceiveNumber = serviceExpensePayment.Payment?.ReceiveNumber,
                PaymentAmount = serviceExpensePayment.Payment?.Amount,
                PaymentDate = serviceExpensePayment.Payment?.PaymentDate
            };
        }

        public static ServiceExpensePayment ToServiceExpensePayment(this ServiceExpensePaymentRequest serviceExpensePaymentRequest)
        {
            if (serviceExpensePaymentRequest == null) return null;

            return new ServiceExpensePayment
            {
                ServiceExpenseId = serviceExpensePaymentRequest.ServiceExpenseId,
                PaymentId = serviceExpensePaymentRequest.PaymentId
            };
        }

        // DatabaseVersion conversions
        public static DatabaseVersionResponse ToDatabaseVersionResponse(this DatabaseVersion databaseVersion)
        {
            if (databaseVersion == null) return null;

            return new DatabaseVersionResponse
            {
                VersionNumber = databaseVersion.VersionNumber,
                LastUpdated = databaseVersion.LastUpdated
            };
        }

        public static DatabaseVersion ToDatabaseVersion(this DatabaseVersionRequest databaseVersionRequest)
        {
            if (databaseVersionRequest == null) return null;

            return new DatabaseVersion
            {
                VersionNumber = databaseVersionRequest.VersionNumber,
                LastUpdated = databaseVersionRequest.LastUpdated
            };
        }

        // ExpenseCategory conversions
        public static ExpenseCategoryResponse ToExpenseCategoryResponse(this ExpenseCategory expenseCategory)
        {
            if (expenseCategory == null) return null;

            return new ExpenseCategoryResponse
            {
                Id = expenseCategory.Id,
                Category = expenseCategory.Category,
                Description = expenseCategory.Description,
                //TotalExpenses = expenseCategory.Expenses?.Count ?? 0
            };
        }

        public static ExpenseCategory ToExpenseCategory(this ExpenseCategoryRequest expenseCategoryRequest)
        {
            if (expenseCategoryRequest == null) return null;

            return new ExpenseCategory
            {
                Id = expenseCategoryRequest.Id,
                Category = expenseCategoryRequest.Category,
                Description = expenseCategoryRequest.Description
            };
        }

        // PropertyOwner conversions
        public static PropertyOwnerResponse ToPropertyOwnerResponse(this PropertyOwner propertyOwner)
        {
            if (propertyOwner == null) return null;

            return new PropertyOwnerResponse
            {
                PropertyId = propertyOwner.PropertyId,
                UserId = propertyOwner.UserId,
                StartDate = propertyOwner.StartDate,
                EndDate = propertyOwner.EndDate,
                PropertyLegalId = propertyOwner.Property?.LegalId,
                PropertyTower = propertyOwner.Property?.Tower,
                PropertyFloor = propertyOwner.Property?.Floor,
                PropertyCode = propertyOwner.Property?.Code,
                UserName = propertyOwner.User?.UserName,
                UserLastName = propertyOwner.User?.LastName,
                UserLogin = propertyOwner.User?.Login,
                UserLegalId = propertyOwner.User?.LegalId
            };
        }

        public static PropertyOwner ToPropertyOwner(this PropertyOwnerRequest propertyOwnerRequest)
        {
            if (propertyOwnerRequest == null) return null;

            return new PropertyOwner
            {
                PropertyId = propertyOwnerRequest.PropertyId,
                UserId = propertyOwnerRequest.UserId,
                StartDate = DateTime.Now,
                EndDate = null
            };
        }
    }
}
