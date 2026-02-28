# Seguridad de Contraseñas - CondominioAPI

## Implementación de Hashing con BCrypt

Este proyecto utiliza **BCrypt** para el hashing seguro de contraseñas. BCrypt es un algoritmo de hash adaptativo diseñado específicamente para contraseñas.

## ? Características de Seguridad

### 1. **Hashing Unidireccional**
- Las contraseñas nunca se almacenan en texto plano
- BCrypt genera un hash único que no puede ser revertido
- Cada vez que se hashea la misma contraseña, se genera un hash diferente (gracias al salt automático)

### 2. **Salt Automático**
- BCrypt genera automáticamente un salt único para cada contraseña
- El salt se almacena como parte del hash
- Protege contra ataques de rainbow table

### 3. **Costo Adaptativo**
- BCrypt permite ajustar el "factor de trabajo" para hacer el hashing más lento
- Esto protege contra ataques de fuerza bruta
- El costo puede aumentarse en el futuro sin cambiar el código

## ?? Uso

### Crear un Usuario
Cuando se crea un usuario, la contraseña se hashea automáticamente:

```csharp
POST /api/users
{
  "userName": "JohnDoe",
  "lastName": "Doe",
  "legalId": "12345",
  "login": "johndoe",
  "password": "mySecurePassword123",
  "startDate": "2025-01-01T00:00:00Z"
}
```

La contraseña `"mySecurePassword123"` se hashea antes de guardarse en la base de datos.

### Login
Cuando un usuario hace login, la contraseña se verifica contra el hash:

```csharp
POST /api/auth/login
{
  "login": "johndoe",
  "password": "mySecurePassword123"
}
```

BCrypt compara la contraseña en texto plano con el hash almacenado sin necesidad de desencriptar.

## ?? Migración de Contraseñas Existentes

Si ya tienes usuarios con contraseñas en texto plano, ejecuta el endpoint de migración **UNA SOLA VEZ**:

```bash
POST /api/migration/hash-passwords
```

Este endpoint:
1. Lee todos los usuarios de la base de datos
2. Identifica contraseñas en texto plano (no hasheadas)
3. Hashea cada contraseña con BCrypt
4. Actualiza los registros en la base de datos

**?? IMPORTANTE:**
- Ejecuta este endpoint solo una vez
- Después de la migración, elimina o protege este endpoint
- En producción, agrega `[Authorize]` con rol de administrador

## ?? Ejemplo de Hash BCrypt

Una contraseña como `"password123"` se convierte en algo como:

```
$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy
```

Estructura del hash:
- `$2a$` = Versión del algoritmo BCrypt
- `11$` = Factor de costo (2^11 iteraciones)
- Los siguientes 22 caracteres = Salt
- El resto = Hash de la contraseña

## ??? Buenas Prácticas Implementadas

1. ? **Nunca almacenar contraseñas en texto plano**
2. ? **Usar un algoritmo de hashing moderno (BCrypt)**
3. ? **Generar salt único para cada contraseña**
4. ? **Validación de contraseñas nulas o vacías**
5. ? **Manejo de errores en la verificación**
6. ? **No exponer información sobre por qué falló el login**

## ?? Lo que NO se hace (y está bien)

- ? **No se "desencripta" la contraseña** - Es imposible y así debe ser
- ? **No se envía la contraseña hasheada en las respuestas** - Por seguridad
- ? **No se puede recuperar la contraseña original** - Solo se puede resetear

## ?? Recomendaciones Adicionales

Para mejorar aún más la seguridad:

1. **Implementar políticas de contraseñas fuertes:**
   - Mínimo 8 caracteres
   - Combinación de mayúsculas, minúsculas, números y símbolos
   - Validar antes de hashear

2. **Implementar límite de intentos de login:**
   - Bloquear cuenta después de N intentos fallidos
   - Implementar CAPTCHA después de X intentos

3. **Agregar autenticación de dos factores (2FA)**

4. **Implementar reseteo seguro de contraseñas:**
   - Enviar token único por email
   - Token con expiración
   - No mostrar si el usuario existe o no

5. **Logging de intentos de login:**
   - Registrar intentos fallidos
   - Detectar patrones sospechosos

## ?? Referencias

- [BCrypt.Net-Next Documentation](https://github.com/BcryptNet/bcrypt.net)
- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
