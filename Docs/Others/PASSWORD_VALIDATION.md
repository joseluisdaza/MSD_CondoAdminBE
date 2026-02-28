# ?? Validación de Contraseñas Fuertes - CondominioAPI

## Implementación Completada ?

Se ha implementado un sistema de validación automática de contraseñas fuertes usando **Data Annotations** de .NET.

---

## ?? Requisitos de Contraseña

Todas las contraseñas en el sistema deben cumplir con los siguientes requisitos:

| Requisito | Descripción | Ejemplo |
|-----------|-------------|---------|
| ?? **Longitud** | Mínimo 12 caracteres | `MyPass123456!` ? |
| 1?? **Números** | Al menos 1 número (0-9) | `Password1!` ? |
| ?? **Mayúsculas** | Al menos 1 letra mayúscula (A-Z) | `Password1!` ? |
| ? **Especiales** | Al menos 1 caracter especial (!@#$%^&*...) | `Password123!` ? |

---

## ??? Cómo Funciona

### **1. Validación Automática en el DTO**

El atributo `[StrongPassword]` se aplica directamente en la propiedad `Password` del DTO:

```csharp
public class UserRequest : UserBaseRequest
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    [StrongPassword]  // ? Validación automática
    public string Password { get; set; }
}
```

### **2. Validación en el Controller**

ASP.NET Core valida automáticamente el modelo antes de entrar al método del controller:

```csharp
[HttpPost]
[Authorize]
public async Task<ActionResult<UserRequest>> Create(UserRequest user)
{
    // Si la contraseña no cumple los requisitos, retorna BadRequest automáticamente
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // El resto del código solo se ejecuta si la contraseña es válida
    // ...
}
```

---

## ?? Endpoints de Prueba

### **1. Validar una Contraseña**

**POST** `/api/passwordvalidationdemo/validate`

Prueba si una contraseña cumple con los requisitos antes de crear un usuario.

**Request:**
```json
{
  "password": "MyTestPassword123!"
}
```

**Response (Válida):**
```json
{
  "password": "MyTestPassword123!",
  "isValid": true,
  "summary": "? La contraseña cumple con todos los requisitos de seguridad",
  "requirements": [
    {
      "requirement": "Longitud",
      "isMet": true,
      "status": "?",
      "description": "Al menos 12 caracteres (actual: 19)"
    },
    {
      "requirement": "Número",
      "isMet": true,
      "status": "?",
      "description": "Al menos 1 número (0-9)"
    },
    {
      "requirement": "Mayúscula",
      "isMet": true,
      "status": "?",
      "description": "Al menos 1 letra mayúscula (A-Z)"
    },
    {
      "requirement": "Caracter especial",
      "isMet": true,
      "status": "?",
      "description": "Al menos 1 caracter especial (!@#$%^&*...)"
    }
  ]
}
```

**Response (Inválida):**
```json
{
  "password": "weak",
  "isValid": false,
  "summary": "? La contraseña no cumple con los requisitos de seguridad",
  "requirements": [
    {
      "requirement": "Longitud",
      "isMet": false,
      "status": "?",
      "description": "Al menos 12 caracteres (actual: 4)"
    },
    {
      "requirement": "Número",
      "isMet": false,
      "status": "?",
      "description": "Al menos 1 número (0-9)"
    },
    {
      "requirement": "Mayúscula",
      "isMet": false,
      "status": "?",
      "description": "Al menos 1 letra mayúscula (A-Z)"
    },
    {
      "requirement": "Caracter especial",
      "isMet": false,
      "status": "?",
      "description": "Al menos 1 caracter especial (!@#$%^&*...)"
    }
  ]
}
```

### **2. Ver Requisitos de Contraseña**

**GET** `/api/passwordvalidationdemo/requirements`

Obtiene la lista de requisitos sin validar ninguna contraseña.

**Response:**
```json
{
  "title": "Requisitos de Contraseña Segura",
  "requirements": [
    {
      "name": "Longitud mínima",
      "value": "12 caracteres",
      "icon": "??"
    },
    {
      "name": "Números",
      "value": "Al menos 1 número (0-9)",
      "icon": "1??"
    },
    {
      "name": "Mayúsculas",
      "value": "Al menos 1 letra mayúscula (A-Z)",
      "icon": "??"
    },
    {
      "name": "Caracteres especiales",
      "value": "Al menos 1 caracter especial (!@#$%^&*...)",
      "icon": "?"
    }
  ],
  "examples": {
    "valid": [
      "MySecurePass123!",
      "C0mpl3x&Strong",
      "Password2025#Secure"
    ],
    "invalid": [
      {
        "password": "short",
        "reason": "Menos de 12 caracteres"
      },
      {
        "password": "NoNumbersHere!",
        "reason": "No contiene números"
      },
      {
        "password": "no-uppercase-123",
        "reason": "No contiene mayúsculas"
      },
      {
        "password": "NoSpecialChar123",
        "reason": "No contiene caracteres especiales"
      }
    ]
  }
}
```

---

## ?? Ejemplos de Uso

### **Crear Usuario con Contraseña Válida**

**POST** `/api/users`

```json
{
  "userName": "John",
  "lastName": "Doe",
  "legalId": "12345",
  "login": "johndoe",
  "password": "SecurePass123!",
  "startDate": "2025-01-20T00:00:00Z"
}
```

? **Response:** `201 Created` - Usuario creado exitosamente

### **Crear Usuario con Contraseña Inválida**

**POST** `/api/users`

```json
{
  "userName": "John",
  "lastName": "Doe",
  "legalId": "12345",
  "login": "johndoe",
  "password": "weak",
  "startDate": "2025-01-20T00:00:00Z"
}
```

? **Response:** `400 Bad Request`

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Password": [
      "La contraseña debe contener al menos 12 caracteres, al menos 1 número, al menos 1 letra mayúscula, al menos 1 caracter especial (!@#$%^&*...)"
    ]
  }
}
```

---

## ?? Ejemplos de Contraseñas

### ? **Contraseñas Válidas:**

- `MySecurePass123!`
- `C0mpl3x&Strong`
- `Password2025#Secure`
- `Admin@123456789`
- `Test!ng_Pass2025`

### ? **Contraseñas Inválidas:**

| Contraseña | Razón |
|------------|-------|
| `short` | Menos de 12 caracteres |
| `NoNumbersHere!` | No contiene números |
| `no-uppercase-123` | No contiene mayúsculas |
| `NoSpecialChar123` | No contiene caracteres especiales |
| `Only Spaces 123!` | Los espacios NO cuentan como caracteres especiales |

---

## ?? Personalización

Si necesitas cambiar los requisitos, edita la clase `StrongPasswordAttribute` en:

```
Condominio.DTOs/Validation/StrongPasswordAttribute.cs
```

Puedes ajustar:
- `MinimumLength`: Longitud mínima (actualmente 12)
- Agregar más validaciones (minúsculas, longitud máxima, etc.)
- Personalizar mensajes de error

---

## ?? Beneficios de Esta Implementación

1. ? **Validación Automática**: No necesitas validar manualmente en cada controller
2. ? **Reutilizable**: El atributo se puede aplicar a cualquier propiedad de contraseña
3. ? **Mensajes Claros**: Los usuarios reciben feedback específico sobre qué falta
4. ? **Estándar de .NET**: Usa Data Annotations, el patrón estándar de ASP.NET Core
5. ? **Swagger Compatible**: Los requisitos aparecen en la documentación de la API
6. ? **Testeable**: Fácil de probar con unit tests

---

## ?? Integración con Frontend

Tu frontend puede:

1. **Obtener los requisitos** con `GET /api/passwordvalidationdemo/requirements`
2. **Validar en tiempo real** llamando a `POST /api/passwordvalidationdemo/validate`
3. **Mostrar feedback visual** mientras el usuario escribe la contraseña
4. **Prevenir envíos inválidos** antes de llamar al endpoint de creación de usuario

---

## ?? Referencias

- [Data Annotations en .NET](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)
- [Model Validation en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [OWASP Password Guidelines](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
