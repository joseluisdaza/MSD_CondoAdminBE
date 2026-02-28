# ?? Nuevo Endpoint: Update Password

## Endpoint Agregado
**`PUT /api/users/{id}/password`** - Actualizar contraseña de usuario

### Descripción
Permite actualizar únicamente la contraseña de un usuario con validaciones de seguridad.

### Autorización
- **Administradores/Super**: Pueden cambiar la contraseña de cualquier usuario
- **Habitantes**: Solo pueden cambiar su propia contraseña

### Request Body
```json
{
  "newPassword": "NewPassword123!",
  "confirmPassword": "NewPassword123!"
}
```

### Validaciones Aplicadas
- ? **StrongPassword**: Mínimo 8 caracteres, mayúscula, minúscula, número y símbolo
- ? **Compare**: La confirmación debe coincidir con la nueva contraseña
- ? **Required**: Ambos campos son obligatorios

### Ejemplo de Uso en Postman

#### Request
```
PUT {{base_url}}/api/users/1/password
Authorization: Bearer {{jwt_token}}
Content-Type: application/json

{
  "newPassword": "MyNewSecurePass123!",
  "confirmPassword": "MyNewSecurePass123!"
}
```

#### Response Success (200)
```json
{
  "message": "Contraseña actualizada exitosamente."
}
```

#### Response Error (400) - Validación
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "NewPassword": [
      "La contraseña debe tener al menos 8 caracteres, incluyendo una letra mayúscula, una minúscula, un número y un símbolo."
    ],
    "ConfirmPassword": [
      "La confirmación de contraseña no coincide"
    ]
  }
}
```

#### Response Error (403) - Sin permisos
```json
{
  "message": "No tienes permisos para cambiar la contraseña de otro usuario."
}
```

#### Response Error (404) - Usuario no encontrado
```json
{
  "message": "Usuario no encontrado."
}
```

### Casos de Uso

#### 1. Administrador cambia contraseña de cualquier usuario
```bash
# Login como admin
POST /api/auth/login
{
  "username": "usa",
  "password": "sa"
}

# Cambiar contraseña del usuario ID 5
PUT /api/users/5/password
{
  "newPassword": "NewEmployeePass123!",
  "confirmPassword": "NewEmployeePass123!"
}
```

#### 2. Usuario cambia su propia contraseña
```bash
# Login como habitante (ID 4)
POST /api/auth/login
{
  "username": "uhabitante",
  "password": "habitante"
}

# Cambiar su propia contraseña
PUT /api/users/4/password
{
  "newPassword": "MyPersonalPass456@",
  "confirmPassword": "MyPersonalPass456@"
}
```

#### 3. Intento no autorizado
```bash
# Login como habitante (ID 4)
POST /api/auth/login
{
  "username": "uhabitante",
  "password": "habitante"
}

# Intentar cambiar contraseña de otro usuario (ID 1) - FALLA
PUT /api/users/1/password
# Response: 403 Forbidden
```

### Logging y Auditoría
El endpoint registra automáticamente:
- Intentos de cambio de contraseña
- Usuario que realiza el cambio
- Usuario objetivo del cambio
- Resultado de la operación

### Agregar a Postman Collection

Para agregar este endpoint a tu colección de Postman:

1. **Crear nueva request** en la carpeta "Users"
2. **Configurar request**:
   - Method: `PUT`
   - URL: `{{base_url}}/api/users/1/password`
   - Headers: `Authorization: Bearer {{jwt_token}}`
   - Body (raw JSON):
   ```json
   {
     "newPassword": "NewPassword123!",
     "confirmPassword": "NewPassword123!"
   }
   ```

3. **Crear test script** (opcional):
```javascript
pm.test("Status code is 200", function () {
    pm.response.to.have.status(200);
});

pm.test("Response has success message", function () {
    var jsonData = pm.response.json();
    pm.expect(jsonData.message).to.include("exitosamente");
});
```

### Seguridad Implementada
- ? **Autorización por roles**
- ? **Validación de identidad** (usuarios solo pueden cambiar su propia contraseña)
- ? **Hash seguro** de contraseñas con BCrypt
- ? **Validación de contraseña fuerte**
- ? **Confirmación de contraseña**
- ? **Logging de operaciones**

¡El endpoint está listo para usar! ??