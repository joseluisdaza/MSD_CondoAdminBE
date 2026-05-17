# 🚀 Guía de Deployment en Render - Condominio API

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Requisitos Previos](#requisitos-previos)
3. [Paso 1: Configurar Base de Datos MySQL (Gratuita)](#paso-1-configurar-base-de-datos-mysql-gratuita)
4. [Paso 2: Preparar el Repositorio](#paso-2-preparar-el-repositorio)
5. [Paso 3: Crear Cuenta en Render](#paso-3-crear-cuenta-en-render)
6. [Paso 4: Configurar Web Service en Render](#paso-4-configurar-web-service-en-render)
7. [Paso 5: Verificar Deployment](#paso-5-verificar-deployment)
8. [Solución de Problemas](#solución-de-problemas)
9. [Costos](#costos)

---

## Descripción General

Esta guía te mostrará cómo desplegar tu **Condominio API** (.NET 8 + MySQL) en **Render** de forma **gratuita**.

**Arquitectura:**

- **Backend**: Render Web Service (Contenedor Docker) - **GRATIS**
- **Base de Datos**: FreeSQLDatabase (MySQL gratuito) - **GRATIS**
- **Total**: **$0/mes** 💰

**Limitaciones del plan gratuito:**

- El servicio se "duerme" después de 15 minutos de inactividad
- Primera petición después de dormir tarda ~30-60 segundos
- 750 horas/mes de ejecución (suficiente para uso personal/demo)
- 512 MB RAM

---

## Requisitos Previos

### ✅ Checklist

- [ ] Cuenta de GitHub con tu repositorio `MSD_CondoAdminBE`
- [ ] El código debe estar en un repositorio **público** o conectar Render a GitHub
- [ ] Tener un Dockerfile funcional (✅ ya lo tienes)
- [ ] Correo electrónico para crear cuenta en Render

### 📦 Software (solo para pruebas locales)

- Docker Desktop (opcional, para probar antes de subir)
- Git

---

## Paso 1: Configurar Base de Datos MySQL (Gratuita)

Render NO ofrece MySQL gratuito, pero hay alternativas:

### Opción A: FreeSQLDatabase (Recomendada) ⭐

**Características:**

- 100% GRATIS
- 50 MB de almacenamiento
- MySQL 8.0
- Suficiente para desarrollo/demo

**Pasos:**

1. **Ir a FreeSQLDatabase**
   - Web: https://www.freesqldatabase.com/
   - Click en "Create Free MySQL Database"

2. **Crear base de datos**

   ```
   Database Name: condominioXXXX (te asignarán un nombre único)
   ```

3. **Guardar credenciales** (recibirás por email):

   ```
   Server: sql.freesqldatabase.com
   Database: sql12XXXXXX
   Username: sql12XXXXXX
   Password: XXXXXXXXXX
   Port: 3306
   ```

4. **Acceder a phpMyAdmin** (incluido)
   - URL: https://www.phpmyadmin.co/
   - Usar las credenciales recibidas

5. **Importar el schema**
   - En phpMyAdmin, ir a "Import"
   - Subir tu archivo `Database/CreateDB.sql`
   - Ejecutar

### Opción B: db4free.net (Alternativa)

**Características:**

- Gratuito
- 200 MB de almacenamiento
- Menos estable que FreeSQLDatabase

**Pasos:**

1. Ir a https://db4free.net/
2. Registrarse
3. Crear base de datos
4. Guardar credenciales:
   ```
   Server: db4free.net
   Port: 3306
   Database: tu_nombre_db
   User: tu_usuario
   Password: tu_password
   ```

### Opción C: Railway ($5 crédito inicial)

Si ninguna opción gratuita funciona:

1. Ir a https://railway.app/
2. Crear cuenta con GitHub
3. New Project → Provision MySQL
4. Te dan $5 de crédito inicial (dura ~2-3 meses)

---

## Paso 2: Preparar el Repositorio

### 2.1 Verificar Dockerfile

Tu `Dockerfile` ya está configurado correctamente. Solo verifica que esté en:

```
CondominioAPI/Dockerfile
```

### 2.2 Crear archivo Render.yaml (Opcional pero Recomendado)

Este archivo automatiza la configuración:

**Crear:** `render.yaml` en la raíz del proyecto:

```yaml
services:
  - type: web
    name: condominio-api
    runtime: docker
    dockerfilePath: ./CondominioAPI/Dockerfile
    dockerContext: ./CondominioAPI
    envVars:
      - key: DB_SERVER
        value: sql.freesqldatabase.com
      - key: DB_NAME
        sync: false
      - key: DB_USER
        sync: false
      - key: DB_PASSWORD
        sync: false
      - key: JWT_SECRET_KEY
        generateValue: true
        sync: false
      - key: CORS_ALLOWED_ORIGIN
        value: "*"
      - key: LOG_PATH
        value: /tmp/logs/log-.txt
      - key: ASPNETCORE_ENVIRONMENT
        value: Production
    healthCheckPath: /api/users
```

### 2.3 Subir cambios a GitHub

```bash
git add .
git commit -m "Add Render deployment configuration"
git push origin main
```

---

## Paso 3: Crear Cuenta en Render

1. **Ir a Render**
   - Web: https://render.com/
   - Click en "Get Started for Free"

2. **Registrarse con GitHub**
   - Recomendado: "Sign up with GitHub"
   - Autorizar acceso a Render

3. **Verificar email**
   - Revisar inbox y confirmar cuenta

---

## Paso 4: Configurar Web Service en Render

### 4.1 Crear Nuevo Web Service

1. **En Dashboard de Render**
   - Click en "New +" (esquina superior derecha)
   - Seleccionar "Web Service"

2. **Conectar Repositorio**
   - Seleccionar tu repositorio: `MSD_CondoAdminBE`
   - Si no aparece, click en "Configure account" y dar permisos

3. **Configurar Service**

   ```
   Name: condominio-api

   Region: Oregon (US West) o el más cercano

   Branch: main

   Runtime: Docker

   Dockerfile Path: ./CondominioAPI/Dockerfile

   Docker Context: ./CondominioAPI

   Instance Type: Free
   ```

### 4.2 Configurar Variables de Entorno

**IMPORTANTE:** Click en "Advanced" y agregar las siguientes variables:

| Key                      | Value                                            | Ejemplo                  |
| ------------------------ | ------------------------------------------------ | ------------------------ |
| `DB_SERVER`              | `sql.freesqldatabase.com`                        | Tu servidor MySQL        |
| `DB_NAME`                | `sql12345678`                                    | Tu database name         |
| `DB_USER`                | `sql12345678`                                    | Tu usuario               |
| `DB_PASSWORD`            | `tu_password`                                    | Tu contraseña MySQL      |
| `JWT_SECRET_KEY`         | `tu_clave_secreta_jwt_minimo_32_caracteres_aqui` | Genera una clave segura  |
| `CORS_ALLOWED_ORIGIN`    | `*`                                              | O tu dominio específico  |
| `LOG_PATH`               | `/tmp/logs/log-.txt`                             | Ruta para logs en Render |
| `ASPNETCORE_ENVIRONMENT` | `Production`                                     | Ambiente de .NET         |

**Generar JWT_SECRET_KEY segura:**

```bash
# En PowerShell:
-join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
```

### 4.3 Health Check (Opcional pero Recomendado)

```
Health Check Path: /api/users
```

Esto ayuda a Render a saber si tu API está funcionando.

### 4.4 Crear Service

- Click en **"Create Web Service"**
- Render comenzará a construir tu Docker image
- Este proceso toma **5-10 minutos** la primera vez

---

## Paso 5: Verificar Deployment

### 5.1 Monitorear Build

En el dashboard verás:

```
Building... (esto toma 5-10 min)
  ↓
Deploying...
  ↓
Live ✅
```

### 5.2 Obtener URL

Una vez desplegado, verás:

```
https://condominio-api.onrender.com
```

Esta es tu URL pública.

### 5.3 Probar la API

#### Opción 1: Navegador

Abrir en navegador:

```
https://condominio-api.onrender.com/swagger
```

Deberías ver Swagger UI.

#### Opción 2: PowerShell

```powershell
# Probar endpoint de salud
Invoke-RestMethod -Uri "https://condominio-api.onrender.com/api/users" -Method GET

# Login de prueba (si tienes usuarios en DB)
$body = @{
    email = "admin@admin.com"
    password = "Admin123"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://condominio-api.onrender.com/api/authentication/login" -Method POST -Body $body -ContentType "application/json"
```

#### Opción 3: Postman

1. Importar colección (si la tienes)
2. Cambiar base URL a: `https://condominio-api.onrender.com`
3. Probar endpoints

---

## Solución de Problemas

### ❌ Error: "Build failed"

**Causa:** Dockerfile mal configurado o falta de archivos.

**Solución:**

1. Verificar que `Dockerfile` esté en `CondominioAPI/Dockerfile`
2. Revisar logs de build en Render
3. Verificar que la rama en GitHub sea la correcta

### ❌ Error: "Connection to MySQL failed"

**Causa:** Variables de entorno incorrectas o DB no accesible.

**Solución:**

1. Verificar variables de entorno en Render:
   - DB_SERVER
   - DB_NAME
   - DB_USER
   - DB_PASSWORD
2. Probar conexión desde otra herramienta (ej: MySQL Workbench)
3. Verificar que FreeSQLDatabase permita conexiones externas

### ❌ API responde lento la primera vez

**Causa:** Plan gratuito de Render "duerme" el servicio después de 15 min de inactividad.

**Solución:**

- Primera petición tarda 30-60 segundos (normal)
- Peticiones subsecuentes son rápidas
- Para evitar esto: upgrade a plan pago ($7/mes)

### ❌ Error 500 al hacer requests

**Causa:** Error en la aplicación.

**Solución:**

1. Revisar logs en Render Dashboard → Tu Service → Logs
2. Buscar stack trace del error
3. Verificar que la base de datos tenga el schema correcto

### 🔍 Ver Logs en Tiempo Real

En Render Dashboard:

```
Tu Service → Logs (tab)
```

Verás logs de:

- Build
- Deployment
- Runtime (errores de tu aplicación)

---

## Costos

### Plan Actual (GRATIS)

| Recurso               | Costo      | Límites                              |
| --------------------- | ---------- | ------------------------------------ |
| Render Web Service    | $0         | 750 horas/mes, se duerme tras 15 min |
| FreeSQLDatabase MySQL | $0         | 50 MB, limitado                      |
| **TOTAL**             | **$0/mes** | Ideal para desarrollo/demo           |

### Para Producción (Recomendado)

Si necesitas algo más robusto:

| Recurso        | Costo        | Beneficios                 |
| -------------- | ------------ | -------------------------- |
| Render Starter | $7/mes       | Siempre activo, 512 MB RAM |
| Railway MySQL  | ~$5/mes      | 1 GB SSD, backups          |
| **TOTAL**      | **~$12/mes** | Producción pequeña         |

### Alternativas 100% Gratuitas

Si Render/FreeSQLDatabase no funciona:

1. **Railway** - $5 crédito inicial (dura 2-3 meses)
2. **Fly.io** - Plan gratuito generoso
3. **Azure App Service** - $200 crédito inicial (estudiantes)

---

## 🎉 ¡Deployment Exitoso!

Tu API ahora está en:

```
https://condominio-api.onrender.com
```

### Próximos Pasos

1. **Conectar tu Frontend**
   - Actualizar variable `VITE_API_URL` con tu nueva URL
2. **Configurar CORS**
   - Actualizar `CORS_ALLOWED_ORIGIN` con tu dominio frontend

3. **Monitoreo**
   - Revisar logs regularmente en Render
   - Configurar alerts (plan pago)

4. **Backup de Base de Datos**
   - Exportar SQL regularmente desde phpMyAdmin

---

## 📚 Recursos Adicionales

- [Documentación Render](https://render.com/docs)
- [Render + Docker](https://render.com/docs/docker)
- [FreeSQLDatabase FAQ](https://www.freesqldatabase.com/faq/)
- [MySQL en Producción](https://dev.mysql.com/doc/)

---

## 🆘 Soporte

Si tienes problemas:

1. Revisar logs en Render
2. Verificar variables de entorno
3. Probar conexión MySQL desde herramienta externa
4. Revisar GitHub Actions (si los tienes)

**Tiempo estimado total: 30-45 minutos** ⏱️
