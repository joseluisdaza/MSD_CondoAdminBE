# ?? Condominio API - Postman Collection

## Descripción
Esta colección de Postman incluye todos los endpoints disponibles en la API del Sistema de Administración de Condominios.

## Archivos Incluidos
- `CondominioAPI_Postman_Collection.json` - Colección principal con todos los endpoints
- `CondominioAPI_Environment.postman_environment.json` - Variables de entorno

## ?? Importar en Postman

### 1. Importar la Colección
1. Abre Postman
2. Click en **Import** (botón superior izquierdo)
3. Selecciona **Files** y elige `CondominioAPI_Postman_Collection.json`
4. Click **Import**

### 2. Importar el Environment
1. Click en **Import** nuevamente
2. Selecciona `CondominioAPI_Environment.postman_environment.json`
3. Click **Import**
4. Selecciona el environment "Condominio API Environment" en el dropdown superior derecho

## ?? Configuración Inicial

### Variables de Entorno
- `base_url`: `https://localhost:7001` (Puerto HTTPS por defecto)
- `base_url_dev`: `http://localhost:5000` (Puerto HTTP alternativo)
- `jwt_token`: Se llena automáticamente después del login
- Credenciales de usuarios por defecto incluidas

### Cambiar Puerto (si es necesario)
Si tu API corre en un puerto diferente, actualiza la variable `base_url`:
1. Click en el ícono del ojo ??? junto al environment
2. Edita `base_url` con tu puerto correcto

## ?? Autenticación

### Paso 1: Login
1. Expande la carpeta **Authentication**
2. Ejecuta la request **Login** (usa credenciales por defecto)
3. El token JWT se guardará automáticamente en `jwt_token`

### Paso 2: Usar Endpoints Protegidos
Todos los demás endpoints usan automáticamente el token guardado.

## ?? Estructura de la Colección

### ?? Authentication
- **Login** - Autenticación con usuario/password
- **Health Check** - Verificar estado de la API
- **Auth Health Check** - Verificar autenticación

### ?? Users
- CRUD completo de usuarios
- Endpoints con roles específicos

### ?? Properties & Property Types
- Gestión de propiedades y tipos de propiedad
- Relaciones entre entidades

### ?? Expenses & Payments
- **Expenses**: Gastos regulares del condominio
- **Payments**: Pagos realizados
- **Expense Payments**: Relaciones gasto-pago

### ?? Services
- **Service Types**: Tipos de servicios
- **Service Expenses**: Gastos de servicios
- **Service Payments**: Pagos de servicios
- **Service Expense Payments**: Relaciones servicio-pago

### ?? Audit Logs
- Consultas de logs de auditoría (Solo Super Admin)
- Filtros por usuario, acción, tabla y fechas

## ?? Ejemplos de Uso

### 1. Flujo Básico - Crear Gasto
```
1. Login (Authentication ? Login)
2. Crear Propiedad (Properties ? Create Property)
3. Crear Gasto (Expenses ? Create Expense)
4. Crear Pago (Payments ? Create Payment)
5. Asociar Gasto-Pago (Expense Payments ? Create Expense Payment Relation)
```

### 2. Flujo de Servicios
```
1. Login (Authentication ? Login)
2. Crear Tipo de Servicio (Service Types ? Create Service Type)
3. Crear Gasto de Servicio (Service Expenses ? Create Service Expense)
4. Crear Pago de Servicio (Service Payments ? Create Service Payment)
5. Asociar Servicio-Pago (Service Expense Payments ? Create Service Expense Payment Relation)
```

## ?? Usuarios por Defecto

| Usuario | Password | Rol | Descripción |
|---------|----------|-----|-------------|
| `usa` | `sa` | Super | Super Administrador |
| `uadmin` | `admin` | Admin | Administrador |
| `udirector` | `director` | Director | Miembro Directiva |
| `uhabitante` | `habitante` | Habitante | Usuario Regular |
| `uauxiliar` | `auxiliar` | Auxiliar | Auxiliar Admin |
| `useguridad` | `seguridad` | Seguridad | Guardia |

## ?? Configuración Avanzada

### Pre-request Scripts
La colección incluye scripts que:
- Extraen automáticamente el JWT token del login
- Lo almacenan en variables de entorno
- Lo usan automáticamente en requests posteriores

### Tests
Algunos endpoints incluyen tests básicos para:
- Verificar códigos de respuesta
- Extraer datos de respuestas
- Validar estructura de JSON

## ?? Notas Importantes

### Permisos por Rol
- **Super Admin**: Acceso total
- **Admin**: CRUD en la mayoría de entidades
- **Director**: Lectura + algunas operaciones
- **Habitante**: Solo lectura de datos propios
- **Auxiliar/Seguridad**: Acceso limitado

### Headers Automáticos
- `Authorization: Bearer {{jwt_token}}` - Se agrega automáticamente
- `Content-Type: application/json` - En requests con body

### Errores Comunes
- **401 Unauthorized**: Token expirado o inválido ? Ejecutar Login nuevamente
- **403 Forbidden**: Usuario sin permisos para la operación
- **404 Not Found**: Verificar IDs y rutas
- **500 Internal Server Error**: Error del servidor ? Verificar logs

## ?? Variables Dinámicas

Algunas requests usan variables que puedes personalizar:
- `{{base_url}}` - URL base de la API
- `{{jwt_token}}` - Token de autenticación
- IDs en URLs pueden cambiarse según tus datos

## ?? Personalización

### Agregar Nuevos Endpoints
Si se agregan nuevos endpoints a la API:
1. Duplica una request similar
2. Modifica URL, método y body
3. Ajusta autorización si es necesario

### Cambiar Datos de Prueba
Los bodies de ejemplo pueden modificarse según tus necesidades de testing.

## ?? Testing Recomendado

### Orden Sugerido de Pruebas
1. Health Check
2. Login
3. Users (CRUD)
4. Property Types ? Properties
5. Expenses ? Payments ? Expense Payments
6. Service Types ? Service Expenses ? Service Payments ? Relations
7. Audit Logs

¡La colección está lista para usar! ??