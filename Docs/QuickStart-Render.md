# 🚀 Quick Start - Deploy a Render

## ⏱️ Tiempo Total: 30-45 minutos

Esta guía rápida te llevará paso a paso por el deployment de tu Condominio API en Render.

---

## 📋 Checklist Pre-Deployment

Antes de empezar, asegúrate de tener:

- [ ] Cuenta de GitHub con el repositorio
- [ ] Correo electrónico para crear cuentas
- [ ] 30-45 minutos de tiempo disponible

---

## 🎯 Pasos Rápidos

### PASO 1: Configurar MySQL (15 min)

1. **Ir a FreeSQLDatabase**
   - URL: https://www.freesqldatabase.com/
   - Click "Create Free MySQL Database"
2. **Completar formulario**
   ```
   Tu email: tu_email@example.com
   ```
3. **Guardar credenciales del email** (llegarán en ~1 min)

   ```
   Server: sql.freesqldatabase.com
   Database: sql12XXXXXX
   Username: sql12XXXXXX
   Password: XXXXXXXXXX
   ```

4. **Importar schema**
   - Ir a: https://www.phpmyadmin.co/
   - Login con credenciales
   - Import → Choose File → `CondominioAPI/Database/CreateDB.sql`
   - Go

✅ **MySQL listo!**

---

### PASO 2: Crear Cuenta en Render (5 min)

1. **Ir a Render**
   - URL: https://render.com/
   - Click "Get Started for Free"

2. **Sign up con GitHub**
   - Autorizar acceso

3. **Verificar email**
   - Check inbox

✅ **Cuenta Render lista!**

---

### PASO 3: Generar JWT Secret (1 min)

Ejecuta en PowerShell:

```powershell
.\prepare-render.ps1 -GenerateJWT
```

O manualmente:

```powershell
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

**Copia y guarda la clave generada** ✏️

---

### PASO 4: Deploy en Render (10-15 min)

1. **Crear Web Service**
   - Dashboard → "New +" → "Web Service"
   - Seleccionar tu repo `MSD_CondoAdminBE`

2. **Configurar servicio**

   ```
   Name: condominio-api
   Branch: main
   Runtime: Docker
   Dockerfile Path: ./CondominioAPI/Dockerfile
   Docker Context: ./CondominioAPI
   Instance Type: Free
   ```

3. **Agregar Variables de Entorno** (click "Advanced")

   | Key                      | Value                        |
   | ------------------------ | ---------------------------- |
   | `DB_SERVER`              | `sql.freesqldatabase.com`    |
   | `DB_NAME`                | `sql12XXXXXX` ← tu database  |
   | `DB_USER`                | `sql12XXXXXX` ← tu usuario   |
   | `DB_PASSWORD`            | `XXXXXXXXXX` ← tu password   |
   | `JWT_SECRET_KEY`         | ← la que generaste en Paso 3 |
   | `CORS_ALLOWED_ORIGIN`    | `*`                          |
   | `LOG_PATH`               | `/tmp/logs/log-.txt`         |
   | `ASPNETCORE_ENVIRONMENT` | `Production`                 |

4. **Crear Web Service**
   - Click "Create Web Service"
   - Esperar 5-10 min (build + deploy)

✅ **API desplegada!**

---

### PASO 5: Verificar (2 min)

1. **Obtener URL**

   ```
   https://condominio-api.onrender.com
   ```

2. **Probar en navegador**

   ```
   https://condominio-api.onrender.com/swagger
   ```

3. **Probar con PowerShell**
   ```powershell
   .\prepare-render.ps1 -TestConnection -RenderURL "https://condominio-api.onrender.com"
   ```

✅ **¡Deployment completo!** 🎉

---

## 🔧 Si algo sale mal

### Build falla

- Verificar que `Dockerfile` esté en `CondominioAPI/Dockerfile`
- Revisar logs en Render

### API no responde

- Verificar variables de entorno
- Revisar logs: Dashboard → Tu servicio → Logs
- Esperar 1-2 min (primera startup es lenta)

### Error de conexión MySQL

- Verificar credenciales en variables de entorno
- Probar conexión desde phpMyAdmin
- Verificar que importaste el schema

---

## 📚 Documentación Completa

Para más detalles, ver:

- **Guía completa:** [DeployRender.md](DeployRender.md)
- **MySQL gratuito:** [MySQL-Gratuito.md](MySQL-Gratuito.md)
- **Variables de entorno:** [.env.render.example](../.env.render.example)

---

## 🎯 Próximos Pasos

Después del deployment:

1. **Conectar tu frontend**
   - Actualizar `VITE_API_URL` con tu URL de Render
2. **Configurar CORS apropiado**
   - Cambiar `CORS_ALLOWED_ORIGIN` de `*` a tu dominio
3. **Crear usuarios iniciales**
   - Usar phpMyAdmin o tu frontend

4. **Backup de database**
   - Exportar regularmente desde phpMyAdmin

---

## 💰 Costos

- **Render Web Service:** $0/mes (plan free)
- **FreeSQLDatabase:** $0/mes
- **Total:** **$0/mes** 🎉

**Limitaciones:**

- Servicio se duerme tras 15 min de inactividad
- Primera petición tarda ~30-60 segundos
- 50 MB de database

**Para producción:** Considera upgrade a Render Starter ($7/mes) + Railway MySQL ($5/mes)

---

## 🆘 Ayuda

Si necesitas ayuda:

1. Revisar [Troubleshooting](DeployRender.md#solución-de-problemas)
2. Verificar logs en Render Dashboard
3. Usar el script de pruebas: `.\prepare-render.ps1`

---

**¡Buena suerte con tu deployment!** 🚀
