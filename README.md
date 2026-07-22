# 🏢 MSD_CondoAdminBE

**Backend API para Sistema de Administración de Condominios**

Una solución completa para la gestión de condominios que incluye manejo de propiedades, gastos, pagos, usuarios, servicios, recursos y reportes.

> **📚 DOCUMENTACIÓN COMPLETA**: La documentación técnica detallada está disponible en la **[Wiki del Proyecto](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki)**. Este README proporciona un resumen rápido.

## 📋 Tabla de Contenidos

- [🚀 Inicio Rápido](#-inicio-rápido)
- [📚 Documentación Completa](#-documentación-completa)
- [✨ Características](#-características)
- [🛠️ Tecnologías](#️-tecnologías)
- [🏗️ Arquitectura](#️-arquitectura)
- [⚙️ Configuración](#️-configuración)
- [🗄️ Base de Datos](#️-base-de-datos)
- [🚀 Deployment](#-deployment)
- [🔗 Endpoints de la API](#-endpoints-de-la-api)
- [🔐 Autenticación y Autorización](#-autenticación-y-autorización)
- [📁 Estructura del Proyecto](#-estructura-del-proyecto)
- [🤝 Contribución](#-contribución)
- [📚 Recursos Útiles](#-recursos-útiles)

## ✨ Características

- **Gestión de Usuarios**: Registro, autenticación y autorización basada en 6 roles
- **Administración de Propiedades**: Manejo de propiedades, tipos y propietarios
- **Control de Gastos**: Registro, categorización y seguimiento de gastos del condominio
- **Sistema de Pagos**: Procesamiento, seguimiento y estado de pagos
- **Servicios Adicionales**: Gestión de servicios (agua, luz, gas) y gastos asociados
- **Recursos Compartidos**: Gestión de recursos (salones, canchas) con reservas
- **Incidentes**: Reporte y seguimiento de incidentes con costos asociados
- **Reportes Avanzados**: Generación de reportes en PDF con auditoría
- **Autenticación JWT**: Seguridad robusta con tokens JWT de 1 hora
- **Logging Centralizado**: Sistema de logs detallado con Serilog (rotación diaria)
- **API RESTful**: 100+ endpoints bien estructurados siguiendo estándares REST
- **Validación Completa**: DTOs con validación de anotaciones y validators custom

## 🛠️ Tecnologías

### **Core**

- **Framework**: ASP.NET Core 8.0
- **Lenguaje**: C# 12
- **Base de Datos**: MySQL 8.0+
- **ORM**: Entity Framework Core 8.0

### **Seguridad y Autenticación**

- **JWT Bearer Tokens**: Autenticación sin estado
- **Hashing**: BCrypt para contraseñas
- **CORS**: Control de origen cruzado

### **Logging y Observabilidad**

- **Serilog**: Logging estructurado
- **Rotación de Logs**: Diaria con límite de 10MB

### **Documentación e Interoperabilidad**

- **Swagger/OpenAPI**: Documentación interactiva
- **DTOs**: Transferencia segura de datos

### **Variables de Entorno**

- **DotNetEnv**: Gestión de .env

### **Versión del Proyecto**

- **Versión Actual**: v2.1
- **Ultima Actualización**: Julio 2025

## 🏗️ Arquitectura

El proyecto sigue una **arquitectura en capas limpia** con **patrón Repository**:

### **Capas**

- **CondominioAPI**: Capa de presentación (Controladores REST)
- **Condominio.DTOs**: Data Transfer Objects (Validación de entrada)
- **Condominio.Models**: Modelos de dominio (Entidades)
- **Condominio.Repository**: Capa de acceso a datos (Repository pattern)
- **Condominio.Data.Mysql**: Contexto de Entity Framework
- **Condominio.Utils**: Utilidades (Hashing, converters, autorización)
- **Condominio.Reports**: Motor de reportes (PDF, auditoría)
- **Condominio.Tests**: Tests unitarios

### **Componentes Transversales**

- **Middleware**: Manejo de excepciones, logging
- **Security**: JWT, token blacklist para logout
- **Database**: MySQL con Migrations versionadas

**Para detalles técnicos, ver:** [Arquitectura del Proyecto](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/02-ARQUITECTURA-PROYECTO)

## 🚀 Inicio Rápido

### Setup Local (5 minutos)

```bash
# 1. Clonar repositorio
git clone https://github.com/joseluisdaza/MSD_CondoAdminBE.git
cd MSD_CondoAdminBE/CondominioAPI

# 2. Configurar variables de entorno
cp .env.example .env

# 3. Restaurar paquetes
dotnet restore

# 4. Aplicar migraciones
dotnet ef database update

# 5. Ejecutar
dotnet run --launch-profile "https"
```

**API disponible en**: `https://localhost:7221/api`

**Swagger UI**: `https://localhost:7221/swagger/index.html`

---

## 📚 Documentación Completa

La **Wiki del proyecto** contiene documentación técnica completa, ejemplos y guías:

### 📖 Fundamentos

- **[01 - Backend Overview](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/01-BACKEND-OVERVIEW)** - Visión general del backend, stack tecnológico
- **[02 - Arquitectura del Proyecto](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/02-ARQUITECTURA-PROYECTO)** - Estructura de capas, patrón Repository, middleware

### 📊 Diseño de Datos

- **[03 - Entidades y Modelos](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/03-ENTIDADES-MODELOS)** - 35+ modelos de datos, relaciones, diagrama ER
- **[04 - Controllers y Endpoints](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/04-CONTROLLERS-ENDPOINTS)** - 25 controllers, 100+ endpoints documentados
- **[05 - Repositorios y Datos](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/05-REPOSITORIOS-DATOS)** - Patrón Repository, 32 repositorios, ejemplos de queries

### 🔐 Seguridad

- **[06 - Autenticación y Autorización](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/06-AUTENTICACION-AUTORIZACION)** - JWT, roles (6), matriz de permisos, seguridad

### ⚙️ Configuración y Desarrollo

- **[07 - Configuración y Desarrollo Local](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/07-CONFIGURACION-DESARROLLO)** - Setup local, Docker, debugging, troubleshooting

### 🚀 Operaciones

- **[08 - Deployment y Producción](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/08-DEPLOYMENT)** - Docker, AWS, Render, CI/CD, monitoreo
- **[09 - Referencia de API](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/09-API-REFERENCE)** - Referencia completa con ejemplos cURL, requests/responses

---

## ⚙️ Configuración

### **Requisitos Previos**

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### **Instalación Rápida**

```bash
# 1. Clonar
git clone https://github.com/joseluisdaza/MSD_CondoAdminBE.git
cd MSD_CondoAdminBE/CondominioAPI

# 2. Configurar variables de entorno
cp .env.example .env
# Editar .env con tus credenciales

# 3. Restaurar y ejecutar
dotnet restore
dotnet ef database update
dotnet run --launch-profile "https"
```

### **Variables de Entorno (.env)**

```env
DB_SERVER=localhost
DB_NAME=condominio
DB_USER=root
DB_PASSWORD=tu_contraseña

JWT_SECRET_KEY=tu_clave_secreta_muy_larga_minimo_32_caracteres

CORS_ALLOWED_ORIGIN=https://localhost:5173
LOG_PATH=Logs/log-.txt
```

**Para configuración detallada y troubleshooting**, ver: [Configuración y Desarrollo Local](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/07-CONFIGURACION-DESARROLLO)

## 🗄️ Base de Datos

### **Modelos Principales (35+)**

| Entidad             | Descripción                                                                       |
| ------------------- | --------------------------------------------------------------------------------- |
| **User**            | Usuarios del sistema                                                              |
| **Role**            | Roles de autorización (6: Super, Admin, Director, Auxiliar, Habitante, Seguridad) |
| **Property**        | Propiedades/unidades del condominio                                               |
| **Expense**         | Gastos del condominio (con estado: Pending, PartiallyPaid, Paid)                  |
| **Payment**         | Pagos registrados                                                                 |
| **Resource**        | Recursos compartidos (salones, canchas)                                           |
| **ResourceBooking** | Reservas de recursos                                                              |
| **Incident**        | Incidentes reportados                                                             |
| **Report**          | Reportes generados (con auditoría)                                                |
| **ServiceExpense**  | Gastos de servicios adicionales                                                   |

**Para diagrama ER completo y relaciones**, ver: [Entidades y Modelos](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/03-ENTIDADES-MODELOS)

### **Migraciones**

```bash
# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar a la BD
dotnet ef database update

# Ver historial
dotnet ef migrations list
```

## 🚀 Deployment

### **⭐ Render.com (Recomendado - GRATIS)**

La forma más rápida y económica de desplegar:

```bash
# 1. Generar JWT secret
.\prepare-render.ps1 -GenerateJWT

# 2. Crear cuenta en Render.com
# 3. Conectar repo de GitHub
# 4. Deploy automático en ~2 minutos
```

**Características:**

- ✅ 100% Gratuito (con límites generosos)
- ✅ HTTPS automático
- ✅ Deploy automático con cada push
- ✅ MySQL gratuito en FreeSQLDatabase

**Documentación:** [Deployment - Render](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/08-DEPLOYMENT)

### **AWS (Escalable - ~$30/mes)**

Para deploy en producción con escalabilidad:

- ECS Fargate (contenedores)
- RDS MySQL (base de datos)
- ALB (balanceador de carga)

**Script:**

```powershell
.\deploy-aws.ps1 -Environment production
```

**Documentación:** [Deployment - AWS](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/08-DEPLOYMENT)

### **Docker Local**

```bash
# Build
docker-compose -f docker-compose.local.yml build

# Run
docker-compose -f docker-compose.local.yml up

# API: http://localhost:5000
```

## 🔗 Endpoints de la API

### **Resumen Rápido**

El sistema tiene **100+ endpoints** organizados en **25 controllers**:

| Controller          | Endpoints | Descripción                           |
| ------------------- | --------- | ------------------------------------- |
| **Auth**            | 2         | Login, logout con token blacklist     |
| **User**            | 8         | CRUD usuarios, roles, permisos        |
| **Property**        | 7         | CRUD propiedades, tipos, propietarios |
| **Expense**         | 9         | CRUD gastos, filtros avanzados        |
| **Payment**         | 6         | CRUD pagos, historial                 |
| **ExpenseCategory** | 5         | CRUD categorías de gastos             |
| **Service**         | 8         | CRUD servicios y gastos asociados     |
| **Resource**        | 6         | CRUD recursos compartidos             |
| **ResourceBooking** | 7         | CRUD reservas de recursos             |
| **Incident**        | 7         | CRUD incidentes reportados            |
| **Report**          | 5         | Generación de reportes en PDF         |
| **Health**          | 3         | Chequeo de salud del API              |
| **Y más...**        | 20+       | Otros controllers especializados      |

**Nota de Seguridad:**

- Todos los endpoints (excepto `/api/auth/login`) requieren **JWT Bearer Token**
- Cada endpoint valida permisos basado en **rol del usuario**
- Ver matriz de permisos en [Autenticación y Autorización](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/06-AUTENTICACION-AUTORIZACION)

### **Documentación Completa de Endpoints**

Para la **referencia completa con ejemplos cURL, requests y responses**, ver:
→ **[09 - Referencia de API](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/09-API-REFERENCE)**

Acceso interactivo a través de **Swagger UI**:

```
https://localhost:7221/swagger/index.html  (local)
https://tu-api.onrender.com/swagger        (producción)
```

### **Ejemplo: Login**

```bash
curl -X POST "https://api.condominio.com/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "login":"usuario@email.com",
    "password":"contraseña"
  }'

# Response
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": "user-123",
    "email": "usuario@email.com",
    "role": "Admin"
  }
}
```

**Usar token en requests:**

```bash
curl -X GET "https://api.condominio.com/api/users" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

## 🔐 Autenticación y Autorización

### **Sistema de Roles (6 Roles)**

| Rol             | Descripción              | Permisos                             |
| --------------- | ------------------------ | ------------------------------------ |
| **Super Admin** | Acceso total al sistema  | Todas las operaciones                |
| **Admin**       | Administrador de sistema | CRUD de usuarios, gastos, reportes   |
| **Director**    | Directiva del condominio | Lectura de reportes, autorizar pagos |
| **Auxiliar**    | Personal administrativo  | Entrada de datos de gastos/pagos     |
| **Habitante**   | Residente                | Ver sus propiedades y gastos         |
| **Seguridad**   | Personal de seguridad    | Reportar incidentes                  |

### **JWT Token**

```json
{
  "sub": "user-id-123",
  "email": "usuario@email.com",
  "roles": ["Admin"],
  "exp": 1721234567, // 1 hora desde login
  "iat": 1721230967,
  "iss": "CondominioAPI",
  "aud": "CondominioClient"
}
```

**Características:**

- ✅ Expiración: 1 hora (configurable)
- ✅ Algoritmo: HS256
- ✅ Token Blacklist: Para logout seguro
- ✅ Hashing: BCrypt para contraseñas

### **Seguridad**

- **HTTPS obligatorio** en producción
- **CORS restrictivo**: Solo dominios autorizados
- **Input Validation**: DTOs con validación anotada
- **Password Policy**: Contraseñas de 8+ caracteres con mayúsculas, números y símbolos
- **Rate Limiting**: Límites en endpoints de login (5 intentos/10 min)
- **Audit Logging**: Registro de todas las operaciones de usuario

### **Para Detalles Completos**

→ **[06 - Autenticación y Autorización](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/06-AUTENTICACION-AUTORIZACION)**

- Matriz de permisos detallada (roles × módulos)
- Flujos de autenticación
- Implementación de seguridad

## 📁 Estructura del Proyecto

```
MSD_CondoAdminBE/
├── CondominioAPI/
│   ├── CondominioAPI/              # Capa de Presentación
│   │   ├── Controllers/            # 25+ REST controllers
│   │   ├── Middleware/             # Excepciones, logging, autenticación
│   │   ├── Program.cs              # Configuración de DI y pipeline
│   │   └── appsettings.json        # Configuración
│   │
│   ├── Condominio.Models/          # Modelos de Dominio
│   │   ├── User.cs, Property.cs
│   │   ├── Expense.cs, Payment.cs
│   │   └── 35+ entidades...
│   │
│   ├── Condominio.DTOs/            # Data Transfer Objects
│   │   ├── Validación anotada
│   │   └── Custom validators
│   │
│   ├── Condominio.Repository/      # Patrón Repository
│   │   ├── IRepository<T>.cs       # Interfaz genérica
│   │   ├── Repository<T>.cs        # Implementación genérica
│   │   └── 32 repositorios específicos
│   │
│   ├── Condominio.Data.Mysql/      # Entity Framework Core
│   │   ├── CondominioDbContext.cs
│   │   ├── Migrations/
│   │   └── Configurations/         # DbModelBuilder
│   │
│   ├── Condominio.Utils/           # Utilidades Transversales
│   │   ├── AuthorizationHelper
│   │   ├── PasswordHasher (BCrypt)
│   │   └── DateConverters
│   │
│   ├── Condominio.Reports/         # Motor de Reportes
│   │   ├── PDF generation
│   │   └── Audit trails
│   │
│   ├── Condominio.Tests/           # Tests Unitarios
│   │   └── Controller/Repository tests
│   │
│   └── CondominioAPI.sln           # Solución Visual Studio
│
├── Database/                       # Scripts SQL
│   ├── Db_Schema.sql              # Estructura inicial
│   ├── insert_roles.sql           # Datos iniciales
│   └── Migrations/                # Migraciones EF
│
├── Docs/                          # Documentación adicional
│   ├── DeployRender.md
│   ├── DeployAWS.md
│   └── Otros guides...
│
├── docker-compose.yml             # Producción
├── docker-compose.local.yml       # Desarrollo
├── Dockerfile                     # Imagen Docker
├── prepare-render.ps1             # Script de setup
├── deploy-aws.ps1                 # Script AWS
└── README.md                      # Este archivo

```

**Para detalles arquitectónicos completos:**
→ **[02 - Arquitectura del Proyecto](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/02-ARQUITECTURA-PROYECTO)**

**Para entidades y modelos:**
→ **[03 - Entidades y Modelos](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/03-ENTIDADES-MODELOS)**

## 🤝 Contribución

1. **Fork** el proyecto
2. **Clone** tu fork: `git clone https://github.com/tu-usuario/MSD_CondoAdminBE.git`
3. **Crea una rama** para tu feature: `git checkout -b feature/my-awesome-feature`
4. **Commit** tus cambios: `git commit -m 'Add: my awesome feature'`
5. **Push** a tu fork: `git push origin feature/my-awesome-feature`
6. **Abre un Pull Request** describiendo los cambios

### **Guías de Contribución**

- ✅ Sigue el patrón de arquitectura en capas
- ✅ Implementa los repositorios específicos para acceso a datos
- ✅ Agrega validación en DTOs
- ✅ Escribe tests unitarios para nueva funcionalidad
- ✅ Documenta cambios significativos en la Wiki

---

## 📚 Recursos Útiles

### **Documentación**

- 📖 [Wiki Completa](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki) - Documentación técnica
- 🐳 [Docker Setup](https://github.com/joseluisdaza/MSD_CondoAdminBE/blob/main/CondominioAPI/DOCKER_README.md)
- ☁️ [Cloud Storage Setup](https://github.com/joseluisdaza/MSD_CondoAdminBE/blob/main/CondominioAPI/CLOUD_STORAGE_SETUP.md)

### **Deployment Rápido**

- 🚀 [Render Deploy (Gratis)](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/08-DEPLOYMENT#render-recomendado)
- 🔒 [AWS Deploy (Escalable)](https://github.com/joseluisdaza/MSD_CondoAdminBE/wiki/08-DEPLOYMENT#aws-escalable)

### **Frontend Relacionado**

- 🎨 [Frontend Repository](https://github.com/joseluisdaza/MSD_CondoAdminFront)
- 📱 [Frontend Documentation](https://github.com/joseluisdaza/MSD_CondoAdminFront/wiki)

### **Testing**

- 🧪 [Test Cases](./TEST_CASES.md)
- 📋 [Postman Collection](./CondominioAPI_Postman_Collection.json)

---

## 📄 Licencia

Este proyecto está bajo licencia **MIT**. Ver [LICENSE](LICENSE) para detalles.

---

## 📞 Contacto y Soporte

- **Issues**: [GitHub Issues](https://github.com/joseluisdaza/MSD_CondoAdminBE/issues)
- **Discussions**: [GitHub Discussions](https://github.com/joseluisdaza/MSD_CondoAdminBE/discussions)
- **Email**: [developer@condominio.com](mailto:developer@condominio.com)

---

## 🎯 Roadmap (Próximas Versiones)

- [ ] Integración con WhatsApp para notificaciones
- [ ] Dashboard analítico avanzado
- [ ] Sistema de alertas de pagos vencidos
- [ ] Integración con pasarelas de pago (Stripe, MercadoPago)
- [ ] App móvil nativa (React Native)
- [ ] Predicción de gastos con ML

---

**Desarrollado con ❤️ para la gestión eficiente de condominios**

[![Built with ASP.NET Core](https://img.shields.io/badge/built%20with-ASP.NET%20Core%208-0C54C2?style=flat&logo=.net)](https://dotnet.microsoft.com/)
[![MySQL Database](https://img.shields.io/badge/database-MySQL%208.0-00758F?style=flat&logo=mysql)](https://www.mysql.com/)
[![Docker Ready](https://img.shields.io/badge/docker-ready-2496ED?style=flat&logo=docker)](https://www.docker.com/)
[![MIT License](https://img.shields.io/badge/license-MIT-green.svg)](./LICENSE)
