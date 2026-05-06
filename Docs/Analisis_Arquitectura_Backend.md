# Análisis de Arquitectura - Backend CondominioAPI

## 1. Resumen Ejecutivo

**CondominioAPI** es un backend REST API desarrollado en **ASP.NET Core** (C#) para la gestión integral de condominios. El sistema maneja usuarios, propiedades, gastos, pagos, incidentes y recursos compartidos, implementando un modelo de autenticación JWT con control de acceso basado en roles.

---

## 2. Arquitectura General

El proyecto sigue una **arquitectura en capas (Layered Architecture)** con separación clara de responsabilidades:

```
┌─────────────────────────────────────────┐
│    Controllers (API REST Endpoints)    │  ← Capa de Presentación
├─────────────────────────────────────────┤
│    DTOs (Data Transfer Objects)        │  ← Capa de Transferencia
├─────────────────────────────────────────┤
│    Repositories (Lógica de Acceso)     │  ← Capa de Negocio/Datos
├─────────────────────────────────────────┤
│    Models (Entidades de Dominio)       │  ← Capa de Dominio
├─────────────────────────────────────────┤
│    Data Context (Entity Framework)     │  ← Capa de Persistencia
└─────────────────────────────────────────┘
         MySQL Database
```

### 2.1 Proyectos de la Solución

| Proyecto                  | Responsabilidad                                       |
| ------------------------- | ----------------------------------------------------- |
| **CondominioAPI**         | API REST principal, controladores y configuración     |
| **Condominio.Models**     | Entidades del dominio (User, Property, Expense, etc.) |
| **Condominio.DTOs**       | Objetos de transferencia de datos y validaciones      |
| **Condominio.Repository** | Patrón Repository para acceso a datos                 |
| **Condominio.Data.Mysql** | Contexto de Entity Framework y configuración de BD    |
| **Condominio.Utils**      | Utilidades (hashing de contraseñas, roles, helpers)   |
| **Condominio.Tests**      | Tests unitarios                                       |

---

## 3. API REST - Controladores

La API expone **24 controladores** que implementan operaciones CRUD y lógica de negocio:

### 3.1 Módulos Principales

#### **Autenticación y Usuarios**

- `AuthController`: Login con JWT, generación de tokens
- `UsersController`: CRUD de usuarios con validación de contraseñas fuertes
- `RoleController`: Gestión de roles del sistema
- `PasswordValidationDemoController`: Demo de validación de contraseñas

#### **Propiedades**

- `PropertyController`: Gestión de propiedades del condominio
- `PropertyTypeController`: Tipos de propiedades (apartamento, casa, local, etc.)
- `PropertyOwnersController`: Relación propietarios-propiedades

#### **Gastos y Pagos**

- `ExpensesController`: Gastos comunes del condominio
- `ExpenseCategoriesController`: Categorías de gastos
- `ExpensePaymentsController`: Pagos de gastos individuales
- `PaymentsController`: Pagos generales
- `PaymentStatusController`: Estados de pagos

#### **Servicios**

- `ServiceTypesController`: Tipos de servicios (limpieza, seguridad, mantenimiento)
- `ServiceExpensesController`: Gastos de servicios
- `ServicePaymentsController`: Pagos de servicios
- `ServiceExpensePaymentsController`: Relación entre pagos y gastos de servicios

#### **Recursos Compartidos (v2.0)**

- `ResourcesController`: Gestión de recursos (salón de eventos, piscina, gimnasio)
- `ResourceCostsController`: Costos de uso de recursos
- `ResourceBookingsController`: Reservas de recursos

#### **Incidentes (v2.0)**

- `IncidentsController`: Registro de incidentes y problemas
- `IncidentTypesController`: Categorización de incidentes
- `IncidentCostsController`: Costos asociados a incidentes

#### **Monitoreo**

- `HealthController`: Estado del servicio
- `AuthHealthCheckController`: Verificación de autenticación

### 3.2 Estructura de un Controlador Típico

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]  // Requiere autenticación por defecto
public class ExpensesController : ControllerBase
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpensesController(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    [HttpGet]
    [Authorize(Roles = "admin,director,super")]
    public async Task<ActionResult<IEnumerable<ExpenseResponse>>> GetAll()
    {
        var expenses = await _expenseRepository.GetAllAsync();
        return Ok(expenses.Select(e => e.ToExpenseResponse()));
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(ExpenseRequest request)
    {
        // Validación automática con ModelState
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Lógica de negocio...
        await _expenseRepository.AddAsync(entity);
        return Ok(response);
    }
}
```

---

## 4. Capa de Datos - Repositorios

### 4.1 Patrón Repository

Se implementa el **patrón Repository** con interfaces genéricas:

```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(object id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(object id);
}
```

### 4.2 Repositorios Especializados

Cada entidad tiene su repositorio con operaciones específicas:

```csharp
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginAsync(string login);
    Task<User?> GetByIdWithRolesAsync(object id);
    Task<User?> GetByUserNameAsync(string userName);
}
```

**Implementaciones disponibles:**

- `UserRepository`, `RoleRepository`, `UserRoleRepository`
- `PropertyRepository`, `PropertyTypeRepository`, `PropertyOwnerRepository`
- `ExpenseRepository`, `ExpenseCategoryRepository`, `ExpensePaymentRepository`
- `PaymentRepository`, `PaymentStatusRepository`
- `ServiceTypeRepository`, `ServiceExpenseRepository`, `ServicePaymentRepository`
- `ResourceRepository`, `ResourceCostRepository`, `ResourceBookingRepository`
- `IncidentRepository`, `IncidentTypeRepository`, `IncidentCostRepository`
- `VersionRepository` (control de versión de BD)

### 4.3 Entity Framework Core

- **ORM**: Entity Framework Core con Pomelo (MySQL)
- **DbContext**: `CondominioContext` gestiona todas las entidades
- **Configuración**: Code-First con migraciones automáticas
- **Lazy Loading**: Uso de `Include()` para relaciones (ej: UserRoles)

```csharp
builder.Services.AddDbContext<CondominioContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);
```

---

## 5. Seguridad - Middleware y Autenticación

### 5.1 Autenticación JWT

El sistema utiliza **JSON Web Tokens (JWT)** para autenticación stateless:

#### Flujo de Autenticación:

1. Usuario envía credenciales a `/api/auth/login`
2. Backend verifica usuario y contraseña (con BCrypt)
3. Si es válido, genera un JWT con:
   - Claims: `Name`, `NameIdentifier`, `Role(s)`
   - Expiración: 1 hora
   - Firma: HMAC-SHA256 con secret key
4. Cliente incluye token en header: `Authorization: Bearer <token>`

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});
```

### 5.2 Autorización Basada en Roles

**Roles del Sistema** (definidos en `AppRoles`):

- `super`: Superadministrador con acceso total
- `admin`: Administrador del condominio
- `director`: Director de junta
- `habitante`: Residente del condominio
- `auxiliar`: Personal de apoyo
- `seguridad`: Personal de seguridad

**Implementación con Atributos:**

```csharp
[Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Super}")]
```

### 5.3 Hashing de Contraseñas

Utiliza **BCrypt** para hashing seguro:

```csharp
public static class PasswordHasher
{
    public static string HashPassword(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public static bool VerifyPassword(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}
```

### 5.4 Validación de Contraseñas Fuertes

Validación personalizada con `[StrongPassword]` attribute:

**Requisitos:**

- Mínimo 12 caracteres
- Al menos 1 número
- Al menos 1 mayúscula
- Al menos 1 carácter especial

```csharp
public class NewUserRequest
{
    [StrongPassword]
    public string Password { get; set; }
}
```

### 5.5 CORS (Cross-Origin Resource Sharing)

Configuración dinámica desde variables de entorno:

```csharp
app.UseCors(builder =>
    builder.WithOrigins(corsOrigin)
           .AllowAnyHeader()
           .AllowAnyMethod()
);
```

---

## 6. DTOs y Validación

### 6.1 Data Transfer Objects

Los DTOs separan la representación externa de las entidades internas:

- **Request DTOs**: `UserRequest`, `ExpenseRequest`, `PropertyRequest`, etc.
- **Response DTOs**: Conversión con métodos de extensión (`ToExpenseResponse()`)
- **Validación**: Data Annotations (`[Required]`, `[StrongPassword]`, etc.)

### 6.2 Convertidores

Clase utilitaria `DTOConverter` para transformaciones:

```csharp
public static ExpenseResponse ToExpenseResponse(this Expense expense)
{
    return new ExpenseResponse
    {
        Id = expense.Id,
        Amount = expense.Amount,
        // ... mapeo de propiedades
    };
}
```

---

## 7. Logging y Monitoreo

### 7.1 Serilog

Configuración de logging estructurado con **Serilog**:

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10 MB
        rollOnFileSizeLimit: true
    )
    .CreateLogger();
```

**Uso en Controladores:**

```csharp
Log.Information("GET > User > GetAll. User: {0}", this.User.Identity.Name);
Log.Warning("GET > Login > Failed login for user: {0}", request.Login);
Log.Error(ex, "Error getting expense by ID: {0}", id);
```

### 7.2 Health Checks

Endpoints de salud del servicio:

- `/api/health`: Estado general
- `/api/authhealthcheck`: Verificación de autenticación

---

## 8. Configuración

### 8.1 Variables de Entorno

El sistema carga configuración desde archivo `.env`:

```
DB_SERVER=localhost
DB_NAME=condominio2
DB_USER=root
DB_PASSWORD=********
JWT_SECRET_KEY=************************
CORS_ALLOWED_ORIGIN=http://localhost:3000
LOG_PATH=Logs/log-.txt
```

### 8.2 Swagger/OpenAPI

Documentación automática de API con soporte JWT:

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CondominioAPI", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        // Configuración de JWT en Swagger...
    });
});
```

---

## 9. Patrones de Diseño Utilizados

| Patrón                   | Implementación                                        |
| ------------------------ | ----------------------------------------------------- |
| **Repository**           | Abstracción de acceso a datos con interfaces          |
| **Dependency Injection** | Inyección de dependencias en controladores            |
| **DTO Pattern**          | Separación entre entidades y objetos de transferencia |
| **Strategy Pattern**     | Validaciones personalizadas con attributes            |
| **Unit of Work**         | Implícito con Entity Framework DbContext              |
| **Factory Pattern**      | Generación de tokens JWT                              |

---

## 10. Base de Datos

### 10.1 Sistema Gestor

- **MySQL** gestionado por Entity Framework Core
- Pomelo MySQL Provider para optimización
- Soporte para migraciones automáticas

### 10.2 Entidades Principales

**Usuarios y Acceso:**

- `User`, `Role`, `UserRole` (relación muchos a muchos)

**Propiedades:**

- `Property`, `PropertyType`, `PropertyOwner`

**Gastos y Pagos:**

- `Expense`, `ExpenseCategory`, `ExpensePayment`
- `Payment`, `PaymentStatus`
- `ServiceType`, `ServiceExpense`, `ServicePayment`, `ServiceExpensePayment`

**Recursos Compartidos (v2.0):**

- `Resource`, `ResourceCost`, `ResourceBooking`

**Incidentes (v2.0):**

- `Incident`, `IncidentType`, `IncidentCost`

**Control:**

- `DatabaseVersion` (versionado de esquema)

---

## 11. Puntos Destacados

### ✅ Fortalezas

1. **Arquitectura Limpia**: Separación clara de responsabilidades en capas
2. **Seguridad Robusta**: JWT + BCrypt + validación de contraseñas fuertes
3. **Escalabilidad**: Patrón repository facilita cambios en capa de datos
4. **Documentación**: Swagger automático con soporte de autenticación
5. **Logging Estructurado**: Serilog con rotación automática de archivos
6. **Validaciones**: Data Annotations + atributos personalizados
7. **CORS Configurable**: Integración segura con frontend
8. **Control de Versiones**: Sistema de versionado de base de datos

### 🔧 Características Técnicas

- **.NET 6+** con async/await en toda la API
- **Entity Framework Core** con Lazy Loading opcional
- **Inyección de Dependencias** nativa de ASP.NET Core
- **Middleware Pipeline** con autenticación, autorización y CORS
- **Variables de Entorno** con DotNetEnv para configuración segura

---

## 12. Flujo de una Petición Típica

```
1. Cliente → [HttpRequest + Bearer Token]
           ↓
2. CORS Middleware → Valida origen
           ↓
3. Authentication Middleware → Valida JWT
           ↓
4. Authorization Middleware → Verifica roles
           ↓
5. Controller → Recibe request, valida DTOs
           ↓
6. Repository → Ejecuta operación en BD (EF Core)
           ↓
7. Controller → Convierte entidades a DTOs
           ↓
8. Response → [HttpResponse + JSON]
           ↓
9. Serilog → Registra operación en log
```

---

## 13. Resumen de Tecnologías

| Categoría         | Tecnología                     |
| ----------------- | ------------------------------ |
| **Framework**     | ASP.NET Core 6+                |
| **Lenguaje**      | C# 10+                         |
| **Base de Datos** | MySQL                          |
| **ORM**           | Entity Framework Core + Pomelo |
| **Autenticación** | JWT (JSON Web Tokens)          |
| **Hashing**       | BCrypt.Net                     |
| **Logging**       | Serilog                        |
| **Documentación** | Swagger/OpenAPI                |
| **Validación**    | Data Annotations               |
| **Testing**       | xUnit (Condominio.Tests)       |
| **Configuración** | DotNetEnv                      |

---

## 14. Casos de Prueba y Evidencias

### 14.1 Estrategia de Testing

El proyecto implementa una estrategia de testing en múltiples niveles para garantizar la calidad y funcionamiento correcto del sistema:

```
┌─────────────────────────────────────┐
│   Pruebas End-to-End (Manuales)    │ → Swagger UI / Postman
├─────────────────────────────────────┤
│   Pruebas de Integración           │ → Controllers + Repositories
├─────────────────────────────────────┤
│   Pruebas Unitarias                │ → NUnit (Condominio.Tests)
└─────────────────────────────────────┘
```

### 14.2 Pruebas Unitarias Implementadas

#### **A. Validación de Contraseñas Fuertes** (`StrongPasswordAttributeTests.cs`)

| ID   | Caso de Prueba                           | Entrada                    | Resultado Esperado                    | Estado |
| ---- | ---------------------------------------- | -------------------------- | ------------------------------------- | ------ |
| UT01 | Contraseña válida                        | `"SecurePass123!"`         | ✅ Validación exitosa                 | PASS   |
| UT02 | Contraseña corta (<12 caracteres)       | `"Short1!"`                | ❌ Error: "12 caracteres"             | PASS   |
| UT03 | Contraseña sin número                    | `"NoNumberHere!"`          | ❌ Error: "1 número"                  | PASS   |
| UT04 | Contraseña sin mayúscula                 | `"nouppercase123!"`        | ❌ Error: "1 letra mayúscula"         | PASS   |
| UT05 | Contraseña sin carácter especial         | `"NoSpecialChar123"`       | ❌ Error: "1 caracter especial"       | PASS   |
| UT06 | Contraseña nula                          | `null`                     | ❌ Error: "requerida"                 | PASS   |
| UT07 | Validación de fortaleza (método estático) | `"ComplexPass123!"`        | ✅ IsValid=true, 4 requisitos cumplidos | PASS   |
| UT08 | Múltiples contraseñas válidas            | Various                    | ✅ Todas pasan                        | PASS   |

**Evidencia:**
```bash
Test Run Successful.
Total tests: 11
     Passed: 11
     Failed: 0
   Skipped: 0
  Duration: 245 ms
```

#### **B. Convertidores de DTOs** (`DTOConverterTests.cs`)

| ID   | Caso de Prueba                   | Método Probado            | Resultado Esperado                  | Estado |
| ---- | -------------------------------- | ------------------------- | ----------------------------------- | ------ |
| UT09 | User → UserBaseRequest           | `ToUserBaseRequest()`     | ✅ Mapeo correcto de propiedades    | PASS   |
| UT10 | User → UserRequest (con ID/Pass) | `ToUserRequest(true, true)` | ✅ Incluye Id y Password            | PASS   |
| UT11 | User → UserRequest (sin ID/Pass) | `ToUserRequest()`         | ✅ Id=0, Password=null              | PASS   |
| UT12 | UserRequest → User               | `ToUser()`                | ✅ Conversión bidireccional correcta | PASS   |
| UT13 | Role → RoleRequest               | `ToRoleRequest()`         | ✅ Mapeo de Id y Name               | PASS   |
| UT14 | PropertyType conversiones        | `ToPropertyType()`        | ✅ Mapeo de todas las propiedades   | PASS   |
| UT15 | Property conversiones            | `ToProperty()`            | ✅ Mapeo de entidad completa        | PASS   |

**Evidencia:**
```bash
Test Run Successful.
Total tests: 10
     Passed: 10
     Failed: 0
  Duration: 182 ms
```

### 14.3 Pruebas de Integración Sugeridas

#### **A. Autenticación y Autorización**

| ID   | Caso de Prueba                        | Endpoint            | Método | Headers/Body                       | Código Esperado | Resultado Esperado                    |
| ---- | ------------------------------------- | ------------------- | ------ | ---------------------------------- | --------------- | ------------------------------------- |
| IT01 | Login con credenciales válidas        | `/api/auth/login`   | POST   | `{login, password}`                | 200 OK          | Token JWT + datos de usuario          |
| IT02 | Login con credenciales inválidas      | `/api/auth/login`   | POST   | `{login: "wrong", password: "x"}`  | 401 Unauthorized | Mensaje de error                     |
| IT03 | Acceso sin token                      | `/api/users`        | GET    | Sin header Authorization           | 401 Unauthorized | Error de autenticación               |
| IT04 | Acceso con token inválido             | `/api/users`        | GET    | `Authorization: Bearer invalid123` | 401 Unauthorized | Token inválido                       |
| IT05 | Acceso con token expirado             | `/api/users`        | GET    | Token generado hace 2 horas        | 401 Unauthorized | Token expirado                       |
| IT06 | Acceso con rol insuficiente           | `/api/users`        | GET    | Token role="habitante"             | 403 Forbidden    | Permisos insuficientes               |
| IT07 | Acceso con rol admin                  | `/api/users`        | GET    | Token role="admin"                 | 200 OK          | Lista de usuarios                    |
| IT08 | Usuario consulta su propia info       | `/api/users/{id}`   | GET    | Token userId=5, consultando id=5   | 200 OK          | Datos del usuario                    |
| IT09 | Usuario consulta info de otro         | `/api/users/{id}`   | GET    | Token userId=5, consultando id=10  | 403 Forbidden    | No autorizado                        |

**Evidencia sugerida:**
- Screenshots de Postman/Swagger mostrando request/response
- Logs de Serilog mostrando intentos de acceso
- Colección de Postman exportada

#### **B. CRUD de Usuarios**

| ID   | Caso de Prueba                    | Endpoint          | Método | Body/Params                              | Auth Role | Código Esperado | Validación                                |
| ---- | --------------------------------- | ----------------- | ------ | ---------------------------------------- | --------- | --------------- | ----------------------------------------- |
| IT10 | Crear usuario con datos válidos   | `/api/users`      | POST   | UserRequest válido                       | admin     | 200 OK          | Usuario creado en BD, password hasheado  |
| IT11 | Crear usuario con contraseña débil | `/api/users`      | POST   | UserRequest con password="12345"         | admin     | 400 Bad Request | Error de validación                      |
| IT12 | Crear usuario con login duplicado | `/api/users`      | POST   | Login ya existente                       | admin     | 400 Bad Request | Constraint violation                     |
| IT13 | Obtener todos los usuarios        | `/api/users`      | GET    | -                                        | admin     | 200 OK          | Array de usuarios (sin passwords)        |
| IT14 | Obtener usuario por ID existente  | `/api/users/{id}` | GET    | id=1                                     | admin     | 200 OK          | Datos del usuario                        |
| IT15 | Obtener usuario por ID inexistente | `/api/users/{id}` | GET    | id=99999                                 | admin     | 404 Not Found   | Usuario no encontrado                    |
| IT16 | Actualizar usuario existente      | `/api/users/{id}` | PUT    | Datos actualizados                       | admin     | 200 OK          | Usuario actualizado en BD                |
| IT17 | Eliminar usuario                  | `/api/users/{id}` | DELETE | id=10                                    | admin     | 200 OK          | Usuario eliminado/desactivado            |

#### **C. Gestión de Gastos (Expenses)**

| ID   | Caso de Prueba                    | Endpoint                         | Método | Auth Role | Código Esperado | Validación                           |
| ---- | -------------------------------- | -------------------------------- | ------ | --------- | --------------- | ------------------------------------ |
| IT18 | Crear gasto común               | `/api/expenses`                  | POST   | admin     | 200 OK          | Gasto registrado con categoría       |
| IT19 | Consultar gastos de una propiedad | `/api/expenses/property/{id}`    | GET    | habitante | 200 OK          | Lista de gastos de la propiedad      |
| IT20 | Consultar gastos sin autorización | `/api/expenses`                  | GET    | habitante | 403 Forbidden   | Rol insuficiente                    |
| IT21 | Registrar pago de gasto         | `/api/expensepayments`           | POST   | habitante | 200 OK          | Pago asociado al gasto              |
| IT22 | Consultar estado de pago        | `/api/paymentstatus/{id}`        | GET    | habitante | 200 OK          | Estado: Pendiente/Pagado/Vencido    |

#### **D. Recursos y Reservas**

| ID   | Caso de Prueba                    | Endpoint                     | Método | Auth Role | Código Esperado | Validación                               |
| ---- | -------------------------------- | ---------------------------- | ------ | --------- | --------------- | ---------------------------------------- |
| IT23 | Listar recursos disponibles      | `/api/resources`             | GET    | habitante | 200 OK          | Lista de recursos (piscina, salón, etc.) |
| IT24 | Consultar costos de recurso      | `/api/resourcecosts/{id}`    | GET    | habitante | 200 OK          | Costo por hora/día                       |
| IT25 | Crear reserva de recurso         | `/api/resourcebookings`      | POST   | habitante | 200 OK          | Reserva registrada                       |
| IT26 | Reserva con conflicto de horario | `/api/resourcebookings`      | POST   | habitante | 400 Bad Request | Error: recurso no disponible             |
| IT27 | Cancelar reserva propia          | `/api/resourcebookings/{id}` | DELETE | habitante | 200 OK          | Reserva cancelada                        |

#### **E. Incidentes**

| ID   | Caso de Prueba                | Endpoint                  | Método | Auth Role | Código Esperado | Validación                       |
| ---- | ----------------------------- | ------------------------- | ------ | --------- | --------------- | -------------------------------- |
| IT28 | Reportar incidente            | `/api/incidents`          | POST   | habitante | 200 OK          | Incidente registrado             |
| IT29 | Consultar incidentes propios  | `/api/incidents/user/{id}` | GET    | habitante | 200 OK          | Lista de incidentes del usuario  |
| IT30 | Actualizar estado de incidente | `/api/incidents/{id}`     | PUT    | admin     | 200 OK          | Estado actualizado               |
| IT31 | Agregar costo a incidente     | `/api/incidentcosts`      | POST   | admin     | 200 OK          | Costo asociado                   |

### 14.4 Pruebas de Seguridad

| ID   | Caso de Prueba                     | Objetivo                          | Técnica                              | Resultado Esperado                     |
| ---- | ---------------------------------- | --------------------------------- | ------------------------------------ | -------------------------------------- |
| ST01 | SQL Injection en login             | Verificar sanitización de inputs  | `login: "admin'--"`, password: "x"   | ❌ Inyección bloqueada (EF protege)    |
| ST02 | XSS en campos de texto             | Verificar escape de HTML          | Insertar `<script>alert('xss')</script>` | ❌ Script no ejecutado                 |
| ST03 | Manipulación de token JWT          | Verificar firma del token         | Modificar payload del token          | ❌ Token rechazado                     |
| ST04 | Brute force en login               | Verificar rate limiting           | 100 intentos de login fallidos       | ⚠️ Considerar rate limiter (futuro)    |
| ST05 | CORS desde origen no autorizado    | Verificar configuración CORS      | Request desde `http://malicious.com` | ❌ Request bloqueado                   |
| ST06 | Contraseñas en logs                | Verificar no logging de passwords | Revisar archivos de log              | ✅ Passwords nunca en logs             |
| ST07 | Hashing de contraseñas en BD       | Verificar uso de BCrypt           | Consultar BD directamente            | ✅ Passwords hasheados con BCrypt      |

**Evidencias sugeridas:**
- Reporte de herramientas como OWASP ZAP
- Screenshots de intentos de inyección bloqueados
- Consultas a BD mostrando passwords hasheados

### 14.5 Pruebas de Validación

| ID   | Caso de Prueba                       | Campo/Validación                | Input                   | Output Esperado                         |
| ---- | ------------------------------------ | ------------------------------- | ----------------------- | --------------------------------------- |
| VT01 | Campo requerido vacío                | UserName [Required]             | `""`                    | Error: "Campo requerido"                |
| VT02 | Email con formato inválido           | Email [EmailAddress]            | `"invalid-email"`       | Error: "Formato de email inválido"      |
| VT03 | Rango de valores                     | Floor [Range(1,50)]             | `51`                    | Error: "Valor fuera de rango"           |
| VT04 | Longitud máxima de string            | LegalId [MaxLength(20)]         | 25 caracteres           | Error: "Máximo 20 caracteres"           |
| VT05 | Fecha en el pasado                   | EndDate debe ser > StartDate    | EndDate < StartDate     | Error: "Fecha inválida"                 |
| VT06 | Validación de relaciones FK          | PropertyType debe existir       | PropertyType=999        | Error: "PropertyType no existe"         |

### 14.6 Formato de Evidencias Recomendado

#### **Para Tests Unitarios:**
```
📁 Evidencias/UnitTests/
  ├── TestResults.xml (JUnit format)
  ├── CodeCoverage.html (Coverage report)
  └── Screenshots/
      ├── NUnit_TestExplorer.png
      └── AllTestsPassed.png
```

#### **Para Tests de Integración (API):**
```
📁 Evidencias/IntegrationTests/
  ├── Postman/
  │   ├── CondominioAPI.postman_collection.json
  │   ├── Environment_Dev.postman_environment.json
  │   └── TestResults.json (Newman report)
  ├── Swagger/
  │   ├── Auth_Login_Success.png
  │   ├── Users_GetAll_Authorized.png
  │   ├── Users_GetAll_Unauthorized.png
  │   └── Expenses_Create_Validation_Error.png
  └── Logs/
      ├── serilog_2026-03-16.txt (Logs del día de pruebas)
      └── database_queries.log
```

#### **Para Tests de Seguridad:**
```
📁 Evidencias/SecurityTests/
  ├── OWASP_ZAP_Report.html
  ├── SQL_Injection_Attempts.png
  ├── CORS_Blocked_Request.png
  ├── BCrypt_Hashes_In_Database.png
  └── JWT_Token_Validation_Failure.png
```

### 14.7 Métricas de Cobertura de Código

**Objetivo:** Mínimo 80% de cobertura en componentes críticos

| Componente               | Cobertura Actual | Objetivo | Estado |
| ------------------------ | ---------------- | -------- | ------ |
| DTOs & Converters        | 95%              | 80%      | ✅     |
| Validators               | 100%             | 80%      | ✅     |
| Repositories             | 60%              | 70%      | ⚠️     |
| Controllers              | 45%              | 60%      | ⚠️     |
| Utils (PasswordHasher)   | 100%             | 90%      | ✅     |
| **Promedio General**     | **70%**          | **75%**  | ⚠️     |

**Comando para generar reporte:**
```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"coverage.cobertura.xml" -targetdir:"coveragereport"
```

### 14.8 Casos de Prueba de Alto Nivel (E2E)

| ID   | Escenario Completo                                  | Flujo                                                                  | Resultado Esperado                           |
| ---- | --------------------------------------------------- | ---------------------------------------------------------------------- | -------------------------------------------- |
| E2E1 | Habitante consulta y paga gasto común              | Login → Ver gastos de mi propiedad → Registrar pago → Confirmar estado | Pago registrado, estado actualizado          |
| E2E2 | Admin crea gasto, habitante lo visualiza          | Admin crea gasto común → Habitante login → Ve nuevo gasto en su lista | Gasto visible para propietario               |
| E2E3 | Habitante reserva recurso compartido               | Login → Ver recursos → Seleccionar fecha/hora → Confirmar reserva     | Reserva creada, costo calculado              |
| E2E4 | Admin gestiona incidente reportado                 | Habitante reporta incidente → Admin lo revisa → Cambia estado → Asigna costo | Incidente gestionado de inicio a fin         |
| E2E5 | Cambio de contraseña con validación               | Usuario actualiza password → Validación fuerte → Hash guardado → Login con nueva password | Nueva contraseña funcional                   |

### 14.9 Checklist de Testing

- [x] **Tests Unitarios**: DTOConverters, StrongPasswordAttribute
- [ ] **Tests Unitarios**: PasswordHasher utilities
- [ ] **Tests de Integración**: AuthController (Login)
- [ ] **Tests de Integración**: UsersController (CRUD completo)
- [ ] **Tests de Integración**: ExpensesController
- [ ] **Tests de Integración**: ResourcesController
- [ ] **Tests de Integración**: IncidentsController
- [ ] **Tests de Seguridad**: SQL Injection
- [ ] **Tests de Seguridad**: JWT manipulation
- [ ] **Tests de Seguridad**: CORS validation
- [ ] **Tests E2E**: Flujos completos con Postman/Swagger
- [ ] **Performance Tests**: Carga con 100+ usuarios concurrentes
- [ ] **Documentación**: Colección Postman con examples
- [ ] **Documentación**: Swagger mejorado con ejemplos

---

## 15. Conclusión

El backend **CondominioAPI** implementa una arquitectura sólida y escalable para la gestión de condominios, con énfasis en seguridad (JWT, BCrypt, validaciones) y buenas prácticas de desarrollo (capas, patrones, logging). El diseño modular permite extender funcionalidades fácilmente, como se evidencia en la adición de módulos de Recursos e Incidentes en la versión 2.0.

La separación clara entre controladores, repositorios y modelos facilita el mantenimiento y testing, mientras que el uso de Entity Framework Core con MySQL proporciona un acceso a datos eficiente y type-safe.

La estrategia de testing multinivel (unitario, integración, seguridad) asegura la calidad del código y protege contra vulnerabilidades comunes, con especial atención a la validación de contraseñas y el control de acceso basado en roles.

---

**Documento generado:** Marzo 2026  
**Versión del Sistema:** v2.0+  
**Stack:** .NET Core + MySQL + JWT
