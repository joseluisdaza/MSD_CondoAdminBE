# 🗺️ Mapa Visual del Deployment

## 📊 Flujo Completo de Deployment

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEPLOYMENT EN RENDER                         │
│                         (30-45 MIN)                             │
└─────────────────────────────────────────────────────────────────┘

FASE 1: PREPARACIÓN (10 min)
────────────────────────────
┌──────────────┐
│ GitHub Repo  │
│   Listo      │◄── Código subido a GitHub
└──────┬───────┘
       │
       ▼
┌──────────────┐
│ Generar JWT  │
│   Secret     │◄── .\prepare-render.ps1 -GenerateJWT
└──────┬───────┘
       │
       ▼
┌──────────────┐
│ Crear Cuenta │
│   Render     │◄── https://render.com/
└──────────────┘


FASE 2: BASE DE DATOS (15 min)
───────────────────────────────
┌──────────────────────┐
│  FreeSQLDatabase     │◄── https://www.freesqldatabase.com/
│  Crear Database      │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Recibir             │
│  Credenciales Email  │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  phpMyAdmin          │◄── https://www.phpmyadmin.co/
│  Login + Import SQL  │
└──────────────────────┘


FASE 3: RENDER CONFIG (15 min)
───────────────────────────────
┌──────────────────────┐
│  New Web Service     │◄── Dashboard → New + → Web Service
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Conectar GitHub     │◄── Seleccionar MSD_CondoAdminBE
│  Repo                │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Configurar          │◄── Runtime: Docker
│  Service             │    Dockerfile: ./CondominioAPI/Dockerfile
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Variables de        │◄── 8 variables de entorno
│  Entorno             │    (DB_*, JWT_SECRET_KEY, etc.)
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Create Service      │◄── Click y esperar build
└──────────┬───────────┘
           │
           ▼ (5-10 min)
┌──────────────────────┐
│  Building...         │
│  Deploying...        │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  ✅ Live!            │◄── https://tu-api.onrender.com
└──────────────────────┘


FASE 4: VERIFICACIÓN (5 min)
─────────────────────────────
┌──────────────────────┐
│  Probar Swagger      │◄── /swagger
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Test Login          │◄── POST /api/authentication/login
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  Test Endpoints      │◄── GET /api/users (con token)
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│  ✅ Checklist        │◄── Docs/Checklist-PostDeployment.md
└──────────────────────┘
```

---

## 🏗️ Arquitectura Desplegada

```
┌────────────────────────────────────────────────────────────┐
│                        INTERNET                            │
└───────────────────────────┬────────────────────────────────┘
                            │
                            │ HTTPS
                            │
                            ▼
                ┌───────────────────────┐
                │   RENDER PLATFORM     │
                │  (Load Balancer +     │
                │   SSL Termination)    │
                └───────────┬───────────┘
                            │
                            ▼
            ┌───────────────────────────────┐
            │   CONDOMINIO API CONTAINER    │
            │   (Docker - .NET 8)           │
            │                               │
            │   - Controllers               │
            │   - JWT Auth                  │
            │   - Business Logic            │
            │   - EF Core                   │
            └───────────┬───────────────────┘
                        │
                        │ MySQL Protocol
                        │ (Port 3306)
                        │
                        ▼
        ┌───────────────────────────────────┐
        │   FREESQLDATABASE                 │
        │   (MySQL 8.0 - External)          │
        │                                   │
        │   - 50 MB Storage                 │
        │   - phpMyAdmin Access             │
        └───────────────────────────────────┘
```

---

## 🔄 Flujo de Request

```
CLIENTE (Frontend/Browser)
    │
    │ 1. HTTPS Request
    │    GET https://tu-api.onrender.com/api/users
    │
    ▼
RENDER LOAD BALANCER
    │
    │ 2. SSL Termination
    │    Forward to Container
    │
    ▼
API CONTAINER (.NET 8)
    │
    ├──► 3a. Authentication Middleware
    │        ├─ Verify JWT Token
    │        └─ Extract User Info
    │
    ├──► 3b. Authorization Middleware
    │        └─ Check User Roles
    │
    ├──► 3c. Controller (UsersController)
    │        └─ Business Logic
    │
    └──► 3d. Repository Layer
         │
         │ 4. Query Database
         │    SELECT * FROM Users WHERE IsDeleted = 0
         │
         ▼
    MYSQL DATABASE
         │
         │ 5. Return Data
         │
         ▼
API CONTAINER
    │
    │ 6. Convert to DTO
    │    Apply Soft Delete Filter
    │
    ▼
CLIENTE
    │
    └─ 7. JSON Response
       {
         "users": [...]
       }
```

---

## 📁 Estructura de Archivos de Deployment

```
MSD_CondoAdminBE/
│
├── 📄 README.md                      ← Actualizado con deployment
├── 📄 DEPLOYMENT_SUMMARY.md          ← Resumen de TODO
├── 📄 render.yaml                    ← Config automática Render
├── 📄 prepare-render.ps1             ← Script de utilidad
├── 📄 .env.render.example            ← Template de variables
├── 📄 .gitignore                     ← Protección credenciales
│
├── 📂 Docs/
│   ├── 📄 README.md                  ← Índice de documentación
│   ├── ⭐ QuickStart-Render.md       ← EMPIEZA AQUÍ
│   ├── 📘 DeployRender.md            ← Guía completa
│   ├── 🗄️ MySQL-Gratuito.md         ← Opciones MySQL
│   ├── ✅ Checklist-PostDeployment   ← Verificación
│   ├── 📋 CheatSheet-Render.md       ← Referencia rápida
│   └── 🗺️ DiagramaDeployment.md     ← Este archivo
│
└── 📂 CondominioAPI/
    ├── 🐳 Dockerfile                 ← Ya configurado
    ├── 📄 .env.example               ← Template local
    └── 📂 Database/
        └── 📄 CreateDB.sql           ← Schema para importar
```

---

## 🎯 Puntos de Decisión

```
                    ┌─────────────────┐
                    │ ¿Dónde desplegar? │
                    └────────┬──────────┘
                             │
              ┌──────────────┴──────────────┐
              │                             │
              ▼                             ▼
    ┌─────────────────┐         ┌──────────────────┐
    │ GRATIS / RÁPIDO │         │ PRODUCCIÓN       │
    │ (Render)        │         │ (AWS)            │
    └────────┬────────┘         └────────┬─────────┘
             │                           │
             ▼                           ▼
    ┌─────────────────┐         ┌──────────────────┐
    │ ✅ 30 min       │         │ ⏱️ 2-3 horas    │
    │ ✅ $0/mes       │         │ 💰 ~$30/mes     │
    │ ⚠️ Se duerme   │         │ ✅ 24/7 activo  │
    └─────────────────┘         └──────────────────┘
```

```
                   ┌──────────────────┐
                   │ ¿Qué MySQL usar? │
                   └────────┬─────────┘
                            │
              ┌─────────────┼─────────────┐
              │             │             │
              ▼             ▼             ▼
    ┌──────────────┐ ┌──────────┐ ┌──────────────┐
    │FreeSQLDatabase│ │ db4free  │ │   Railway    │
    │   (50 MB)    │ │ (200 MB) │ │  ($5 crédito)│
    └──────┬───────┘ └────┬─────┘ └──────┬───────┘
           │              │              │
           ▼              ▼              ▼
     ⭐ Recomendado    Alternativa    Si otros fallan
```

---

## ⏱️ Timeline Visual

```
0 min  ─────────────────────────────────────────────────► 45 min
  │         │           │            │            │
  ▼         ▼           ▼            ▼            ▼
Prep    Create DB   Config Render  Build      Verify
(10m)     (15m)        (15m)       (5-10m)     (5m)
```

---

## 🔐 Flujo de Autenticación

```
1. LOGIN REQUEST
   ┌─────────────┐
   │  Frontend   │
   └──────┬──────┘
          │ POST /api/authentication/login
          │ { email, password }
          ▼
   ┌──────────────────┐
   │ Authentication   │
   │   Controller     │
   └──────┬───────────┘
          │
          ├─► Validate credentials
          ├─► Hash password & compare
          └─► Generate JWT Token
          │
          ▼
   ┌──────────────────┐
   │  Return Token    │
   │  + User Info     │
   └──────────────────┘

2. AUTHENTICATED REQUEST
   ┌─────────────┐
   │  Frontend   │
   └──────┬──────┘
          │ GET /api/users
          │ Header: Authorization: Bearer <token>
          ▼
   ┌──────────────────┐
   │ JWT Middleware   │
   └──────┬───────────┘
          │
          ├─► Verify token signature
          ├─► Check expiration
          └─► Extract user claims
          │
          ▼
   ┌──────────────────┐
   │ Authorization    │
   │   Middleware     │
   └──────┬───────────┘
          │
          ├─► Check user roles
          └─► Validate permissions
          │
          ▼
   ┌──────────────────┐
   │   Controller     │
   │   Execute        │
   └──────────────────┘
```

---

## 💾 Flujo de Datos (CRUD)

```
CREATE (POST)
Frontend → Controller → Validator → Repository → MySQL
                                          │
                                          ▼
                                   INSERT INTO ...

READ (GET)
Frontend → Controller → Repository → MySQL
                             │
                             ▼
                      SELECT * FROM ...
                             │
                             ▼
                      Filter (IsDeleted=0)
                             │
                             ▼
                      Map to DTO

UPDATE (PUT)
Frontend → Controller → Validator → Repository → MySQL
                                          │
                                          ▼
                                   UPDATE ... SET ...

DELETE (DELETE) - Soft Delete
Frontend → Controller → Repository → MySQL
                             │
                             ▼
                      UPDATE ... SET IsDeleted=1
```

---

## 🎨 Vista General del Sistema

```
┌────────────────────────────────────────────────────────────┐
│                    SISTEMA COMPLETO                        │
└────────────────────────────────────────────────────────────┘

┌─────────────────┐         ┌──────────────────┐
│   FRONTEND      │◄───────►│   BACKEND API    │
│   (React/Vue)   │  REST   │   (.NET 8)       │
│                 │  JSON   │                  │
│  - UI/UX        │         │  - Controllers   │
│  - State Mgmt   │         │  - Auth (JWT)    │
│  - API Calls    │         │  - Business      │
│                 │         │  - Validation    │
│  Deployed on:   │         │                  │
│  Vercel/Netlify │         │  Deployed on:    │
│                 │         │  Render          │
└─────────────────┘         └────────┬─────────┘
                                     │
                                     │ MySQL
                                     │ Protocol
                                     ▼
                         ┌───────────────────┐
                         │   DATABASE        │
                         │   (MySQL 8.0)     │
                         │                   │
                         │  - Users          │
                         │  - Properties     │
                         │  - Expenses       │
                         │  - Payments       │
                         │                   │
                         │  Hosted on:       │
                         │  FreeSQLDatabase  │
                         └───────────────────┘
```

---

## 📊 Métricas de Éxito

```
✅ DEPLOYMENT EXITOSO SI:

□ API responde en < 2 segundos
□ Swagger UI carga correctamente
□ Login retorna JWT token
□ Endpoints protegidos funcionan
□ CORS configurado
□ No hay errores en logs
□ Base de datos conectada
□ Todas las tablas creadas
□ Frontend puede conectarse

TOTAL: ___/9 ✓
```

---

**Guarda este diagrama como referencia visual durante el deployment!** 🗺️
