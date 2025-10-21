# ?? Sistema de Roles y Autorizaci�n - CondominioAPI

## Implementaci�n Completada ?

Se ha implementado un sistema completo de autorizaci�n basado en roles usando JWT y Claims en ASP.NET Core.

---

## ?? Roles del Sistema

| Rol | Nombre en Sistema | Descripci�n | Permisos |
|-----|------------------|-------------|----------|
| ?? **Defecto** | `Defecto` | Rol por defecto | Health check, Login |
| ?? **Habitante** | `Habitante` | Usuario residente | Ver su informaci�n personal y propiedades asignadas |
| ?? **Administrador** | `Administrador` | Administrador del sistema | CRUD completo de Usuarios y Propiedades |
| ?? **RoleAdmin** | `RoleAdmin` | Administrador de roles | Gestionar roles de usuarios |

---

## ?? Matriz de Permisos por Endpoint

### ?? **Autenticaci�n**

| Endpoint | M�todo | Rol Requerido | Descripci�n |
|----------|--------|---------------|-------------|
| `/api/auth/login` | POST | ? Ninguno (AllowAnonymous) | Login de usuario |
| `/api/authhealthcheck` | GET | ? Defecto (Cualquier usuario autenticado) | Verificar autenticaci�n |

---

### ?? **Usuarios**

| Endpoint | M�todo | Roles Permitidos | Descripci�n |
|----------|--------|------------------|-------------|
| `/api/users` | GET | ?? Administrador | Obtener todos los usuarios |
| `/api/users/{id}` | GET | ?? Administrador, ?? Habitante* | Ver usuario por ID (* solo su propia info) |
| `/api/users` | POST | ?? Administrador | Crear nuevo usuario |
| `/api/users/{id}` | PUT | ?? Administrador | Actualizar usuario |
| `/api/users/{id}` | DELETE | ?? Administrador | Eliminar usuario (soft delete) |

**Nota:** Los habitantes solo pueden ver su propia informaci�n (validaci�n por ID)

---

### ?? **Roles**

| Endpoint | M�todo | Roles Permitidos | Descripci�n |
|----------|--------|------------------|-------------|
| `/api/role/{id}` | GET | ?? RoleAdmin, ?? Administrador | Ver roles de un usuario |
| `/api/role/available` | GET | ?? RoleAdmin, ?? Administrador | Listar todos los roles disponibles |
| `/api/role` | POST | ?? RoleAdmin | Asignar rol a usuario |
| `/api/role` | DELETE | ?? RoleAdmin | Remover rol de usuario |

---

### ?? **Propiedades**

| Endpoint | M�todo | Roles Permitidos | Descripci�n |
|----------|--------|------------------|-------------|
| `/api/property` | GET | ?? Administrador | Obtener todas las propiedades |
| `/api/property/my-properties` | GET | ?? Habitante, ?? Administrador | Ver propiedades propias o todas |
| `/api/property/{id}` | GET | ?? Administrador, ?? Habitante* | Ver propiedad por ID (* solo asignadas) |
| `/api/property` | POST | ?? Administrador | Crear nueva propiedad |
| `/api/property/{id}` | PUT | ?? Administrador | Actualizar propiedad |
| `/api/property/{id}` | DELETE | ?? Administrador | Eliminar propiedad (soft delete) |

---

### ?? **Tipos de Propiedad**

| Endpoint | M�todo | Roles Permitidos | Descripci�n |
|----------|--------|------------------|-------------|
| `/api/propertytype` | GET | ?? Administrador, ?? Habitante | Listar tipos de propiedad |
| `/api/propertytype/{id}` | GET | ?? Administrador, ?? Habitante | Ver tipo de propiedad por ID |
| `/api/propertytype` | POST | ?? Administrador | Crear tipo de propiedad |
| `/api/propertytype/{id}` | PUT | ?? Administrador | Actualizar tipo de propiedad |
| `/api/propertytype/{id}` | DELETE | ?? Administrador | Eliminar tipo de propiedad |

---

## ?? C�mo Funciona

### **1. Login y Generaci�n de Token**

Cuando un usuario hace login, el sistema:

1. Valida las credenciales
2. Obtiene los roles activos del usuario
3. Genera un token JWT con los claims:
   - `ClaimTypes.Name`: Login del usuario
   - `ClaimTypes.NameIdentifier`: ID del usuario
   - `ClaimTypes.Role`: Uno o m�s roles del usuario

```json
// Response del login
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "login": "johndoe",
    "userName": "John Doe",
    "roles": ["Habitante", "Defecto"]
  }
}
```

### **2. Autorizaci�n por Rol**

Los endpoints usan el atributo `[Authorize(Roles = "...")]`:

```csharp
// Solo Administradores
[Authorize(Roles = AppRoles.Administrador)]

// Administradores O Habitantes
[Authorize(Roles = $"{AppRoles.Administrador},{AppRoles.Habitante}")]

// Cualquier usuario autenticado
[Authorize]
```

### **3. Validaci�n Adicional en el Controller**

Para casos especiales (ej: habitante viendo solo su info):

```csharp
var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
var isAdmin = User.IsInRole(AppRoles.Administrador);

if (!isAdmin && currentUserId != id)
{
    return Forbid(); // 403 Forbidden
}
```

---

## ?? Pruebas en Postman

### **Paso 1: Login y Obtener Token**

```http
POST /api/auth/login
Content-Type: application/json

{
  "login": "admin",
  "password": "AdminPass123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "login": "admin",
    "userName": "Administrador",
    "roles": ["Administrador", "Defecto"]
  }
}
```

### **Paso 2: Usar el Token**

En Postman:
1. Ve a la pesta�a **Authorization**
2. Selecciona **Bearer Token**
3. Pega el token obtenido

### **Paso 3: Probar Endpoint Protegido**

```http
GET /api/users
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ?? Respuestas de Autorizaci�n

### ? **200 OK / 201 Created**
Usuario tiene el rol requerido y la operaci�n fue exitosa

### ? **401 Unauthorized**
- Token no proporcionado
- Token inv�lido o expirado
- Usuario no autenticado

```json
{
  "status": 401,
  "title": "Unauthorized"
}
```

### ?? **403 Forbidden**
- Usuario autenticado pero sin el rol requerido
- Usuario intentando acceder a recursos que no le pertenecen

```json
{
  "status": 403,
  "title": "Forbidden"
}
```

---

## ?? Seguridad Implementada

### ? **Caracter�sticas de Seguridad**

1. **JWT con Roles**: Los roles se incluyen en el token JWT
2. **Validaci�n en Servidor**: ASP.NET Core valida autom�ticamente los roles
3. **Separaci�n de Permisos**: Cada rol tiene permisos espec�ficos
4. **Validaci�n Adicional**: Controllers verifican propiedad de recursos
5. **Contrase�as Hasheadas**: BCrypt con salt �nico
6. **Tokens con Expiraci�n**: 1 hora de validez

---

## ?? Casos de Uso por Rol

### ?? **Habitante:**
```
? Login
? Ver su propia informaci�n
? Ver sus propiedades asignadas
? Ver tipos de propiedades
? Ver otros usuarios
? Crear/editar/eliminar usuarios
? Gestionar roles
```

### ?? **Administrador:**
```
? Login
? CRUD completo de Usuarios
? CRUD completo de Propiedades
? CRUD completo de Tipos de Propiedad
? Ver roles de usuarios
? Asignar/remover roles (solo RoleAdmin)
```

### ?? **RoleAdmin:**
```
? Login
? Ver roles de usuarios
? Asignar roles a usuarios
? Remover roles de usuarios
? Ver todos los roles disponibles
?? Necesita tambi�n rol de Administrador para gestionar usuarios
```

---

## ??? Configuraci�n Inicial de Roles en la Base de Datos

Para que el sistema funcione, necesitas tener estos roles en la tabla `roles`:

```sql
INSERT INTO roles (Rol_Name, Description) VALUES
('Defecto', 'Rol por defecto para operaciones b�sicas'),
('Habitante', 'Residente que puede ver su informaci�n y propiedades'),
('Administrador', 'Administrador con permisos completos de CRUD'),
('RoleAdmin', 'Administrador de roles y permisos de usuarios');
```

---

## ?? Notas Importantes

1. **Token JWT incluye roles**: No necesitas consultar la BD en cada request
2. **Roles activos**: Solo se incluyen roles con `EndDate = null`
3. **M�ltiples roles**: Un usuario puede tener varios roles simult�neamente
4. **Jerarqu�a**: No hay jerarqu�a impl�cita, cada endpoint define sus roles
5. **Refresh Token**: Actualmente no implementado (token expira en 1 hora)

---

## ?? Pr�ximas Mejoras Sugeridas

1. ? Implementar Refresh Tokens
2. ? Agregar cach� de roles en memoria
3. ? Implementar permisos granulares (policies)
4. ? Logging de acciones por rol
5. ? Implementar 2FA para RoleAdmin
6. ? Rate limiting por rol

---

## ?? Referencias

- [ASP.NET Core Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction)
- [Role-based Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/roles)
- [JWT Bearer Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
