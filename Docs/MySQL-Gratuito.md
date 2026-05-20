# 🗄️ Guía: Configurar MySQL Gratuito para Render

## 📋 Resumen

Esta guía cubre las opciones de MySQL gratuito para usar con tu deployment en Render.

---

## Opción 1: FreeSQLDatabase ⭐ (Recomendada)

### Características

- ✅ 100% Gratuito
- ✅ 50 MB almacenamiento
- ✅ MySQL 8.0
- ✅ Incluye phpMyAdmin
- ✅ No requiere tarjeta de crédito
- ⚠️ Limitado a proyectos pequeños/demos

### Paso a Paso

#### 1. Crear Base de Datos

1. Ir a: https://www.freesqldatabase.com/
2. Click en **"Create Free MySQL Database"**
3. Llenar formulario:
   ```
   Database Name: condominioXXXX (se auto-genera)
   Your Email: tu_email@example.com
   ```
4. Click en **"Create Database"**

#### 2. Recibir Credenciales

Recibirás un email con:

```
Database Information:

Server: sql.freesqldatabase.com
Name: sql12XXXXXX
Username: sql12XXXXXX
Password: XXXXXXXXXX
Port: 3306
PhpMyAdmin: https://www.phpmyadmin.co/
```

**⚠️ IMPORTANTE:** Guarda estas credenciales, las necesitarás para Render.

#### 3. Acceder a phpMyAdmin

1. Ir a: https://www.phpmyadmin.co/
2. Ingresar credenciales:
   ```
   Server: sql.freesqldatabase.com
   Username: sql12XXXXXX (tu username)
   Password: XXXXXXXXXX (tu password)
   ```
3. Click en **"Log in"**

#### 4. Importar Schema de Base de Datos

Opción A: **Importar SQL (Recomendado)**

1. En phpMyAdmin, click en tu base de datos (left sidebar)
2. Click en tab **"Import"**
3. Click en **"Choose File"**
4. Seleccionar: `MSD_CondoAdminBE/CondominioAPI/Database/CreateDB.sql`
5. Click en **"Go"**

Opción B: **Ejecutar SQL Manualmente**

1. En phpMyAdmin, click en tab **"SQL"**
2. Copiar contenido de `CreateDB.sql`
3. Pegar en el editor SQL
4. Click en **"Go"**

#### 5. Verificar Tablas Creadas

1. Click en tu database (left sidebar)
2. Deberías ver todas las tablas:
   ```
   - Users
   - Roles
   - UserRoles
   - Properties
   - PropertyTypes
   - PropertyOwners
   - Expenses
   - ExpenseCategories
   - Payments
   - PaymentStatuses
   - ExpensePayments
   - ServiceTypes
   - ServiceExpenses
   - ServicePayments
   - ServiceExpensePayments
   - Resources
   - ResourceCosts
   - ResourceBookings
   - IncidentTypes
   - Incidents
   - IncidentCosts
   - DatabaseVersion
   ```

#### 6. Crear Usuario Inicial (Opcional)

Si tu `CreateDB.sql` no incluye usuarios por default:

```sql
-- En phpMyAdmin → SQL tab
INSERT INTO Users (Email, Password, Name, LastName, PhoneNumber, CreatedAt, CreatedBy, IsDeleted)
VALUES
('admin@admin.com',
 -- Password hasheada para "Admin123"
 '$2a$11$XhKxZ...',
 'Admin',
 'System',
 '1234567890',
 NOW(),
 1,
 0);

-- Obtener el UserID generado
SELECT LAST_INSERT_ID();

-- Asignar rol Admin (asumiendo RoleID=1 es Admin)
INSERT INTO UserRoles (UserId, RoleId, IsDeleted)
VALUES (LAST_INSERT_ID(), 1, 0);
```

#### 7. Configurar en Render

En Render Dashboard → Environment Variables:

```
DB_SERVER=sql.freesqldatabase.com
DB_NAME=sql12XXXXXX
DB_USER=sql12XXXXXX
DB_PASSWORD=XXXXXXXXXX
```

### Limitaciones

- **Almacenamiento:** 50 MB (suficiente para ~10,000 registros)
- **Conexiones:** Limitadas (puede fallar bajo carga alta)
- **Uptime:** No garantizado (puede caerse ocasionalmente)
- **Uso:** Solo desarrollo/demos, NO producción

---

## Opción 2: db4free.net

### Características

- ✅ 100% Gratuito
- ✅ 200 MB almacenamiento (más que FreeSQLDatabase)
- ✅ MySQL 8.0
- ⚠️ Menos estable
- ⚠️ Puede tener downtime

### Paso a Paso

#### 1. Registrarse

1. Ir a: https://db4free.net/
2. Click en **"Sign up"**
3. Llenar formulario:
   ```
   Desired username: tu_usuario (será el nombre de DB también)
   Password: tu_password_seguro
   Password (repeated): tu_password_seguro
   Email: tu_email@example.com
   ```
4. Aceptar términos
5. Click en **"Sign up"**

#### 2. Confirmar Email

1. Revisar inbox
2. Click en link de confirmación
3. Esperar aprobación (usualmente inmediato)

#### 3. Credenciales

```
Server: db4free.net
Port: 3306
Database: tu_usuario (mismo que username)
Username: tu_usuario
Password: tu_password
```

#### 4. Conectar con Cliente MySQL

Opción A: **MySQL Workbench**

1. Descargar: https://dev.mysql.com/downloads/workbench/
2. New Connection:
   ```
   Connection Name: db4free
   Hostname: db4free.net
   Port: 3306
   Username: tu_usuario
   Password: [Store in Keychain]
   ```
3. Test Connection
4. OK

Opción B: **HeidiSQL** (Windows)

1. Descargar: https://www.heidisql.com/download.php
2. New Session:
   ```
   Network type: MariaDB or MySQL (TCP/IP)
   Hostname: db4free.net
   User: tu_usuario
   Password: tu_password
   Port: 3306
   ```
3. Open

#### 5. Importar Schema

1. En tu cliente MySQL, seleccionar database
2. File → Run SQL file
3. Seleccionar `CreateDB.sql`
4. Execute

#### 6. Configurar en Render

```
DB_SERVER=db4free.net
DB_NAME=tu_usuario
DB_USER=tu_usuario
DB_PASSWORD=tu_password
```

### Limitaciones

- **Estabilidad:** Puede tener downtime frecuente
- **Velocidad:** Puede ser lento
- **Soporte:** Comunidad, no comercial

---

## Opción 3: Railway ($5 Crédito Inicial)

### Características

- 💰 $5 crédito inicial gratis
- ✅ MySQL confiable
- ✅ 1 GB almacenamiento
- ✅ Backups automáticos
- 💰 ~$5/mes después de crédito inicial

### Paso a Paso

#### 1. Crear Cuenta

1. Ir a: https://railway.app/
2. Click en **"Start a New Project"**
3. Sign up con GitHub
4. Autorizar Railway

#### 2. Crear Proyecto MySQL

1. En Dashboard: **"New Project"**
2. Seleccionar **"Provision MySQL"**
3. Esperar ~30 segundos

#### 3. Obtener Credenciales

1. Click en tu MySQL service
2. Tab **"Connect"**
3. Copiar credenciales:

   ```
   MYSQL_URL=mysql://root:password@host:port/railway

   O separadas:
   Host: containers-us-west-XX.railway.app
   Port: XXXX
   User: root
   Password: XXXXXXXXXX
   Database: railway
   ```

#### 4. Importar Schema

Opción A: **Usar Railway CLI**

```bash
# Instalar Railway CLI
npm i -g @railway/cli

# Login
railway login

# Link a tu proyecto
railway link

# Importar SQL
railway run mysql -u root -p railway < CondominioAPI/Database/CreateDB.sql
```

Opción B: **Usar Cliente MySQL**

Igual que db4free, pero con las credenciales de Railway.

#### 5. Configurar en Render

```
DB_SERVER=containers-us-west-XX.railway.app
DB_NAME=railway
DB_USER=root
DB_PASSWORD=XXXXXXXXXX
Port=XXXX (agregar a connection string si es diferente de 3306)
```

**Nota:** Si el puerto NO es 3306, modifica tu `Program.cs`:

```csharp
var connectionString = $"server={dbServer};port={dbPort};database={dbName};user={dbUser};password={dbPassword}";
```

Y agrega variable de entorno `DB_PORT` en Render.

### Costos

- **Crédito inicial:** $5 (gratis)
- **Uso promedio:** $3-5/mes
- **Crédito dura:** ~2 meses con uso moderado

---

## Comparación Rápida

| Feature              | FreeSQLDatabase | db4free | Railway             |
| -------------------- | --------------- | ------- | ------------------- |
| **Precio**           | Gratis          | Gratis  | $5 crédito → $5/mes |
| **Almacenamiento**   | 50 MB           | 200 MB  | 1 GB                |
| **Confiabilidad**    | ⭐⭐⭐          | ⭐⭐    | ⭐⭐⭐⭐⭐          |
| **Velocidad**        | Media           | Lenta   | Rápida              |
| **Backups**          | ❌              | ❌      | ✅                  |
| **Soporte**          | ❌              | ❌      | Comunidad           |
| **Recomendado para** | Demos           | Testing | Producción pequeña  |

---

## Recomendación Final

### Para empezar (100% gratis):

**FreeSQLDatabase** ⭐

### Si FreeSQLDatabase no funciona:

**db4free** (ten paciencia con downtime)

### Si necesitas algo confiable:

**Railway** ($5 inicial, luego ~$5/mes)

### Para producción seria:

**PlanetScale** (MySQL serverless, free tier generoso)
https://planetscale.com/

---

## Troubleshooting

### Error: "Can't connect to MySQL server"

**Causas comunes:**

1. Firewall bloqueando puerto 3306
2. Credenciales incorrectas
3. Servicio MySQL caído (db4free)

**Solución:**

- Verificar credenciales
- Probar desde otra red (ej: hotspot móvil)
- Esperar si el servicio está caído
- Usar Railway en su lugar

### Error: "Access denied for user"

**Causa:** Username o password incorrectos

**Solución:**

- Verificar copy/paste de credenciales
- Revisar email de confirmación
- Resetear password en el servicio

### Error: "Database ... doesn't exist"

**Causa:** Nombre de database incorrecto

**Solución:**

- En FreeSQLDatabase: usar `sql12XXXXXX` (el que te enviaron)
- En db4free: usar tu username como database name
- En Railway: usar `railway`

---

## Scripts Útiles

### Generar Password Hash para Usuario

```csharp
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

// En C# Interactive o mini programa
string password = "Admin123";
byte[] salt = new byte[128 / 8];
using (var rng = RandomNumberGenerator.Create())
{
    rng.GetBytes(salt);
}

string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
    password: password,
    salt: salt,
    prf: KeyDerivationPrf.HMACSHA256,
    iterationCount: 10000,
    numBytesRequested: 256 / 8));

Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hashed: {hashed}");
```

### Backup Manual de Database

```sql
-- Exportar todas las tablas
mysqldump -h sql.freesqldatabase.com -u sql12XXXXXX -p sql12XXXXXX > backup.sql

-- O desde phpMyAdmin:
-- Export tab → Quick export → Go
```

---

## Próximos Pasos

Una vez configurada tu database:

1. ✅ Guardar credenciales de forma segura
2. ✅ Importar schema (`CreateDB.sql`)
3. ✅ Crear usuarios iniciales (si es necesario)
4. ✅ Configurar variables de entorno en Render
5. ✅ Desplegar tu API
6. ✅ Probar conexión

**Tiempo estimado: 15-20 minutos** ⏱️
