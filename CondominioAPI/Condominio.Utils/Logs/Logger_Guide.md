# ?? Logger - Guía de Uso

## Descripción
La clase `Logger` en `Condominio.Utils.Logs.Logger` es un wrapper de Serilog que proporciona funcionalidad de logging tanto en archivos (Serilog) como en base de datos (tabla `audit_logs`) para auditoría.

## Características

### ? Logging con Serilog
- Información general (`LogInformation`)
- Advertencias (`LogWarning`)
- Errores (`LogError`)
- Debug (`LogDebug`)
- Errores críticos (`LogFatal`)

### ? Auditoría en Base de Datos
- Logs automáticos en tabla `audit_logs`
- Seguimiento de acciones CRUD
- Logs de autenticación
- Logs de errores del sistema
- Logs de cambios de configuración

## Configuración

### 1. Registro en DI Container
```csharp
// En Program.cs
builder.Services.AddScoped<Condominio.Utils.Logs.Logger>();
```

### 2. Inyección en Controllers
```csharp
public class MyController : ControllerBase
{
    private readonly Logger _logger;
    
    public MyController(Logger logger)
    {
        _logger = logger;
    }
}
```

## Ejemplos de Uso

### 1. Logging Básico con Serilog
```csharp
// Información general
_logger.LogInformation("User {UserId} accessed resource {Resource}", userId, resource);

// Advertencias
_logger.LogWarning("Unusual activity detected for user {UserId}", userId);

// Errores
_logger.LogError(exception, "Error processing request for user {UserId}", userId);

// Debug (solo en desarrollo)
_logger.LogDebug("Processing step {Step} completed", stepNumber);
```

### 2. Auditoría Manual
```csharp
// Con ID de usuario específico
await _logger.LogAuditAsync(userId, "LOGIN", "User logged in successfully");

// Con ClaimsPrincipal (más común en controllers)
await _logger.LogAuditAsync(User, "CREATE_EXPENSE", 
    "Created expense with amount 500.00", "expenses");
```

### 3. Auditoría CRUD Automática
```csharp
// Crear registro
var expense = expenseRequest.ToExpense();
await _expenseRepository.AddAsync(expense);
await _logger.LogCreateAsync(userId, "expenses", expenseRequest, expense.Id);

// Actualizar registro
var oldData = new { existingExpense.Amount, existingExpense.Description };
// ... actualizar expense ...
var newData = new { existingExpense.Amount, existingExpense.Description };
await _logger.LogUpdateAsync(userId, "expenses", expenseId, oldData, newData);

// Eliminar registro
await _logger.LogDeleteAsync(userId, "expenses", expenseId, deletedExpense);
```

### 4. Auditoría de Autenticación
```csharp
// Login exitoso
await _logger.LogLoginAsync(userId, username, ipAddress);

// Logout
await _logger.LogLogoutAsync(userId, username);

// Login fallido
await _logger.LogFailedLoginAsync(username, ipAddress, "Invalid password");
```

### 5. Errores del Sistema
```csharp
try
{
    // ... código que puede fallar ...
}
catch (Exception ex)
{
    await _logger.LogSystemErrorAsync(ex, "ExpenseCreation", userId);
    return StatusCode(500, "Error interno del servidor");
}
```

### 6. Cambios de Configuración
```csharp
await _logger.LogConfigurationChangeAsync(userId, "MaxLoginAttempts", 3, 5);
```

### 7. Control de Acceso
```csharp
// Acceso denegado
await _logger.LogAccessDeniedAsync(userId, "expenses", "DELETE");

// Cambio de permisos
await _logger.LogPermissionChangeAsync(adminUserId, targetUserId, "CanDeleteExpenses", true);
```

## Estructura de la Tabla audit_logs

```sql
CREATE TABLE audit_logs (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    User_Id INT NOT NULL,
    Action VARCHAR(100) NOT NULL,
    Table_Name VARCHAR(100) NULL,
    Message TEXT NOT NULL,
    Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (User_Id) REFERENCES users(Id)
);
```

## Tipos de Acciones Comunes

### CRUD Operations
- `CREATE` - Crear registros
- `UPDATE` - Actualizar registros
- `DELETE` - Eliminar registros
- `READ` - Leer/consultar registros (opcional)

### Autenticación
- `LOGIN` - Login exitoso
- `LOGOUT` - Logout
- `FAILED_LOGIN` - Intento de login fallido

### Sistema
- `SYSTEM_ERROR` - Errores del sistema
- `CONFIG_CHANGE` - Cambios de configuración
- `ACCESS_DENIED` - Acceso denegado
- `PERMISSION_CHANGE` - Cambio de permisos

## Ejemplo Completo en Controller

```csharp
[HttpPost]
[Authorize(Roles = AppRoles.Administrador)]
public async Task<ActionResult<ExpenseResponse>> Create([FromBody] ExpenseRequest request)
{
    try
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        // Log información general
        _logger.LogInformation("Creating expense for user {UserId} with amount {Amount}", 
            userId, request.Amount);
        
        // Validaciones...
        if (!ModelState.IsValid)
        {
            await _logger.LogAuditAsync(userId, "CREATE_FAILED", 
                "Expense creation failed - Invalid model", "expenses");
            return BadRequest(ModelState);
        }
        
        // Crear entidad
        var expense = request.ToExpense();
        await _expenseRepository.AddAsync(expense);
        
        // Auditoría de creación
        await _logger.LogCreateAsync(userId, "expenses", request, expense.Id);
        
        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, 
            expense.ToExpenseResponse());
    }
    catch (Exception ex)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        // Log error completo
        _logger.LogError(ex, "Error creating expense for user {UserId}", userId);
        await _logger.LogSystemErrorAsync(ex, "ExpenseCreation", userId);
        
        return StatusCode(500, "Error interno del servidor");
    }
}
```

## Consultas de Auditoría

Usa el `AuditLogsController` para consultar los logs:

### Endpoints Disponibles
- `GET /api/auditlogs` - Todos los logs (Solo Super Admin)
- `GET /api/auditlogs/user/{userId}` - Logs por usuario
- `GET /api/auditlogs/action/{action}` - Logs por acción
- `GET /api/auditlogs/table/{tableName}` - Logs por tabla
- `GET /api/auditlogs/date-range?startDate={date}&endDate={date}` - Por fecha
- `GET /api/auditlogs/recent?count=100` - Logs recientes

## Mejores Prácticas

### ? DO
- Usar auditoría para todas las operaciones CRUD importantes
- Incluir datos relevantes en los mensajes de log
- Usar las funciones específicas (`LogCreateAsync`, `LogUpdateAsync`, etc.)
- Registrar intentos de acceso no autorizado
- Capturar errores del sistema

### ? DON'T
- No incluir información sensible (passwords, tokens) en logs
- No hacer logging excesivo que afecte performance
- No ignorar errores de logging (ya están manejados internamente)
- No usar logging síncrono en operaciones críticas

## Troubleshooting

### Error: "User ID not found"
- Asegúrate de que el usuario esté autenticado
- Verifica que el ClaimTypes.NameIdentifier esté en el token JWT

### Error: "Database context not available"
- Verifica que el Logger esté registrado correctamente en DI
- Asegúrate de que CondominioContext esté disponible

### Performance Issues
- Considera usar logging asíncrono
- Implementa batching para alta carga de logs
- Archiva logs antiguos regularmente