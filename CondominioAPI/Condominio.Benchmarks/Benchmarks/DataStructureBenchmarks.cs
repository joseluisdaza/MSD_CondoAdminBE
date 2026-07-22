using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Condominio.Benchmarks.Tests;

/// <summary>
/// Unit tests para validar el rendimiento y funcionamiento de diferentes estructuras de datos
/// </summary>
public class DataStructureBenchmarksTests
{
  private List<UserData> _userList = null!;
  private Dictionary<int, UserData> _userDictionary = null!;
  private HashSet<int> _userIdSet = null!;
  private const int USER_COUNT = 10000;

  private void Setup()
  {
    _userList = new List<UserData>();
    _userDictionary = new Dictionary<int, UserData>();
    _userIdSet = new HashSet<int>();

    for (int i = 1; i <= USER_COUNT; i++)
    {
      var user = new UserData
      {
        Id = i,
        Email = $"user{i}@condominio.com",
        Name = $"User {i}",
        Role = (i % 6) switch
        {
          0 => "Super",
          1 => "Admin",
          2 => "Director",
          3 => "Auxiliar",
          4 => "Habitante",
          _ => "Seguridad"
        }
      };

      _userList.Add(user);
      _userDictionary.Add(i, user);
      _userIdSet.Add(i);
    }
  }

  /// <summary>
  /// Test: Búsqueda en List con LINQ
  /// </summary>
  [Fact]
  public void ListSearch_ById_ShouldFindUser()
  {
    Setup();
    var result = _userList.FirstOrDefault(u => u.Id == 5000);

    Assert.NotNull(result);
    Assert.Equal(5000, result.Id);
    Assert.Equal("user5000@condominio.com", result.Email);
  }

  /// <summary>
  /// Test: Búsqueda en Dictionary (O(1))
  /// </summary>
  [Fact]
  public void DictionarySearch_ById_ShouldFindUserFast()
  {
    Setup();
    var found = _userDictionary.TryGetValue(5000, out var user);

    Assert.True(found);
    Assert.NotNull(user);
    Assert.Equal(5000, user.Id);
  }

  /// <summary>
  /// Test: Búsqueda en HashSet
  /// </summary>
  [Fact]
  public void HashSetContains_ShouldFindUserId()
  {
    Setup();
    var result = _userIdSet.Contains(5000);

    Assert.True(result);
  }

  /// <summary>
  /// Test: Búsqueda en List con Any
  /// </summary>
  [Fact]
  public void ListContains_ShouldFindUserId()
  {
    Setup();
    var result = _userList.Any(u => u.Id == 5000);

    Assert.True(result);
  }

  /// <summary>
  /// Test: Agregar a Dictionary
  /// </summary>
  [Fact]

  public void DictionaryAdd_ShouldInsert100Items()
  {
    Setup();
    var tempDict = new Dictionary<int, UserData>();
    for (int i = 1; i <= 100; i++)
    {
      tempDict.Add(i, _userDictionary[i]);
    }

    Assert.Equal(100, tempDict.Count);
  }

  /// <summary>
  /// Test: Búsqueda en elemento no existente
  /// </summary>
  [Fact]
  public void ListSearch_NonExistent_ShouldReturnNull()
  {
    Setup();
    var result = _userList.FirstOrDefault(u => u.Id == 99999);

    Assert.Null(result);
  }

  /// <summary>
  /// Test: Dictionary TryGetValue en clave inexistente
  /// </summary>
  [Fact]
  public void DictionarySearch_NonExistent_ShouldReturnFalse()
  {
    Setup();
    var found = _userDictionary.TryGetValue(99999, out var user);

    Assert.False(found);
    Assert.Null(user);
  }

  /// <summary>
  /// Test: HashSet con elementos no existentes
  /// </summary>
  [Fact]
  public void HashSetContains_NonExistent_ShouldReturnFalse()
  {
    Setup();
    var result = _userIdSet.Contains(99999);

    Assert.False(result);
  }

  /// <summary>
  /// Clase auxiliar para datos de usuario
  /// </summary>
  private class UserData
  {
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;
  }
}
