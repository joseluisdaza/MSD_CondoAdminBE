# ✅ Checklist Post-Deployment

Usa esta checklist para verificar que tu deployment esté funcionando correctamente.

---

## 🔍 Verificación Básica

### ✅ API está en línea

- [ ] URL de Render es accesible
- [ ] Responde en navegador
- [ ] No hay errores 502/503

**Probar:**

```
https://tu-api.onrender.com
```

### ✅ Swagger UI funciona (si está habilitado)

- [ ] Swagger UI carga correctamente
- [ ] Muestra todos los endpoints
- [ ] Puede expandir endpoints

**URL:**

```
https://tu-api.onrender.com/swagger
```

---

## 🗄️ Base de Datos

### ✅ Conexión a MySQL

- [ ] API puede conectar a MySQL
- [ ] No hay errores de conexión en logs

**Verificar en Render Logs:**

```
Buscar: "Connection"
NO debe aparecer: "Connection failed" o "Access denied"
```

### ✅ Tablas creadas

- [ ] Todas las tablas existen
- [ ] Schema correcto

**Verificar en phpMyAdmin:**

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

### ✅ Datos iniciales

- [ ] Tabla `Roles` tiene datos
- [ ] Al menos 1 usuario admin existe (si es necesario)

**SQL para verificar:**

```sql
SELECT * FROM Roles;
SELECT * FROM Users WHERE IsDeleted = 0;
```

---

## 🔐 Autenticación

### ✅ JWT funciona

- [ ] Endpoint de login responde
- [ ] Retorna token JWT válido
- [ ] Token puede usarse en peticiones

**Probar con PowerShell:**

```powershell
$body = @{
    email = "admin@admin.com"
    password = "Admin123"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/authentication/login" -Method POST -Body $body -ContentType "application/json"

# Debe retornar un token
$response.token
```

### ✅ Autorización funciona

- [ ] Endpoints protegidos requieren token
- [ ] Sin token retorna 401 Unauthorized
- [ ] Con token válido retorna 200 OK

**Probar endpoint protegido sin token:**

```powershell
# Debe fallar con 401
Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/users" -Method GET
```

**Probar con token:**

```powershell
$headers = @{
    "Authorization" = "Bearer $($response.token)"
}

# Debe funcionar
Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/users" -Method GET -Headers $headers
```

---

## 🌐 CORS

### ✅ CORS configurado

- [ ] Frontend puede hacer peticiones
- [ ] No hay errores de CORS en consola del navegador

**Probar desde navegador (frontend):**

```javascript
fetch("https://tu-api.onrender.com/api/users")
  .then((res) => res.json())
  .then((data) => console.log(data))
  .catch((err) => console.error(err));
```

**NO debe aparecer:**

```
Access to fetch at '...' from origin '...' has been blocked by CORS policy
```

---

## 📊 Endpoints Principales

### ✅ GET /api/users

- [ ] Retorna lista de usuarios
- [ ] Formato JSON correcto
- [ ] Requiere autenticación

```powershell
$headers = @{ "Authorization" = "Bearer $token" }
Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/users" -Headers $headers
```

### ✅ GET /api/property

- [ ] Retorna lista de propiedades
- [ ] Formato JSON correcto

```powershell
Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/property" -Headers $headers
```

### ✅ GET /api/expenses

- [ ] Retorna lista de gastos
- [ ] Formato JSON correcto

```powershell
Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/expenses" -Headers $headers
```

### ✅ POST /api/users (Crear usuario)

- [ ] Puede crear nuevo usuario
- [ ] Validación funciona
- [ ] Retorna usuario creado

```powershell
$newUser = @{
    email = "test@test.com"
    password = "Test123!"
    name = "Test"
    lastName = "User"
    phoneNumber = "1234567890"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/users" -Method POST -Body $newUser -ContentType "application/json" -Headers $headers
```

---

## 🔧 Variables de Entorno

### ✅ Todas las variables están configuradas

Verificar en Render Dashboard → Environment:

- [ ] `DB_SERVER`
- [ ] `DB_NAME`
- [ ] `DB_USER`
- [ ] `DB_PASSWORD`
- [ ] `JWT_SECRET_KEY`
- [ ] `CORS_ALLOWED_ORIGIN`
- [ ] `LOG_PATH`
- [ ] `ASPNETCORE_ENVIRONMENT`

### ✅ Valores correctos

- [ ] DB_SERVER = servidor MySQL correcto
- [ ] DB_NAME = nombre de database correcto
- [ ] DB_USER = usuario correcto
- [ ] DB_PASSWORD = contraseña correcta (sin espacios extra)
- [ ] JWT_SECRET_KEY = mínimo 32 caracteres
- [ ] CORS_ALLOWED_ORIGIN = `*` o dominio frontend
- [ ] ASPNETCORE_ENVIRONMENT = `Production`

---

## 📝 Logs

### ✅ Logs en Render

- [ ] Logs se generan correctamente
- [ ] No hay errores críticos
- [ ] No hay warnings preocupantes

**Revisar en Render Dashboard → Logs:**

**Buscar ERRORES:**

```
Error
Exception
Failed
```

**Buscar WARNINGS:**

```
Warning
```

**Logs esperados (OK):**

```
Application started
Now listening on: http://[::]:8080
```

---

## ⚡ Rendimiento

### ✅ Primera petición

- [ ] Primera petición después de dormir tarda ~30-60 seg (normal en free tier)
- [ ] Peticiones subsecuentes son rápidas (<2 seg)

### ✅ Tiempo de respuesta

- [ ] GET requests: < 2 segundos
- [ ] POST requests: < 3 segundos

**Medir tiempo:**

```powershell
Measure-Command {
    Invoke-RestMethod -Uri "https://tu-api.onrender.com/api/users" -Headers $headers
}
```

---

## 🔒 Seguridad

### ✅ HTTPS

- [ ] URL usa HTTPS (no HTTP)
- [ ] Certificado SSL válido

**Verificar:**

```
https://tu-api.onrender.com
(debe tener candado verde en navegador)
```

### ✅ Secrets no expuestos

- [ ] `.env` NO está en GitHub
- [ ] Passwords NO están en código fuente
- [ ] JWT secret NO está hardcodeado

**Verificar en GitHub:**

```
Buscar archivos: .env, .env.render
NO deben existir en repo (deben estar en .gitignore)
```

### ✅ Endpoints protegidos

- [ ] Endpoints sensibles requieren autenticación
- [ ] Endpoints admin solo accesibles por admin

---

## 📱 Integración con Frontend

### ✅ Frontend puede conectarse

- [ ] Actualizar variable `VITE_API_URL` (o similar)
- [ ] Frontend puede hacer login
- [ ] Frontend puede obtener datos

**En tu frontend (.env):**

```
VITE_API_URL=https://tu-api.onrender.com
```

### ✅ Peticiones funcionan

- [ ] Login funciona
- [ ] Listar usuarios funciona
- [ ] Crear/editar/eliminar funciona

---

## 🚨 Monitoreo Continuo

### ✅ Configurar alertas (opcional)

- [ ] Render Dashboard → Settings → Notifications
- [ ] Email para deployment failures
- [ ] Email para service down

### ✅ Backup de database

- [ ] Exportar database regularmente
- [ ] Guardar backup local

**Exportar desde phpMyAdmin:**

```
Export → Quick export → Go
Guardar archivo .sql
```

### ✅ Documentar credenciales

- [ ] Guardar credenciales MySQL en lugar seguro
- [ ] Guardar URL de API
- [ ] Guardar accesos a Render

---

## 📊 Checklist Completo

| Categoría               | Items  | Completados   |
| ----------------------- | ------ | ------------- |
| ✅ Verificación Básica  | 3      | \_\_ / 3      |
| 🗄️ Base de Datos        | 4      | \_\_ / 4      |
| 🔐 Autenticación        | 2      | \_\_ / 2      |
| 🌐 CORS                 | 1      | \_\_ / 1      |
| 📊 Endpoints            | 4      | \_\_ / 4      |
| 🔧 Variables de Entorno | 2      | \_\_ / 2      |
| 📝 Logs                 | 1      | \_\_ / 1      |
| ⚡ Rendimiento          | 2      | \_\_ / 2      |
| 🔒 Seguridad            | 3      | \_\_ / 3      |
| 📱 Frontend             | 2      | \_\_ / 2      |
| 🚨 Monitoreo            | 3      | \_\_ / 3      |
| **TOTAL**               | **27** | **\_\_ / 27** |

---

## 🎯 Siguiente Paso

Una vez completada esta checklist:

1. **Si todo funciona (24-27/27):** ✅ ¡Deployment exitoso!
2. **Si hay problemas (< 20/27):** Ver [Troubleshooting](DeployRender.md#solución-de-problemas)
3. **Conectar frontend:** Actualizar URL y probar integración completa

---

## 🆘 Si algo falla

### Logs en Render

```
Render Dashboard → Tu servicio → Logs
```

### Logs de build

```
Render Dashboard → Tu servicio → Events → Build logs
```

### Verificar variables de entorno

```
Render Dashboard → Tu servicio → Environment
```

### Probar conexión MySQL

```
phpMyAdmin: https://www.phpmyadmin.co/
```

### Script de diagnóstico

```powershell
.\prepare-render.ps1 -TestConnection -RenderURL "https://tu-api.onrender.com"
```

---

**Tiempo estimado para completar checklist: 15-20 minutos** ⏱️
