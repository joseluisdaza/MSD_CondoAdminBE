using NUnit.Framework;
using Condominio.Utils;
using System;

namespace Condominio.Tests.Utils
{
    [TestFixture]
    public class PasswordHasherTests
    {
        [Test]
        public void HashPassword_WithValidPassword_ReturnsHashedPassword()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hashedPassword = PasswordHasher.HashPassword(password);

            // Assert
            Assert.IsNotNull(hashedPassword);
            Assert.IsNotEmpty(hashedPassword);
            Assert.AreNotEqual(password, hashedPassword);
        }

        [Test]
        public void HashPassword_WithDifferentPasswords_ReturnsDifferentHashes()
        {
            // Arrange
            var password1 = "Password1";
            var password2 = "Password2";

            // Act
            var hash1 = PasswordHasher.HashPassword(password1);
            var hash2 = PasswordHasher.HashPassword(password2);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void HashPassword_WithSamePassword_ReturnsDifferentHashesEachTime()
        {
            // Arrange
            var password = "SamePassword";

            // Act
            var hash1 = PasswordHasher.HashPassword(password);
            var hash2 = PasswordHasher.HashPassword(password);

            // Assert
            Assert.AreNotEqual(hash1, hash2);
        }

        [Test]
        public void HashPassword_WithNullPassword_ThrowsArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(null));
        }

        [Test]
        public void HashPassword_WithEmptyPassword_ThrowsArgumentNullException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(""));
        }

        [Test]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "CorrectPassword123";
            var hashedPassword = PasswordHasher.HashPassword(password);

            // Act
            var result = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "CorrectPassword";
            var incorrectPassword = "IncorrectPassword";
            var hashedPassword = PasswordHasher.HashPassword(password);

            // Act
            var result = PasswordHasher.VerifyPassword(incorrectPassword, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithNullPassword_ReturnsFalse()
        {
            // Arrange
            var hashedPassword = PasswordHasher.HashPassword("ValidPassword");

            // Act
            var result = PasswordHasher.VerifyPassword(null, hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithEmptyPassword_ReturnsFalse()
        {
            // Arrange
            var hashedPassword = PasswordHasher.HashPassword("ValidPassword");

            // Act
            var result = PasswordHasher.VerifyPassword("", hashedPassword);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithNullHashedPassword_ReturnsFalse()
        {
            // Act
            var result = PasswordHasher.VerifyPassword("AnyPassword", null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithEmptyHashedPassword_ReturnsFalse()
        {
            // Act
            var result = PasswordHasher.VerifyPassword("AnyPassword", "");

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithBothNullParameters_ReturnsFalse()
        {
            // Act
            var result = PasswordHasher.VerifyPassword(null, null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithInvalidHash_ReturnsFalse()
        {
            // Arrange
            var password = "ValidPassword";
            var invalidHash = "InvalidHashString";

            // Act
            var result = PasswordHasher.VerifyPassword(password, invalidHash);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void VerifyPassword_WithMultiplePasswordVerifications_WorksCorrectly()
        {
            // Arrange
            var password1 = "Password1";
            var password2 = "Password2";
            var hash1 = PasswordHasher.HashPassword(password1);
            var hash2 = PasswordHasher.HashPassword(password2);

            // Act & Assert
            Assert.IsTrue(PasswordHasher.VerifyPassword(password1, hash1));
            Assert.IsTrue(PasswordHasher.VerifyPassword(password2, hash2));
            Assert.IsFalse(PasswordHasher.VerifyPassword(password1, hash2));
            Assert.IsFalse(PasswordHasher.VerifyPassword(password2, hash1));
        }

        [Test]
        public void HashPassword_WithSpecialCharacters_ReturnsValidHash()
        {
            // Arrange
            var password = "P@ssw0rd!#$%^&*()";

            // Act
            var hashedPassword = PasswordHasher.HashPassword(password);
            var verified = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(verified);
        }

        [Test]
        public void HashPassword_WithLongPassword_ReturnsValidHash()
        {
            // Arrange
            var password = new string('a', 200);

            // Act
            var hashedPassword = PasswordHasher.HashPassword(password);
            var verified = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(verified);
        }

        [Test]
        public void HashPassword_WithWhitespacePassword_ReturnsValidHash()
        {
            // Arrange
            var password = "   spaces   ";

            // Act
            var hashedPassword = PasswordHasher.HashPassword(password);
            var verified = PasswordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.IsTrue(verified);
        }
    }
}
