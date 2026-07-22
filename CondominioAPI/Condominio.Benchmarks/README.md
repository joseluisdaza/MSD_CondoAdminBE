# 🚀 Condominio.Benchmarks

Proyecto de benchmarks para medir performance de componentes críticos del backend CondoAdmin usando **BenchmarkDotNet**.

## 📋 Descripción

Este proyecto contiene microbenchmarks para medir y optimizar la performance de:

- **PasswordHasherBenchmarks**: Hashing y validación de contraseñas con BCrypt
- **DtoBenchmarks**: Creación y manipulación de DTOs
- **LinqBenchmarks**: Operaciones LINQ comunes (filtrado, agrupación, ordenamiento)
- **DataStructureBenchmarks**: Comparación de rendimiento entre List, Dictionary, HashSet
- **JsonSerializationBenchmarks**: Serialización/deserialización JSON

## 🎯 Requisitos

- .NET 8.0 SDK
- BenchmarkDotNet 0.13.2+

## 🚀 Uso

### Ejecutar todos los benchmarks

```bash
cd CondominioAPI/Condominio.Benchmarks
dotnet run --configuration Release
```

### Ejecutar benchmark específico

```bash
# Solo benchmarks de contraseña
dotnet run --configuration Release -- --filter *PasswordHasher*

# Solo benchmarks de LINQ
dotnet run --configuration Release -- --filter *Linq*

# Solo benchmarks de estructuras de datos
dotnet run --configuration Release -- --filter *DataStructure*
```

### Opciones de ejecución útiles

```bash
# Listar todos los benchmarks disponibles
dotnet run --configuration Release -- --list

# Ejecutar con warmup custom
dotnet run --configuration Release -- -w 5 -i 10

# Generar reporte en memoria
dotnet run --configuration Release -- -m MemoryDiagnoser

# Exportar a múltiples formatos
dotnet run --configuration Release -- -e json,html,csv
```

## 📊 Interpretación de Resultados

### Métricas principales

| Métrica       | Descripción                        | Unidad      |
| ------------- | ---------------------------------- | ----------- |
| **Mean**      | Promedio de tiempo de ejecución    | ms/µs/ns    |
| **StdDev**    | Desviación estándar (variabilidad) | ms/µs/ns    |
| **Median**    | Valor central (50º percentil)      | ms/µs/ns    |
| **Min**       | Tiempo mínimo observado            | ms/µs/ns    |
| **Max**       | Tiempo máximo observado            | ms/µs/ns    |
| **Allocated** | Memoria asignada en el heap        | bytes/KB/MB |

### Ejemplo de lectura

```
|          Method |      Mean |    StdDev |   Median |      Min |      Max |   Allocated |
|---------------- |----------:|----------:|--------:|--------:|--------:|----------:|
| VerifyPassword_Correct | 62.14 ms | 10.23 ms | 59.87 ms | 51.23 ms | 79.45 ms | 5.12 KB |
```

**Interpretación**:

- El método tarda en promedio **62.14 ms** en ejecutarse
- La variabilidad es de **±10.23 ms** (puede haber outliers)
- La mediana es **59.87 ms** (50% más rápido, 50% más lento)
- El rango es **51.23 ms a 79.45 ms**
- Asigna aproximadamente **5.12 KB** de memoria

## 📈 Benchmarks Incluidos

### 1. PasswordHasherBenchmarks

Mide performance de operaciones criptográficas:

```csharp
[Benchmark] HashPassword_CostFactor10()      // Factor de costo 10 (por defecto)
[Benchmark] HashPassword_CostFactor12()      // Factor de costo 12 (más seguro)
[Benchmark] VerifyPassword_Correct()         // Verificación con contraseña correcta
[Benchmark] VerifyPassword_Incorrect()       // Verificación con contraseña incorrecta
[Benchmark] ValidateStrongPassword_Valid()   // Validación de contraseña fuerte
[Benchmark] ValidateStrongPassword_Weak()    // Validación de contraseña débil
```

**Casos de uso**:

- Identificar cuellos de botella en autenticación
- Comparar diferentes niveles de seguridad (cost factors)
- Optimizar tiempos de login

### 2. DtoBenchmarks

Mide rendimiento en creación y manipulación de DTOs:

```csharp
[Benchmark] CreateExpenseRequest_Valid()     // Crear DTO válido
[Benchmark] CreateExpenseRequest_Invalid()   // Crear DTO inválido
[Benchmark] CreatePaymentRequest()           // Crear DTO de pago
[Benchmark] CloneExpenseRequest()            // Clonar DTO
[Benchmark] CompareExpenseRequests()         // Comparar DTOs
```

**Casos de uso**:

- Validar que la creación de DTOs no introduce overhead
- Medir el costo de validación
- Optimizar transformaciones de objetos

### 3. LinqBenchmarks

Mide operaciones LINQ sobre colecciones (1000 items):

```csharp
[Benchmark] FilterByProperty()               // Where
[Benchmark] GroupByCategory()                // GroupBy
[Benchmark] OrderByDate()                    // OrderBy descending
[Benchmark] SumByProperty()                  // GroupBy con Sum
[Benchmark] FilterAndProject()               // Where + Select
[Benchmark] JoinExpensesAndPayments()        // Join
[Benchmark] MultipleAggregations()           // Agregaciones complejas
[Benchmark] SearchWithAny()                  // Any
[Benchmark] SearchWithFirstOrDefault()       // FirstOrDefault
[Benchmark] GetDistinctProperties()          // Distinct
[Benchmark] Pagination_Page1()               // Skip/Take primera página
[Benchmark] Pagination_Page50()              // Skip/Take página 50
```

**Casos de uso**:

- Identificar queries LINQ ineficientes
- Comparar diferentes enfoques de filtrado
- Optimizar paginación
- Medir impacto de joins

### 4. DataStructureBenchmarks

Compara performance de List vs Dictionary vs HashSet (10,000 items):

```csharp
[Benchmark] ListSearch_ById()                // List.FirstOrDefault
[Benchmark] DictionarySearch_ById()          // Dictionary.TryGetValue (O(1))
[Benchmark] HashSetContains()                // HashSet.Contains (O(1))
[Benchmark] ListContains()                   // List.Any
[Benchmark] DictionaryAdd()                  // Agregar a Dictionary
[Benchmark] ListAdd()                        // Agregar a List
[Benchmark] ListToDictionary()               // Conversión List→Dictionary
[Benchmark] ListToHashSet()                  // Conversión List→HashSet
[Benchmark] FilterByRole_List()              // Filtrar en List
[Benchmark] FilterByRole_Dictionary()        // Filtrar en Dictionary
```

**Casos de uso**:

- Elegir estructura de datos correcta para operaciones frecuentes
- Identificar cuando cachear en Dictionary es beneficioso
- Optimizar búsquedas

**Conclusión esperada**: Dictionary es mucho más rápido para búsquedas por ID.

### 5. JsonSerializationBenchmarks

Mide performance de serialización JSON:

```csharp
[Benchmark] SerializeExpense()               // Serializar 1 objeto
[Benchmark] DeserializeExpense()             // Deserializar 1 objeto
[Benchmark] SerializeExpenseList()           // Serializar 100 objetos
[Benchmark] DeserializeExpenseList()         // Deserializar 100 objetos
[Benchmark] RoundTrip()                      // Serializar + Deserializar
[Benchmark] SerializeExpense_Indented()      // Serializar con pretty-print
[Benchmark] SerializeExpense_WithOptions()   // Serializar con opciones custom
```

**Casos de uso**:

- Medir overhead de JSON en respuestas API
- Optimizar opciones de JsonSerializer
- Comparar performance de serialización

## 📊 Resultados Esperados

### Performance típica (en desarrollo)

```
PasswordHasher:
- Hash (CF=10): ~50-60 ms
- Verify: ~50-60 ms
- Validate: ~0.1 ms

LINQ (1000 items):
- Filter: ~0.1 ms
- GroupBy: ~0.3 ms
- Join: ~0.5 ms

Dictionary vs List (10,000 items):
- Dict lookup: ~0.001 ms
- List search: ~0.5 ms
- Dict is 500x más rápido!

JSON:
- Serialize 1 object: ~0.05 ms
- Serialize 100 objects: ~3 ms
- Deserialize 100 objects: ~5 ms
```

## 💡 Optimizaciones Comunes Basadas en Benchmarks

1. **Usar Dictionary para búsquedas frecuentes por ID**

   ```csharp
   // ❌ Lento
   var user = users.FirstOrDefault(u => u.Id == id);

   // ✅ Rápido
   var userDict = users.ToDictionary(u => u.Id);
   var user = userDict.TryGetValue(id, out var u) ? u : null;
   ```

2. **Cachear resultados agrupados**

   ```csharp
   // ❌ Recalcula cada vez
   var byRole = users.GroupBy(u => u.Role).ToDictionary(g => g.Key, g => g.ToList());

   // ✅ Cachear
   private static readonly Dictionary<string, List<User>> _usersByRole =
       _users.GroupBy(u => u.Role).ToDictionary(g => g.Key, g => g.ToList());
   ```

3. **Usar Select para proyectar solo campos necesarios**

   ```csharp
   // ❌ Carga todos los campos en memoria
   var expenses = db.Expenses.ToList();

   // ✅ Proyecta solo lo necesario
   var expenses = db.Expenses.Select(e => new { e.Id, e.Amount }).ToList();
   ```

## 🔍 Análisis Profundo

### Generar reportes detallados

```bash
# Exportar a JSON para análisis posterior
dotnet run --configuration Release -- -e json

# Los resultados están en: bin/Release/net8.0/BenchmarkDotNet.Artifacts/
```

### Comparar dos versiones

Ejecuta benchmarks en dos commits diferentes y compara:

```bash
git checkout main
dotnet run --configuration Release -- -e json -o main_results

git checkout feature-branch
dotnet run --configuration Release -- -e json -o feature_results

# Luego compara los JSONs en main_results vs feature_results
```

## ⚙️ Configuración

### Cambiar configuración de benchmarks

En cualquier clase benchmark:

```csharp
[SimpleJob(warmupCount: 5, targetCount: 10)]  // Más ejecuciones para más precisión
[MemoryDiagnoser]                              // Medir memoria asignada
[RPlotExporter]                                // Generar gráficos
public class MiBenchmark { }
```

### Opciones globales

```bash
# Más preciso pero más lento
-w 10 -i 20         # 10 warmups, 20 iterations

# Rápido pero menos preciso
-w 1 -i 3           # 1 warmup, 3 iterations
```

## 📝 Mejores Prácticas

1. ✅ **Ejecutar siempre en Release**: `--configuration Release`
2. ✅ **Calentar la JIT**: Los warmups son cruciales
3. ✅ **Aislar benchmarks**: Un benchmark por método crítico
4. ✅ **Usar datos realistas**: Tamaños similares a producción
5. ✅ **Repetir mediciones**: Varias iteraciones para variabilidad
6. ❌ **No confiar en resultados de Debug**
7. ❌ **No cambiar sistema durante ejecución**: Cierra otras aplicaciones

## 🔗 Enlaces Útiles

- [BenchmarkDotNet Docs](https://benchmarkdotnet.org/)
- [BenchmarkDotNet Best Practices](https://benchmarkdotnet.org/articles/overview.html)
- [ChooseTheBestAlgorithm Guide](https://benchmarkdotnet.org/articles/samples/IntroInlineVsVirtual.html)

## 🤝 Contribuir

Al agregar nuevos benchmarks:

1. Crear clase derivada de benchmarks existentes o nueva
2. Usar atributos: `[MemoryDiagnoser]`, `[SimpleJob(...)]`
3. Agregar setup si es necesario: `[GlobalSetup]`
4. Documentar con XML comments
5. Usar nombres descriptivos: `MethodName_Scenario()`
6. Ejecutar y verificar resultados

## 📄 Licencia

MIT

---

**Última actualización**: Julio 2025
