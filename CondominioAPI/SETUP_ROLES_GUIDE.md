# ?? Gu�a R�pida de Configuraci�n - Sistema de Roles

## Configuraci�n Inicial (Primera Vez)

Sigue estos pasos para configurar el sistema de roles por primera vez:

---

### **Paso 1: Crear un Usuario Inicial**

```http
POST /api/users
Authorization: Bearer <token_temporal> (o desactiva temporalmente [Authorize])
Content-Type: application/json

{
  "userName": "Admin",
  "lastName": "Sistema",
  "legalId": "ADMIN001",
  "login": "admin",
  "password": "AdminPass123!",
  "startDate": "2025-01-20T00:00:00Z"
}
```

---

### **Paso 2: Inicializar los Roles del Sistema**

```http
POST /api/rolesetup/initialize-roles
```

**Response:**
```json
{
  "message": "Roles creados exitosamente",
  "createdRoles": [
    "Defecto",
    "Habitante",
    "Administrador",
    "RoleAdmin"
  ],
  "totalRoles": 4
}
```

---

### **Paso 3: Asignar Roles de Administrador al Primer Usuario**

```http
POST /api/rolesetup/setup-first-admin
```

**Response:**
```json
{
  "message": "Roles de administrador asignados exitosamente",
  "user": {
    "id": 1,
    "login": "admin",
    "userName": "Admin Sistema"
  },
  "assignedRoles": [
    "Administrador",
    "RoleAdmin"
  ]
}
```

---

### **Paso 4: Login con el Usuario Administrador**

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
    "userName": "Admin Sistema",
    "roles": [
      "Administrador",
      "RoleAdmin"
    ]
  }
}
```

---

### **Paso 5: Probar que Funciona**

```http
GET /api/users
Authorization: Bearer <token_del_paso_4>
```

? Deber�as recibir la lista de usuarios (200 OK)

---

## ?? Seguridad Post-Setup

**IMPORTANTE:** Despu�s de la configuraci�n inicial, debes:

### 1. **Proteger el RoleSetupController**

Edita `CondominioAPI/Controllers/RoleSetupController.cs`:

```csharp
// Cambiar de:
[AllowAnonymous]

// A:
[Authorize(Roles = AppRoles.RoleAdmin)]
```

O mejor a�n, **elimina el controller completamente** despu�s del setup inicial.

### 2. **Verificar que UsersController.Create est� protegido**

Aseg�rate de que el endpoint de creaci�n de usuarios requiera rol de Administrador:

```csharp
[HttpPost]
[Authorize(Roles = AppRoles.Administrador)] // ? Ya est� configurado
public async Task<ActionResult<UserRequest>> Create(UserRequest user)
```

---

## ?? Checklist de Configuraci�n

- [ ] ? Usuario administrador creado
- [ ] ? Roles del sistema inicializados
- [ ] ? Roles asignados al administrador
- [ ] ? Login exitoso con roles incluidos en el token
- [ ] ? Endpoints protegidos funcionando correctamente
- [ ] ?? RoleSetupController protegido o eliminado
- [ ] ?? Contrase�a del admin cambiada

---

## ?? Pruebas de Roles

### **Test 1: Administrador puede ver todos los usuarios**

```http
GET /api/users
Authorization: Bearer <token_admin>
```
? Esperado: 200 OK con lista de usuarios

### **Test 2: Habitante NO puede ver todos los usuarios**

```http
GET /api/users
Authorization: Bearer <token_habitante>
```
? Esperado: 403 Forbidden

### **Test 3: Habitante puede ver su propia informaci�n**

```http
GET /api/users/{su_propio_id}
Authorization: Bearer <token_habitante>
```
? Esperado: 200 OK con su informaci�n

### **Test 4: Habitante NO puede ver informaci�n de otro usuario**

```http
GET /api/users/{otro_id}
Authorization: Bearer <token_habitante>
```
? Esperado: 403 Forbidden

### **Test 5: Solo RoleAdmin puede asignar roles**

```http
POST /api/role
Authorization: Bearer <token_roleadmin>
Content-Type: application/json

{
  "userId": 2,
  "roleId": 2
}
```
? Esperado: 200 OK

### **Test 6: Administrador NO puede asignar roles**

```http
POST /api/role
Authorization: Bearer <token_admin_sin_roleadmin>
Content-Type: application/json

{
  "userId": 2,
  "roleId": 2
}
```
? Esperado: 403 Forbidden

---

## ?? Crear Usuarios con Diferentes Roles

### **Crear un Habitante**

```http
POST /api/users
Authorization: Bearer <token_admin>
Content-Type: application/json

{
  "userName": "Juan",
  "lastName": "P�rez",
  "legalId": "12345",
  "login": "jperez",
  "password": "SecurePass123!",
  "startDate": "2025-01-20T00:00:00Z"
}
```

Luego asignar rol Habitante:

```http
POST /api/role
Authorization: Bearer <token_roleadmin>
Content-Type: application/json

{
  "userId": 2,  // ID del usuario reci�n creado
  "roleId": 2   // ID del rol Habitante (verifica en /api/rolesetup/list-roles)
}
```

---

## ?? Verificar Configuraci�n

### **Listar todos los roles**

```http
GET /api/rolesetup/list-roles
```

**Response:**
```json
{
  "totalRoles": 4,
  "roles": [
    {
      "id": 1,
      "name": "Defecto",
      "description": "Rol por defecto para operaciones b�sicas..."
    },
    {
      "id": 2,
      "name": "Habitante",
      "description": "Residente que puede ver su informaci�n..."
    },
    {
      "id": 3,
      "name": "Administrador",
      "description": "Administrador del sistema..."
    },
    {
      "id": 4,
      "name": "RoleAdmin",
      "description": "Administrador de roles..."
    }
  ]
}
```

### **Ver roles de un usuario**

```http
GET /api/role/1
Authorization: Bearer <token_roleadmin_o_admin>
```

---

## ?? Troubleshooting

### **Problema: No puedo acceder a ning�n endpoint**

**Soluci�n:**
1. Verifica que el token est� en el header: `Authorization: Bearer <token>`
2. Verifica que el token no haya expirado (1 hora de validez)
3. Haz login nuevamente para obtener un nuevo token

### **Problema: Recibo 403 Forbidden**

**Soluci�n:**
1. Verifica que tu usuario tenga el rol requerido
2. Consulta `/api/role/{tu_id}` para ver tus roles actuales
3. Pide a un RoleAdmin que te asigne el rol necesario

### **Problema: El token no incluye los roles**

**Soluci�n:**
1. Verifica que los roles est�n asignados en la tabla `user_roles`
2. Verifica que `End_Date` sea `NULL` para roles activos
3. Haz login nuevamente para obtener un token actualizado

---

## ?? Mejores Pr�cticas

1. ? **Cambia la contrase�a del administrador** despu�s del setup inicial
2. ? **Elimina o protege RoleSetupController** despu�s de la configuraci�n
3. ? **Usa contrase�as fuertes** para todos los usuarios
4. ? **Revisa peri�dicamente** los roles asignados a los usuarios
5. ? **Documenta** qui�n tiene acceso a RoleAdmin
6. ? **Implementa auditor�a** de cambios de roles
7. ? **No compartas tokens** entre usuarios

---

## ?? Notas Finales

- Los tokens expiran en **1 hora**
- Los roles se obtienen de la base de datos **solo en el login**
- Si cambias roles, el usuario debe **hacer login nuevamente**
- Un usuario puede tener **m�ltiples roles simult�neamente**
- Los roles inactivos (`End_Date != NULL`) no se incluyen en el token

---

�Tu sistema de roles est� listo! ??
