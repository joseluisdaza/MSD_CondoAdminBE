# ?? Guía Rápida de Configuración - Sistema de Roles

## Configuración Inicial (Primera Vez)

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

? Deberías recibir la lista de usuarios (200 OK)

---

## ?? Seguridad Post-Setup

**IMPORTANTE:** Después de la configuración inicial, debes:

### 1. **Proteger el RoleSetupController**

Edita `CondominioAPI/Controllers/RoleSetupController.cs`:

```csharp
// Cambiar de:
[AllowAnonymous]

// A:
[Authorize(Roles = AppRoles.RoleAdmin)]
```

O mejor aún, **elimina el controller completamente** después del setup inicial.

### 2. **Verificar que UsersController.Create esté protegido**

Asegúrate de que el endpoint de creación de usuarios requiera rol de Administrador:

```csharp
[HttpPost]
[Authorize(Roles = AppRoles.Administrador)] // ? Ya está configurado
public async Task<ActionResult<UserRequest>> Create(UserRequest user)
```

---

## ?? Checklist de Configuración

- [ ] ? Usuario administrador creado
- [ ] ? Roles del sistema inicializados
- [ ] ? Roles asignados al administrador
- [ ] ? Login exitoso con roles incluidos en el token
- [ ] ? Endpoints protegidos funcionando correctamente
- [ ] ?? RoleSetupController protegido o eliminado
- [ ] ?? Contraseña del admin cambiada

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

### **Test 3: Habitante puede ver su propia información**

```http
GET /api/users/{su_propio_id}
Authorization: Bearer <token_habitante>
```
? Esperado: 200 OK con su información

### **Test 4: Habitante NO puede ver información de otro usuario**

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
  "lastName": "Pérez",
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
  "userId": 2,  // ID del usuario recién creado
  "roleId": 2   // ID del rol Habitante (verifica en /api/rolesetup/list-roles)
}
```

---

## ?? Verificar Configuración

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
      "description": "Rol por defecto para operaciones básicas..."
    },
    {
      "id": 2,
      "name": "Habitante",
      "description": "Residente que puede ver su información..."
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

### **Problema: No puedo acceder a ningún endpoint**

**Solución:**
1. Verifica que el token esté en el header: `Authorization: Bearer <token>`
2. Verifica que el token no haya expirado (1 hora de validez)
3. Haz login nuevamente para obtener un nuevo token

### **Problema: Recibo 403 Forbidden**

**Solución:**
1. Verifica que tu usuario tenga el rol requerido
2. Consulta `/api/role/{tu_id}` para ver tus roles actuales
3. Pide a un RoleAdmin que te asigne el rol necesario

### **Problema: El token no incluye los roles**

**Solución:**
1. Verifica que los roles estén asignados en la tabla `user_roles`
2. Verifica que `End_Date` sea `NULL` para roles activos
3. Haz login nuevamente para obtener un token actualizado

---

## ?? Mejores Prácticas

1. ? **Cambia la contraseña del administrador** después del setup inicial
2. ? **Elimina o protege RoleSetupController** después de la configuración
3. ? **Usa contraseñas fuertes** para todos los usuarios
4. ? **Revisa periódicamente** los roles asignados a los usuarios
5. ? **Documenta** quién tiene acceso a RoleAdmin
6. ? **Implementa auditoría** de cambios de roles
7. ? **No compartas tokens** entre usuarios

---

## ?? Notas Finales

- Los tokens expiran en **1 hora**
- Los roles se obtienen de la base de datos **solo en el login**
- Si cambias roles, el usuario debe **hacer login nuevamente**
- Un usuario puede tener **múltiples roles simultáneamente**
- Los roles inactivos (`End_Date != NULL`) no se incluyen en el token

---

¡Tu sistema de roles está listo! ??
