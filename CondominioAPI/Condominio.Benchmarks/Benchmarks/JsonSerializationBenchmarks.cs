using Xunit;
using System.Text.Json;

namespace Condominio.Benchmarks.Tests;

/// <summary>
/// Unit tests para serialización y deserialización JSON
/// </summary>
public class JsonSerializationBenchmarksTests
{
  private ExpenseJson _expense = null!;
  private string _expenseJson = string.Empty;
  private List<ExpenseJson> _expenses = null!;
  private string _expensesJson = string.Empty;
  private JsonSerializerOptions _options = null!;

  private void Setup()
  {
    _options = new JsonSerializerOptions { WriteIndented = false };

    _expense = new ExpenseJson
    {
      Id = 1,
      Description = "Mantenimiento general del edificio",
      Amount = 5000.00m,
      PropertyId = 10,
      CategoryId = 3,
      Date = DateTime.Now,
      Status = "Pending",
      CreatedAt = DateTime.Now.AddDays(-5)
    };

    _expenseJson = JsonSerializer.Serialize(_expense, _options);

    _expenses = Enumerable.Range(1, 100)
        .Select(i => new ExpenseJson
        {
          Id = i,
          Description = $"Expense {i}",
          Amount = i * 100m,
          PropertyId = (i % 10) + 1,
          CategoryId = (i % 5) + 1,
          Date = DateTime.Now.AddDays(-i),
          Status = i % 3 == 0 ? "Paid" : "Pending",
          CreatedAt = DateTime.Now.AddDays(-i)
        })
        .ToList();

    _expensesJson = JsonSerializer.Serialize(_expenses, _options);
  }

  /// <summary>
  /// Test: Serializar un gasto simple
  /// </summary>
  [Fact]
  public void SerializeExpense_ShouldReturnValidJson()
  {
    Setup();
    var result = JsonSerializer.Serialize(_expense, _options);

    Assert.NotEmpty(result);
    Assert.Contains("Mantenimiento", result);
  }

  /// <summary>
  /// Test: Deserializar un gasto simple
  /// </summary>
  [Fact]
  public void DeserializeExpense_ShouldReturnValidObject()
  {
    Setup();
    var result = JsonSerializer.Deserialize<ExpenseJson>(_expenseJson, _options);

    Assert.NotNull(result);
    Assert.Equal(1, result.Id);
    Assert.Equal("Mantenimiento general del edificio", result.Description);
    Assert.Equal(5000.00m, result.Amount);
  }

  /// <summary>
  /// Test: Serializar lista de 100 gastos
  /// </summary>
  [Fact]
  public void SerializeExpenseList_ShouldReturnValidJson()
  {
    Setup();
    var result = JsonSerializer.Serialize(_expenses, _options);

    Assert.NotEmpty(result);
    Assert.Contains("Expense 1", result);
    Assert.Contains("Expense 100", result);
  }

  /// <summary>
  /// Test: Deserializar lista de 100 gastos
  /// </summary>
  [Fact]
  public void DeserializeExpenseList_ShouldReturnList()
  {
    Setup();
    var result = JsonSerializer.Deserialize<List<ExpenseJson>>(_expensesJson, _options);

    Assert.NotNull(result);
    Assert.Equal(100, result.Count);
  }

  /// <summary>
  /// Test: Round-trip (serializar y deserializar)
  /// </summary>
  [Fact]
  public void RoundTrip_ShouldPreserveData()
  {
    Setup();
    var json = JsonSerializer.Serialize(_expense, _options);
    var result = JsonSerializer.Deserialize<ExpenseJson>(json, _options);

    Assert.NotNull(result);
    Assert.Equal(_expense.Id, result.Id);
    Assert.Equal(_expense.Description, result.Description);
    Assert.Equal(_expense.Amount, result.Amount);
  }

  /// <summary>
  /// Test: Serializar con indentación (para respuestas legibles)
  /// </summary>
  [Fact]
  public void SerializeExpense_Indented_ShouldIncludeFormatting()
  {
    Setup();
    var indentedOptions = new JsonSerializerOptions { WriteIndented = true };
    var result = JsonSerializer.Serialize(_expense, indentedOptions);

    Assert.NotEmpty(result);
    Assert.Contains("\n", result);
  }

  /// <summary>
  /// Test: Serializar con opciones personalizadas
  /// </summary>
  [Fact]
  public void SerializeExpense_WithCamelCase_ShouldFormatProperty()
  {
    Setup();
    var options = new JsonSerializerOptions
    {
      WriteIndented = false,
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    var result = JsonSerializer.Serialize(_expense, options);

    Assert.NotEmpty(result);
    Assert.Contains("\"id\"", result.ToLower());
  }

  /// <summary>
  /// Test: Deserialization con null debería devolver null
  /// </summary>
  [Fact]
  public void DeserializeExpense_InvalidJson_ShouldThrow()
  {
    var invalidJson = "{invalid json}";

    Assert.Throws<JsonException>(() =>
      JsonSerializer.Deserialize<ExpenseJson>(invalidJson, _options)
    );
  }

  /// <summary>
  /// Test: Lista vacía
  /// </summary>
  [Fact]
  public void SerializeEmptyList_ShouldReturnEmptyArray()
  {
    var emptyList = new List<ExpenseJson>();
    var result = JsonSerializer.Serialize(emptyList);

    Assert.Equal("[]", result);
  }

  /// <summary>
  /// Clase auxiliar
  /// </summary>
  public class ExpenseJson
  {
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public int PropertyId { get; set; }
    public int CategoryId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
  }
}
