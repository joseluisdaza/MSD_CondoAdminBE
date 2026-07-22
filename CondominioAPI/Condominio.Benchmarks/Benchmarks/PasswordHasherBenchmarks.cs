using Xunit;
using Condominio.Utils;

namespace Condominio.Benchmarks.Tests;

/// <summary>
/// Unit tests para el servicio de hashing de contraseñas usando BCrypt
/// </summary>
public class PasswordHasherBenchmarksTests
{
  private readonly string _password = "SecurePassword@123";
  private string _hashedPassword = null!;

  private void Setup()
  {
    // Generar un hash una vez para usar en tests de verificación
    _hashedPassword = PasswordHasher.HashPassword(_password);
  }

  /// <summary>
  /// Test: Hashing de contraseña
  /// </summary>
  [Fact]
  public void HashPassword_ShouldReturnValidHash()
  {
    var result = PasswordHasher.HashPassword(_password);

    Assert.NotEmpty(result);
    Assert.NotEqual(_password, result);
    Assert.True(result.Length > 10);
  }

  /// <summary>
  /// Test: Dos hashes con la misma contraseña deben ser diferentes (salt aleatorio)
  /// </summary>
  [Fact]
  public void HashPassword_SamePassword_ShouldGenerateDifferentHashes()
  {
    var hash1 = PasswordHasher.HashPassword(_password);
    var hash2 = PasswordHasher.HashPassword(_password);

    Assert.NotEqual(hash1, hash2);
  }

  /// <summary>
  /// Test: Verificación de contraseña correcta
  /// </summary>
  [Fact]
  public void VerifyPassword_Correct_ShouldReturnTrue()
  {
    Setup();
    var result = PasswordHasher.VerifyPassword(_password, _hashedPassword);

    Assert.True(result);
  }

  /// <summary>
  /// Test: Verificación de contraseña incorrecta
  /// </summary>
  [Fact]
  public void VerifyPassword_Incorrect_ShouldReturnFalse()
  {
    Setup();
    var result = PasswordHasher.VerifyPassword("WrongPassword@123", _hashedPassword);

    Assert.False(result);
  }

  /// <summary>
  /// Test: Verificación con hash vacío
  /// </summary>
  [Fact]
  public void VerifyPassword_EmptyHash_ShouldReturnFalse()
  {
    var result = PasswordHasher.VerifyPassword(_password, "");

    Assert.False(result);
  }

  /// <summary>
  /// Test: Verificación con contraseña vacía
  /// </summary>
  [Fact]
  public void VerifyPassword_EmptyPassword_ShouldReturnFalse()
  {
    Setup();
    var result = PasswordHasher.VerifyPassword("", _hashedPassword);

    Assert.False(result);
  }

  /// <summary>
  /// Test: Hash y verificación completo
  /// </summary>
  [Fact]
  public void HashAndVerify_CompleteFlow_ShouldSucceed()
  {
    var testPassword = "Test@12345";
    var hash = PasswordHasher.HashPassword(testPassword);
    var isValid = PasswordHasher.VerifyPassword(testPassword, hash);

    Assert.True(isValid);
  }

  /// <summary>
  /// Test: Contraseña con caracteres especiales
  /// </summary>
  [Fact]
  public void HashPassword_SpecialCharacters_ShouldHashCorrectly()
  {
    var complexPassword = "P@ssw0rd!#$%&*()";
    var hash = PasswordHasher.HashPassword(complexPassword);
    var isValid = PasswordHasher.VerifyPassword(complexPassword, hash);

    Assert.True(isValid);
  }

  /// <summary>
  /// Test: HashPassword no debe aceptar nulo
  /// </summary>
  [Fact]
  public void HashPassword_NullPassword_ShouldThrow()
  {
    Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(null!));
  }

  /// <summary>
  /// Test: HashPassword no debe aceptar vacío
  /// </summary>
  [Fact]
  public void HashPassword_EmptyPassword_ShouldThrow()
  {
    Assert.Throws<ArgumentNullException>(() => PasswordHasher.HashPassword(""));
  }
}
