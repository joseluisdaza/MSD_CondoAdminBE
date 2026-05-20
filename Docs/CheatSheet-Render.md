# 📋 Cheat Sheet - Deployment Render

## ⚡ Comandos Rápidos

### Generar JWT Secret

```powershell
.\prepare-render.ps1 -GenerateJWT
```

### Verificar Docker

```powershell
.\prepare-render.ps1 -CheckDocker
```

### Test Build Local

```powershell
.\prepare-render.ps1 -TestLocal
```

### Probar API Desplegada

```powershell
.\prepare-render.ps1 -TestConnection -RenderURL "https://tu-api.onrender.com"
```

---

## 🔑 Variables de Entorno para Render

```
DB_SERVER=sql.freesqldatabase.com
DB_NAME=sql12XXXXXX
DB_USER=sql12XXXXXX
DB_PASSWORD=XXXXXXXXXX
JWT_SECRET_KEY=[64 caracteres aleatorios]
CORS_ALLOWED_ORIGIN=*
LOG_PATH=/tmp/logs/log-.txt
ASPNETCORE_ENVIRONMENT=Production
```

---

## 🌐 URLs Importantes

| Servicio             | URL                                 |
| -------------------- | ----------------------------------- |
| **Render Dashboard** | https://dashboard.render.com/       |
| **FreeSQLDatabase**  | https://www.freesqldatabase.com/    |
| **phpMyAdmin**       | https://www.phpmyadmin.co/          |
| **Tu API**           | https://tu-api.onrender.com         |
| **Swagger**          | https://tu-api.onrender.com/swagger |

---

## 🗄️ Configuración MySQL

### FreeSQLDatabase

```
Server: sql.freesqldatabase.com
Port: 3306
Database: sql12XXXXXX
Username: sql12XXXXXX
Password: XXXXXXXXXX
phpMyAdmin: https://www.phpmyadmin.co/
```

### db4free

```
Server: db4free.net
Port: 3306
Database: [tu_username]
Username: [tu_username]
Password: [tu_password]
```

---

## 🧪 Tests Rápidos

### Test Login

```powershell
$body = @{
    email = "admin@admin.com"
    password = "Admin123"
} | ConvertTo-Json

$response = Invoke-RestMethod `
    -Uri "https://tu-api.onrender.com/api/authentication/login" `
    -Method POST `
    -Body $body `
    -ContentType "application/json"

$response.token
```

### Test Endpoint Protegido

```powershell
$headers = @{ "Authorization" = "Bearer $($response.token)" }

Invoke-RestMethod `
    -Uri "https://tu-api.onrender.com/api/users" `
    -Headers $headers
```

---

## 📊 Configuración Render

### Web Service Settings

```
Name: condominio-api
Runtime: Docker
Dockerfile Path: ./CondominioAPI/Dockerfile
Docker Context: ./CondominioAPI
Branch: main
Instance Type: Free
```

---

## 🚨 Troubleshooting Rápido

### Build Falla

```
1. Verificar Dockerfile existe
2. Revisar logs en Render
3. Verificar rama en GitHub
```

### API no Responde

```
1. Esperar 1-2 min (primera startup)
2. Verificar variables de entorno
3. Revisar logs en Render
```

### Error MySQL

```
1. Verificar credenciales
2. Probar en phpMyAdmin
3. Verificar schema importado
```

---

## 📝 Checklist Deploy

- [ ] Cuenta Render creada
- [ ] MySQL configurado
- [ ] JWT secret generado
- [ ] Variables env configuradas
- [ ] Build exitoso
- [ ] API responde
- [ ] Login funciona
- [ ] Endpoints protegidos OK

---

## 🔒 Seguridad

### Archivos a NO subir a GitHub

```
.env
.env.local
.env.render
*.env (excepto .example)
```

### Verificar .gitignore

```powershell
git status
# NO debe aparecer: .env, .env.render
```

---

## ⏱️ Tiempos Esperados

| Actividad                            | Tiempo    |
| ------------------------------------ | --------- |
| Crear MySQL                          | 5-10 min  |
| Configurar Render                    | 10 min    |
| Build inicial                        | 5-10 min  |
| Primera petición (después de dormir) | 30-60 seg |
| Peticiones normales                  | <2 seg    |

---

## 💰 Costos

### Free Tier

```
Render Web Service: $0/mes
FreeSQLDatabase: $0/mes
Total: $0/mes
```

### Limitaciones Free

```
- Se duerme tras 15 min inactividad
- 750 horas/mes
- 512 MB RAM
- 50 MB database
```

---

## 📞 Recursos

| Recurso           | Ubicación                        |
| ----------------- | -------------------------------- |
| **Quick Start**   | Docs/QuickStart-Render.md        |
| **Guía Completa** | Docs/DeployRender.md             |
| **MySQL Gratis**  | Docs/MySQL-Gratuito.md           |
| **Checklist**     | Docs/Checklist-PostDeployment.md |

---

## 🎯 Pasos Siguientes Post-Deploy

1. Actualizar frontend con nueva URL
2. Configurar CORS apropiado
3. Crear usuarios iniciales
4. Hacer backup de database
5. Monitorear logs regularmente

---

**Imprime esta hoja para tenerla a mano durante el deployment! 📄**
