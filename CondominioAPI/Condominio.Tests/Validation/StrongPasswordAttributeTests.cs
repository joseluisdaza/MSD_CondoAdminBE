using NUnit.Framework;
using Condominio.DTOs;
using Condominio.DTOs.Validation;
using System.ComponentModel.DataAnnotations;

namespace Condominio.Tests.Validation
{
    [TestFixture]
    public class StrongPasswordAttributeTests
    {
        [Test]
        public void ValidPassword_PassesValidation()
        {
            // Arrange
            var password = "SecurePass123!";
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [Test]
        public void ShortPassword_FailsValidation()
        {
            // Arrange
            var password = "Short1!";
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.ErrorMessage, Does.Contain("12 caracteres"));
        }

        [Test]
        public void PasswordWithoutNumber_FailsValidation()
        {
            // Arrange
            var password = "NoNumberHere!";
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.ErrorMessage, Does.Contain("1 número"));
        }

        [Test]
        public void PasswordWithoutUppercase_FailsValidation()
        {
            // Arrange
            var password = "nouppercase123!";
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.ErrorMessage, Does.Contain("1 letra mayúscula"));
        }

        [Test]
        public void PasswordWithoutSpecialChar_FailsValidation()
        {
            // Arrange
            var password = "NoSpecialChar123";
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.ErrorMessage, Does.Contain("1 caracter especial"));
        }

        [Test]
        public void NullPassword_FailsValidation()
        {
            // Arrange
            string? password = null;
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.ErrorMessage, Does.Contain("requerida"));
        }

        [Test]
        public void ValidatePasswordStrength_ValidPassword_ReturnsTrue()
        {
            // Arrange
            var password = "ComplexPass123!";

            // Act
            var result = StrongPasswordAttribute.ValidatePasswordStrength(password);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(4, result.Requirements.Count);
            Assert.That(result.Requirements, Has.All.Matches<PasswordRequirement>(r => r.IsMet));
        }

        [Test]
        public void ValidatePasswordStrength_WeakPassword_ReturnsFalseWithDetails()
        {
            // Arrange
            var password = "weak";

            // Act
            var result = StrongPasswordAttribute.ValidatePasswordStrength(password);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(4, result.Requirements.Count);
            Assert.That(result.Requirements, Has.All.Matches<PasswordRequirement>(r => !r.IsMet));
        }

        [Test]
        [TestCase("MySecurePass123!")]
        [TestCase("C0mpl3x&Strong")]
        [TestCase("Password2025#Secure")]
        [TestCase("Admin@123456789")]
        public void MultipleValidPasswords_PassValidation(string password)
        {
            // Arrange
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.AreEqual(ValidationResult.Success, result);
        }

        [Test]
        [TestCase("short", "12 caracteres")]
        [TestCase("NoNumberHere!", "1 número")]
        [TestCase("no-uppercase-123", "1 letra mayúscula")]
        [TestCase("NoSpecialChar123", "1 caracter especial")]
        public void InvalidPasswords_FailWithSpecificError(string password, string expectedError)
        {
            // Arrange
            var attribute = new StrongPasswordAttribute();
            var context = new ValidationContext(new object()) { MemberName = "Password" };

            // Act
            var result = attribute.GetValidationResult(password, context);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result!.ErrorMessage, Does.Contain(expectedError));
        }
    }
}
