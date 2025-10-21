# Seguridad de Contrase�as - CondominioAPI

## Implementaci�n de Hashing con BCrypt

Este proyecto utiliza **BCrypt** para el hashing seguro de contrase�as. BCrypt es un algoritmo de hash adaptativo dise�ado espec�ficamente para contrase�as.

## ? Caracter�sticas de Seguridad

### 1. **Hashing Unidireccional**
- Las contrase�as nunca se almacenan en texto plano
- BCrypt genera un hash �nico que no puede ser revertido
- Cada vez que se hashea la misma contrase�a, se genera un hash diferente (gracias al salt autom�tico)

### 2. **Salt Autom�tico**
- BCrypt genera autom�ticamente un salt �nico para cada contrase�a
- El salt se almacena como parte del hash
- Protege contra ataques de rainbow table

### 3. **Costo Adaptativo**
- BCrypt permite ajustar el "factor de trabajo" para hacer el hashing m�s lento
- Esto protege contra ataques de fuerza bruta
- El costo puede aumentarse en el futuro sin cambiar el c�digo

## ?? Uso

### Crear un Usuario
Cuando se crea un usuario, la contrase�a se hashea autom�ticamente:

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

La contrase�a `"mySecurePassword123"` se hashea antes de guardarse en la base de datos.

### Login
Cuando un usuario hace login, la contrase�a se verifica contra el hash:

```csharp
POST /api/auth/login
{
  "login": "johndoe",
  "password": "mySecurePassword123"
}
```

BCrypt compara la contrase�a en texto plano con el hash almacenado sin necesidad de desencriptar.

## ?? Migraci�n de Contrase�as Existentes

Si ya tienes usuarios con contrase�as en texto plano, ejecuta el endpoint de migraci�n **UNA SOLA VEZ**:

```bash
POST /api/migration/hash-passwords
```

Este endpoint:
1. Lee todos los usuarios de la base de datos
2. Identifica contrase�as en texto plano (no hasheadas)
3. Hashea cada contrase�a con BCrypt
4. Actualiza los registros en la base de datos

**?? IMPORTANTE:**
- Ejecuta este endpoint solo una vez
- Despu�s de la migraci�n, elimina o protege este endpoint
- En producci�n, agrega `[Authorize]` con rol de administrador

## ?? Ejemplo de Hash BCrypt

Una contrase�a como `"password123"` se convierte en algo como:

```
$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy
```

Estructura del hash:
- `$2a$` = Versi�n del algoritmo BCrypt
- `11$` = Factor de costo (2^11 iteraciones)
- Los siguientes 22 caracteres = Salt
- El resto = Hash de la contrase�a

## ??? Buenas Pr�cticas Implementadas

1. ? **Nunca almacenar contrase�as en texto plano**
2. ? **Usar un algoritmo de hashing moderno (BCrypt)**
3. ? **Generar salt �nico para cada contrase�a**
4. ? **Validaci�n de contrase�as nulas o vac�as**
5. ? **Manejo de errores en la verificaci�n**
6. ? **No exponer informaci�n sobre por qu� fall� el login**

## ?? Lo que NO se hace (y est� bien)

- ? **No se "desencripta" la contrase�a** - Es imposible y as� debe ser
- ? **No se env�a la contrase�a hasheada en las respuestas** - Por seguridad
- ? **No se puede recuperar la contrase�a original** - Solo se puede resetear

## ?? Recomendaciones Adicionales

Para mejorar a�n m�s la seguridad:

1. **Implementar pol�ticas de contrase�as fuertes:**
   - M�nimo 8 caracteres
   - Combinaci�n de may�sculas, min�sculas, n�meros y s�mbolos
   - Validar antes de hashear

2. **Implementar l�mite de intentos de login:**
   - Bloquear cuenta despu�s de N intentos fallidos
   - Implementar CAPTCHA despu�s de X intentos

3. **Agregar autenticaci�n de dos factores (2FA)**

4. **Implementar reseteo seguro de contrase�as:**
   - Enviar token �nico por email
   - Token con expiraci�n
   - No mostrar si el usuario existe o no

5. **Logging de intentos de login:**
   - Registrar intentos fallidos
   - Detectar patrones sospechosos

## ?? Referencias

- [BCrypt.Net-Next Documentation](https://github.com/BcryptNet/bcrypt.net)
- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
