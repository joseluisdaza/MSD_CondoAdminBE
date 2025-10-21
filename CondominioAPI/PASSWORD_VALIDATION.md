# ?? Validaci�n de Contrase�as Fuertes - CondominioAPI

## Implementaci�n Completada ?

Se ha implementado un sistema de validaci�n autom�tica de contrase�as fuertes usando **Data Annotations** de .NET.

---

## ?? Requisitos de Contrase�a

Todas las contrase�as en el sistema deben cumplir con los siguientes requisitos:

| Requisito | Descripci�n | Ejemplo |
|-----------|-------------|---------|
| ?? **Longitud** | M�nimo 12 caracteres | `MyPass123456!` ? |
| 1?? **N�meros** | Al menos 1 n�mero (0-9) | `Password1!` ? |
| ?? **May�sculas** | Al menos 1 letra may�scula (A-Z) | `Password1!` ? |
| ? **Especiales** | Al menos 1 caracter especial (!@#$%^&*...) | `Password123!` ? |

---

## ??? C�mo Funciona

### **1. Validaci�n Autom�tica en el DTO**

El atributo `[StrongPassword]` se aplica directamente en la propiedad `Password` del DTO:

```csharp
public class UserRequest : UserBaseRequest
{
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    [StrongPassword]  // ? Validaci�n autom�tica
    public string Password { get; set; }
}
```

### **2. Validaci�n en el Controller**

ASP.NET Core valida autom�ticamente el modelo antes de entrar al m�todo del controller:

```csharp
[HttpPost]
[Authorize]
public async Task<ActionResult<UserRequest>> Create(UserRequest user)
{
    // Si la contrase�a no cumple los requisitos, retorna BadRequest autom�ticamente
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }
    
    // El resto del c�digo solo se ejecuta si la contrase�a es v�lida
    // ...
}
```

---

## ?? Endpoints de Prueba

### **1. Validar una Contrase�a**

**POST** `/api/passwordvalidationdemo/validate`

Prueba si una contrase�a cumple con los requisitos antes de crear un usuario.

**Request:**
```json
{
  "password": "MyTestPassword123!"
}
```

**Response (V�lida):**
```json
{
  "password": "MyTestPassword123!",
  "isValid": true,
  "summary": "? La contrase�a cumple con todos los requisitos de seguridad",
  "requirements": [
    {
      "requirement": "Longitud",
      "isMet": true,
      "status": "?",
      "description": "Al menos 12 caracteres (actual: 19)"
    },
    {
      "requirement": "N�mero",
      "isMet": true,
      "status": "?",
      "description": "Al menos 1 n�mero (0-9)"
    },
    {
      "requirement": "May�scula",
      "isMet": true,
      "status": "?",
      "description": "Al menos 1 letra may�scula (A-Z)"
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

**Response (Inv�lida):**
```json
{
  "password": "weak",
  "isValid": false,
  "summary": "? La contrase�a no cumple con los requisitos de seguridad",
  "requirements": [
    {
      "requirement": "Longitud",
      "isMet": false,
      "status": "?",
      "description": "Al menos 12 caracteres (actual: 4)"
    },
    {
      "requirement": "N�mero",
      "isMet": false,
      "status": "?",
      "description": "Al menos 1 n�mero (0-9)"
    },
    {
      "requirement": "May�scula",
      "isMet": false,
      "status": "?",
      "description": "Al menos 1 letra may�scula (A-Z)"
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

### **2. Ver Requisitos de Contrase�a**

**GET** `/api/passwordvalidationdemo/requirements`

Obtiene la lista de requisitos sin validar ninguna contrase�a.

**Response:**
```json
{
  "title": "Requisitos de Contrase�a Segura",
  "requirements": [
    {
      "name": "Longitud m�nima",
      "value": "12 caracteres",
      "icon": "??"
    },
    {
      "name": "N�meros",
      "value": "Al menos 1 n�mero (0-9)",
      "icon": "1??"
    },
    {
      "name": "May�sculas",
      "value": "Al menos 1 letra may�scula (A-Z)",
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
        "reason": "No contiene n�meros"
      },
      {
        "password": "no-uppercase-123",
        "reason": "No contiene may�sculas"
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

### **Crear Usuario con Contrase�a V�lida**

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

### **Crear Usuario con Contrase�a Inv�lida**

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
      "La contrase�a debe contener al menos 12 caracteres, al menos 1 n�mero, al menos 1 letra may�scula, al menos 1 caracter especial (!@#$%^&*...)"
    ]
  }
}
```

---

## ?? Ejemplos de Contrase�as

### ? **Contrase�as V�lidas:**

- `MySecurePass123!`
- `C0mpl3x&Strong`
- `Password2025#Secure`
- `Admin@123456789`
- `Test!ng_Pass2025`

### ? **Contrase�as Inv�lidas:**

| Contrase�a | Raz�n |
|------------|-------|
| `short` | Menos de 12 caracteres |
| `NoNumbersHere!` | No contiene n�meros |
| `no-uppercase-123` | No contiene may�sculas |
| `NoSpecialChar123` | No contiene caracteres especiales |
| `Only Spaces 123!` | Los espacios NO cuentan como caracteres especiales |

---

## ?? Personalizaci�n

Si necesitas cambiar los requisitos, edita la clase `StrongPasswordAttribute` en:

```
Condominio.DTOs/Validation/StrongPasswordAttribute.cs
```

Puedes ajustar:
- `MinimumLength`: Longitud m�nima (actualmente 12)
- Agregar m�s validaciones (min�sculas, longitud m�xima, etc.)
- Personalizar mensajes de error

---

## ?? Beneficios de Esta Implementaci�n

1. ? **Validaci�n Autom�tica**: No necesitas validar manualmente en cada controller
2. ? **Reutilizable**: El atributo se puede aplicar a cualquier propiedad de contrase�a
3. ? **Mensajes Claros**: Los usuarios reciben feedback espec�fico sobre qu� falta
4. ? **Est�ndar de .NET**: Usa Data Annotations, el patr�n est�ndar de ASP.NET Core
5. ? **Swagger Compatible**: Los requisitos aparecen en la documentaci�n de la API
6. ? **Testeable**: F�cil de probar con unit tests

---

## ?? Integraci�n con Frontend

Tu frontend puede:

1. **Obtener los requisitos** con `GET /api/passwordvalidationdemo/requirements`
2. **Validar en tiempo real** llamando a `POST /api/passwordvalidationdemo/validate`
3. **Mostrar feedback visual** mientras el usuario escribe la contrase�a
4. **Prevenir env�os inv�lidos** antes de llamar al endpoint de creaci�n de usuario

---

## ?? Referencias

- [Data Annotations en .NET](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations)
- [Model Validation en ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [OWASP Password Guidelines](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
