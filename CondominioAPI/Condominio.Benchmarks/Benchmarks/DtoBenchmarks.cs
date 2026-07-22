using Xunit;
using Condominio.DTOs;

namespace Condominio.Benchmarks.Tests;

/// <summary>
/// Unit tests para validar DTOs
/// </summary>
public class DtoBenchmarksTests
{
  private ExpenseRequest _validExpenseRequest = null!;
  private CreateExpensePaymentRequest _validPaymentRequest = null!;

  private void Setup()
  {
    // DTO válido
    _validExpenseRequest = new ExpenseRequest
    {
      Description = "Mantenimiento general",
      Amount = 1000.00m,
      CategoryId = 1,
      StartDate = DateTime.Now,
      PaymentLimitDate = DateTime.Now.AddDays(30),
      PropertyId = 1
    };

    // DTO de pago válido
    _validPaymentRequest = new CreateExpensePaymentRequest
    {
      ExpenseId = 1
    };
  }

  /// <summary>
  /// Test: Creación de DTO de gasto válido
  /// </summary>
  [Fact]
  public void CreateExpenseRequest_Valid_ShouldHaveCorrectValues()
  {
    Setup();
    var request = _validExpenseRequest;

    Assert.NotNull(request);
    Assert.Equal("Mantenimiento general", request.Description);
    Assert.Equal(1000.00m, request.Amount);
    Assert.Equal(1, request.CategoryId);
  }

  /// <summary>
  /// Test: DTO válido con propiedades requeridas
  /// </summary>
  [Fact]
  public void CreateExpenseRequest_Valid_ShouldNotBeNull()
  {
    var request = new ExpenseRequest
    {
      Description = "Test",
      Amount = 100m,
      CategoryId = 1,
      StartDate = DateTime.Now,
      PaymentLimitDate = DateTime.Now.AddDays(30)
    };

    Assert.NotNull(request);
    Assert.NotNull(request.Description);
  }

  /// <summary>
  /// Test: DTO con monto negativo debería ser permitido (validación en controller)
  /// </summary>
  [Fact]
  public void CreateExpenseRequest_NegativeAmount_ShouldBeLessThanZero()
  {
    var request = new ExpenseRequest
    {
      Description = "Test",
      Amount = -50m,
      CategoryId = 1,
      StartDate = DateTime.Now,
      PaymentLimitDate = DateTime.Now.AddDays(30)
    };

    Assert.True(request.Amount < 0);
  }

  /// <summary>
  /// Test: Creación de DTO de pago
  /// </summary>
  [Fact]
  public void CreatePaymentRequest_ShouldHaveCorrectValues()
  {
    Setup();
    var request = _validPaymentRequest;

    Assert.NotNull(request);
    Assert.Equal(1, request.ExpenseId);
  }

  /// <summary>
  /// Test: DTO de pago con ExpenseId positivo
  /// </summary>
  [Fact]
  public void CreatePaymentRequest_ExpenseId_ShouldBePositive()
  {
    Setup();
    var request = _validPaymentRequest;

    Assert.True(request.ExpenseId > 0);
  }

  /// <summary>
  /// Test: Múltiples DTOs de gasto
  /// </summary>
  [Fact]
  public void CreateMultipleExpenseRequests_ShouldCreateSuccessfully()
  {
    var requests = new List<ExpenseRequest>();

    for (int i = 1; i <= 5; i++)
    {
      requests.Add(new ExpenseRequest
      {
        Description = $"Expense {i}",
        Amount = i * 100m,
        CategoryId = i,
        StartDate = DateTime.Now.AddDays(-i),
        PaymentLimitDate = DateTime.Now.AddDays(30)
      });
    }

    Assert.Equal(5, requests.Count);
    Assert.All(requests, r => Assert.NotNull(r.Description));
  }

  /// <summary>
  /// Test: DTO con descripción vacía
  /// </summary>
  [Fact]
  public void CreateExpenseRequest_EmptyDescription_ShouldHaveEmptyString()
  {
    var request = new ExpenseRequest
    {
      Description = "",
      Amount = 100m,
      CategoryId = 1,
      StartDate = DateTime.Now,
      PaymentLimitDate = DateTime.Now.AddDays(30)
    };

    Assert.Empty(request.Description);
  }

  /// <summary>
  /// Test: DTO con categoría cero
  /// </summary>
  [Fact]
  public void CreateExpenseRequest_ZeroCategoryId_ShouldBeZero()
  {
    var request = new ExpenseRequest
    {
      Description = "Test",
      Amount = 100m,
      CategoryId = 0,
      StartDate = DateTime.Now,
      PaymentLimitDate = DateTime.Now.AddDays(30)
    };

    Assert.Equal(0, request.CategoryId);
  }

  /// <summary>
  /// Test: DTO con tasas de interés
  /// </summary>
  [Fact]
  public void CreateExpenseRequest_WithInterest_ShouldHaveInterestValues()
  {
    var request = new ExpenseRequest
    {
      Description = "Test",
      Amount = 100m,
      CategoryId = 1,
      StartDate = DateTime.Now,
      PaymentLimitDate = DateTime.Now.AddDays(30),
      InterestRate = 5.5m,
      InterestAmount = 5m
    };

    Assert.NotNull(request.InterestRate);
    Assert.Equal(5.5m, request.InterestRate);
    Assert.NotNull(request.InterestAmount);
    Assert.Equal(5m, request.InterestAmount);
  }
}
