# 🏢 MSD_CondoAdminBE

**Backend API para Sistema de Administración de Condominios**

Una solución completa para la gestión de condominios que incluye manejo de propiedades, gastos, pagos, usuarios y servicios.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Tecnologías](#️-tecnologías)
- [Arquitectura](#️-arquitectura)
- [Instalación](#-instalación)
- [Configuración](#️-configuración)
- [Base de Datos](#️-base-de-datos)
- [Endpoints de la API](#-endpoints-de-la-api)
- [Autenticación y Autorización](#-autenticación-y-autorización)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Contribución](#-contribución)

## ✨ Características

- **Gestión de Usuarios**: Registro, autenticación y autorización basada en roles
- **Administración de Propiedades**: Manejo de propiedades y tipos de propiedad
- **Control de Gastos**: Registro y categorización de gastos del condominio
- **Sistema de Pagos**: Procesamiento y seguimiento de pagos
- **Servicios**: Gestión de servicios y gastos asociados
- **Autenticación JWT**: Seguridad robusta con tokens JWT
- **Logging**: Sistema de logs detallado con Serilog
- **Soft Delete**: Eliminación lógica para mantener integridad de datos
- **API RESTful**: Endpoints bien estructurados siguiendo estándares REST

## 🛠️ Tecnologías

- **Framework**: ASP.NET Core 8.0
- **Base de Datos**: MySQL
- **ORM**: Entity Framework Core
- **Autenticación**: JWT Bearer Tokens
- **Logging**: Serilog
- **Documentación**: Swagger/OpenAPI
- **Validación**: FluentValidation
- **Configuración**: Variables de entorno con DotNetEnv

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas limpia:

- **CondominioAPI**: Capa de presentación (Controladores)
- **Condominio.Models**: Modelos de dominio
- **Condominio.DTOs**: Objetos de transferencia de datos
- **Condominio.Repository**: Capa de acceso a datos
- **Condominio.Utils**: Utilidades y helpers
- **Condominio.Data.Mysql**: Context y configuración de EF Core

## 🚀 Instalación

### Prerrequisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8.0+](https://dev.mysql.com/downloads/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### Pasos de Instalación

1. **Clonar el repositorio**

   ```bash
   git clone [URL_DEL_REPOSITORIO]
   cd MSD_CondoAdminBE
   ```

2. **Restaurar paquetes NuGet**

   ```bash
   dotnet restore CondominioAPI/CondominioAPI.sln
   ```

3. **Configurar variables de entorno**

   ```bash
   cp CondominioAPI/.env.example CondominioAPI/.env
   ```

4. **Crear base de datos**

   ```bash
   # Ejecutar scripts en Database/
   mysql -u root -p < Database/Db_Schema.sql
   mysql -u root -p < Database/insert_roles.sql
   ```

5. **Ejecutar aplicación**
   ```bash
   cd CondominioAPI/CondominioAPI
   dotnet run
   ```

## ⚙️ Configuración

### Variables de Entorno (.env)

Crear archivo `.env` en la carpeta `CondominioAPI/`:

```env
# Base de Datos
DB_SERVER=localhost
DB_NAME=condominio
DB_USER=root
DB_PASSWORD=tu_contraseña

# JWT
JWT_SECRET_KEY=tu_clave_secreta_jwt_muy_segura_aqui

# CORS
CORS_ALLOWED_ORIGIN=http://localhost:5173

# Logging (opcional)
LOG_PATH=Logs/log-.txt
```

### Configuración de Desarrollo

- **Puerto**: Configurado en `launchSettings.json`
- **Swagger**: Disponible en `/swagger` en modo desarrollo
- **Hot Reload**: Activado por defecto

## 🗄️ Base de Datos

### Modelos Principales

- **Users**: Usuarios del sistema
- **Roles**: Roles de usuario (Admin, Director, Habitante)
- **Properties**: Propiedades del condominio
- **Expenses**: Gastos del condominio
- **Payments**: Pagos realizados
- **ServiceExpenses**: Gastos de servicios
- **ExpenseCategories**: Categorías de gastos

### Migraciones

```bash
# Crear migración
dotnet ef migrations add NombreMigracion

# Aplicar migraciones
dotnet ef database update
```

## 🔗 Endpoints de la API

### Autenticación

| Método | Endpoint          | Descripción              |
| ------ | ----------------- | ------------------------ |
| POST   | `/api/auth/login` | Autenticación de usuario |

### Usuarios

| Método | Endpoint                   | Descripción                    | Autorización |
| ------ | -------------------------- | ------------------------------ | ------------ |
| GET    | `/api/users`               | Obtener todos los usuarios     | Admin        |
| GET    | `/api/users/{id}`          | Obtener usuario por ID         | Admin/Propio |
| POST   | `/api/users`               | Crear nuevo usuario            | Admin        |
| PUT    | `/api/users/{id}`          | Actualizar usuario             | Admin        |
| DELETE | `/api/users/{id}`          | Eliminar usuario (soft delete) | Admin        |
| PUT    | `/api/users/{id}/password` | Actualizar contraseña          | Admin/Propio |

### Propiedades

| Método | Endpoint               | Descripción                            | Autorización               |
| ------ | ---------------------- | -------------------------------------- | -------------------------- |
| GET    | `/api/property`        | Obtener todas las propiedades          | Admin/Director             |
| GET    | `/api/property/ByUser` | Obtener propiedades del usuario actual | Autenticado                |
| GET    | `/api/property/{id}`   | Obtener propiedad por ID               | Admin/Director/Propietario |
| POST   | `/api/property`        | Crear nueva propiedad                  | Admin                      |
| PUT    | `/api/property/{id}`   | Actualizar propiedad                   | Admin                      |
| DELETE | `/api/property/{id}`   | Eliminar propiedad                     | Admin                      |

### Gastos

| Método | Endpoint                              | Descripción                        | Autorización   |
| ------ | ------------------------------------- | ---------------------------------- | -------------- |
| GET    | `/api/expenses`                       | Obtener todos los gastos           | Admin/Director |
| GET    | `/api/expenses/{id}`                  | Obtener gasto por ID               | Admin/Director |
| GET    | `/api/expenses/property/{propertyId}` | Obtener gastos por propiedad       | Admin/Director |
| GET    | `/api/expenses/category/{categoryId}` | Obtener gastos por categoría       | Admin/Director |
| GET    | `/api/expenses/date-range`            | Obtener gastos por rango de fechas | Admin/Director |
| POST   | `/api/expenses`                       | Crear nuevo gasto                  | Admin          |
| PUT    | `/api/expenses/{id}`                  | Actualizar gasto                   | Admin          |
| DELETE | `/api/expenses/{id}`                  | Eliminar gasto                     | Admin          |

### Pagos

| Método | Endpoint                   | Descripción                       | Autorización   |
| ------ | -------------------------- | --------------------------------- | -------------- |
| GET    | `/api/payments`            | Obtener todos los pagos           | Admin/Director |
| GET    | `/api/payments/{id}`       | Obtener pago por ID               | Admin/Director |
| GET    | `/api/payments/date-range` | Obtener pagos por rango de fechas | Admin/Director |
| POST   | `/api/payments`            | Crear nuevo pago                  | Admin          |
| PUT    | `/api/payments/{id}`       | Actualizar pago                   | Admin          |
| DELETE | `/api/payments/{id}`       | Eliminar pago                     | Admin          |

### Categorías de Gastos

| Método | Endpoint                      | Descripción                  | Autorización   |
| ------ | ----------------------------- | ---------------------------- | -------------- |
| GET    | `/api/expensecategories`      | Obtener todas las categorías | Admin/Director |
| GET    | `/api/expensecategories/{id}` | Obtener categoría por ID     | Autenticado    |
| POST   | `/api/expensecategories`      | Crear nueva categoría        | Admin          |
| PUT    | `/api/expensecategories/{id}` | Actualizar categoría         | Admin          |
| DELETE | `/api/expensecategories/{id}` | Eliminar categoría           | Admin          |

### Servicios

| Método | Endpoint               | Descripción                 | Autorización   |
| ------ | ---------------------- | --------------------------- | -------------- |
| GET    | `/api/serviceexpenses` | Obtener gastos de servicios | Admin/Director |
| GET    | `/api/servicepayments` | Obtener pagos de servicios  | Admin/Director |
| GET    | `/api/servicetypes`    | Obtener tipos de servicios  | Admin/Director |
| POST   | `/api/serviceexpenses` | Crear gasto de servicio     | Admin          |
| POST   | `/api/servicepayments` | Crear pago de servicio      | Admin          |

### Salud del Sistema

| Método | Endpoint               | Descripción                      |
| ------ | ---------------------- | -------------------------------- |
| GET    | `/api/health`          | Verificar salud del API          |
| GET    | `/api/authHealthCheck` | Verificar salud de autenticación |

> **Total de Endpoints**: 90 endpoints disponibles

## 🔐 Autenticación y Autorización

### Roles del Sistema

1. **Administrador/Super**: Acceso completo
2. **Director**: Acceso a consultas y algunas operaciones
3. **Habitante**: Acceso limitado a sus propias propiedades

### JWT Token

- **Expiración**: Configurable
- **Algoritmo**: HS256
- **Claims**: UserId, Email, Roles

### Ejemplo de Autenticación

```bash
# Login
curl -X POST "https://api.condominio.com/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"login":"usuario@email.com","password":"contraseña"}'

# Usar token en requests
curl -X GET "https://api.condominio.com/api/users" \
  -H "Authorization: Bearer tu_jwt_token_aqui"
```

## 📁 Estructura del Proyecto

```
MSD_CondoAdminBE/
├── CondominioAPI/
│   ├── CondominioAPI/          # Proyecto principal API
│   │   ├── Controllers/        # Controladores REST
│   │   ├── Middleware/         # Middleware personalizado
│   │   └── Program.cs          # Configuración de la aplicación
│   ├── Condominio.Models/      # Modelos de dominio
│   ├── Condominio.DTOs/        # Data Transfer Objects
│   ├── Condominio.Repository/  # Capa de acceso a datos
│   ├── Condominio.Utils/       # Utilidades y helpers
│   ├── Condominio.Data.Mysql/  # Configuración EF Core
│   └── Condominio.Tests/       # Pruebas unitarias
├── Database/                   # Scripts de base de datos
├── Docs/                      # Documentación adicional
└── README.md                  # Este archivo
```

## 🤝 Contribución

1. Fork el proyecto
2. Crea una branch para tu feature (`git checkout -b feature/amazing-feature`)
3. Commit tus cambios (`git commit -m 'Add some amazing feature'`)
4. Push a la branch (`git push origin feature/amazing-feature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la licencia [MIT](LICENSE).

## 📞 Contacto

Para soporte o consultas, contacta al equipo de desarrollo.

---

**Desarrollado con ❤️ para la gestión eficiente de condominios**
