using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Condominio.Benchmarks.Tests;

/// <summary>
/// Unit tests para operaciones LINQ comunes en el proyecto
/// </summary>
public class LinqBenchmarksTests
{
  private List<ExpenseData> _expenses = null!;
  private List<PaymentData> _payments = null!;
  private const int EXPENSE_COUNT = 1000;

  private void Setup()
  {
    // Generar datos de prueba
    _expenses = Enumerable.Range(1, EXPENSE_COUNT)
        .Select(i => new ExpenseData
        {
          Id = i,
          Amount = i * 100m,
          PropertyId = (i % 10) + 1,
          CategoryId = (i % 5) + 1,
          Date = DateTime.Now.AddDays(-i),
          Status = i % 3 == 0 ? "Paid" : i % 3 == 1 ? "Pending" : "PartiallyPaid"
        })
        .ToList();

    _payments = Enumerable.Range(1, EXPENSE_COUNT / 2)
        .Select(i => new PaymentData
        {
          Id = i,
          ExpenseId = i,
          Amount = i * 50m,
          PaymentDate = DateTime.Now.AddDays(-i)
        })
        .ToList();
  }

  /// <summary>
  /// Test: Filtrar gastos por propiedad
  /// </summary>
  [Fact]
  public void FilterByProperty_ShouldReturnOnlySelectedProperty()
  {
    Setup();
    var result = _expenses.Where(e => e.PropertyId == 5).ToList();

    Assert.NotEmpty(result);
    Assert.All(result, e => Assert.Equal(5, e.PropertyId));
  }

  /// <summary>
  /// Test: Agrupar gastos por categoría
  /// </summary>
  [Fact]
  public void GroupByCategory_ShouldCreateGroupsCorrectly()
  {
    Setup();
    var result = _expenses
        .GroupBy(e => e.CategoryId)
        .ToDictionary(g => g.Key, g => g.ToList());

    Assert.NotEmpty(result);
    Assert.True(result.Count <= 5);
  }

  /// <summary>
  /// Test: Ordenar gastos por fecha (descendente)
  /// </summary>
  [Fact]
  public void OrderByDate_ShouldBeSortedDescending()
  {
    Setup();
    var result = _expenses.OrderByDescending(e => e.Date).ToList();

    Assert.NotEmpty(result);
    Assert.True(result[0].Date >= result[result.Count - 1].Date);
  }

  /// <summary>
  /// Test: Calcular suma de gastos por propiedad
  /// </summary>
  [Fact]
  public void SumByProperty_ShouldCalculateTotals()
  {
    Setup();
    var result = _expenses
        .GroupBy(e => e.PropertyId)
        .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

    Assert.NotEmpty(result);
    Assert.All(result, kvp => Assert.True(kvp.Value > 0));
  }

  /// <summary>
  /// Test: Filtrar y proyectar (Select)
  /// </summary>
  [Fact]
  public void FilterAndProject_ShouldReturnProjections()
  {
    Setup();
    var result = _expenses
        .Where(e => e.Status == "Paid")
        .Select(e => new ExpenseProjection { Id = e.Id, Amount = e.Amount })
        .ToList();

    Assert.NotEmpty(result);
    Assert.All(result, r => Assert.True(r.Id > 0 && r.Amount > 0));
  }

  /// <summary>
  /// Test: Join entre gastos y pagos
  /// </summary>
  [Fact]
  public void JoinExpensesAndPayments_ShouldMergeCorrectly()
  {
    Setup();
    var result = _expenses
        .Join(_payments,
            e => e.Id,
            p => p.ExpenseId,
            (e, p) => new ExpensePaymentJoin
            {
              ExpenseId = e.Id,
              ExpenseAmount = e.Amount,
              PaymentAmount = p.Amount
            })
        .ToList();

    Assert.NotEmpty(result);
    Assert.All(result, r => Assert.True(r.ExpenseId > 0));
  }

  /// <summary>
  /// Test: GroupBy con agregaciones múltiples
  /// </summary>
  [Fact]
  public void MultipleAggregations_ShouldCalculateStats()
  {
    Setup();
    var result = _expenses
        .GroupBy(e => e.PropertyId)
        .Select(g => new ExpenseAggregation
        {
          PropertyId = g.Key,
          Count = g.Count(),
          Total = g.Sum(e => e.Amount),
          Average = (double)g.Average(e => e.Amount),
          Max = g.Max(e => e.Amount),
          Min = g.Min(e => e.Amount)
        })
        .ToList();

    Assert.NotEmpty(result);
    Assert.All(result, r => Assert.True(r.Count > 0 && r.Total > 0));
  }

  /// <summary>
  /// Test: Búsqueda con Any (existe)
  /// </summary>
  [Fact]
  public void SearchWithAny_ShouldFindMatches()
  {
    Setup();
    var result = _expenses.Any(e => e.PropertyId == 5 && e.Amount > 5000);

    Assert.True(result);
  }

  /// <summary>
  /// Test: Búsqueda con FirstOrDefault
  /// </summary>
  [Fact]
  public void SearchWithFirstOrDefault_ShouldReturnFirstMatch()
  {
    Setup();
    var result = _expenses.FirstOrDefault(e => e.PropertyId == 5 && e.Status == "Paid");

    Assert.NotNull(result);
    Assert.Equal(5, result.PropertyId);
    Assert.Equal("Paid", result.Status);
  }

  /// <summary>
  /// Test: Distintos valores
  /// </summary>
  [Fact]
  public void GetDistinctProperties_ShouldReturnUniqueValues()
  {
    Setup();
    var result = _expenses.Select(e => e.PropertyId).Distinct().ToList();

    Assert.NotEmpty(result);
    Assert.True(result.Count <= 10);
  }

  /// <summary>
  /// Test: Paginación con Skip/Take (página 1)
  /// </summary>
  [Fact]
  public void Pagination_Page1_ShouldReturn20Items()
  {
    Setup();
    var result = _expenses.OrderBy(e => e.Id).Skip(0).Take(20).ToList();

    Assert.Equal(20, result.Count);
    Assert.Equal(1, result.First().Id);
  }

  /// <summary>
  /// Test: Paginación con Skip/Take (página 50)
  /// </summary>
  [Fact]
  public void Pagination_Page50_ShouldReturnCorrectItems()
  {
    Setup();
    var result = _expenses.OrderBy(e => e.Id).Skip(980).Take(20).ToList();

    Assert.NotEmpty(result);
    Assert.True(result.All(e => e.Id > 980));
  }

  /// <summary>
  /// Test: Filtrar con predicado complejo
  /// </summary>
  [Fact]
  public void ComplexFilter_ShouldFilterCorrectly()
  {
    Setup();
    var result = _expenses
        .Where(e => e.PropertyId > 5 && e.Amount > 1000m && e.Status == "Pending")
        .ToList();

    Assert.NotEmpty(result);
    Assert.All(result, e => Assert.True(e.PropertyId > 5 && e.Amount > 1000m));
  }

  /// <summary>
  /// Test: Contar elementos con condición
  /// </summary>
  [Fact]
  public void CountWithCondition_ShouldReturnCorrectCount()
  {
    Setup();
    var result = _expenses.Count(e => e.Status == "Paid");

    Assert.True(result > 0);
  }

  // Clases auxiliares para tests
  public class ExpenseData
  {
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int PropertyId { get; set; }
    public int CategoryId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = null!;
  }

  public class PaymentData
  {
    public int Id { get; set; }
    public int ExpenseId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
  }

  public class ExpenseProjection
  {
    public int Id { get; set; }
    public decimal Amount { get; set; }
  }

  public class ExpensePaymentJoin
  {
    public int ExpenseId { get; set; }
    public decimal ExpenseAmount { get; set; }
    public decimal PaymentAmount { get; set; }
  }

  public class ExpenseAggregation
  {
    public int PropertyId { get; set; }
    public int Count { get; set; }
    public decimal Total { get; set; }
    public double Average { get; set; }
    public decimal Max { get; set; }
    public decimal Min { get; set; }
  }
}
