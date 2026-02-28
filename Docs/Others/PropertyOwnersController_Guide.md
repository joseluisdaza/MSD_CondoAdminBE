# ?? PropertyOwnersController - API Documentation

## Descripción
Controller completo para la gestión de relaciones entre propiedades y propietarios (usuarios) en el sistema de administración de condominios.

## Base URL
`/api/propertyowners`

## Autorización
- **Requerido**: `Bearer Token` para todos los endpoints
- **Roles específicos** por operación (ver detalles por endpoint)

---

## ?? Endpoints Disponibles

### 1. **GET /api/propertyowners**
Obtiene relaciones de propietarios con filtros opcionales.

**Autorización**: `Administrador`, `Director`, `Habitante`, `Super`

**Query Parameters**:
- `propertyId` (int, opcional): Filtrar por ID de propiedad
- `userId` (int, opcional): Filtrar por ID de usuario
- `includeFinalized` (bool, opcional): Incluir relaciones finalizadas (default: false)

**Comportamiento por rol**:
- **Admin/Director/Super**: Pueden ver cualquier propiedad
- **Habitantes**: Solo pueden ver sus propias propiedades

**Examples**:
```bash
GET /api/propertyowners                                    # Todas las relaciones activas
GET /api/propertyowners?propertyId=5                       # Relaciones de propiedad 5
GET /api/propertyowners?userId=3                           # Propiedades del usuario 3
GET /api/propertyowners?includeFinalized=true              # Incluir finalizadas
GET /api/propertyowners?propertyId=5&includeFinalized=true # Combinado
```

**Response (200)**:
```json
[
  {
    "propertyId": 5,
    "userId": 3,
    "startDate": "2024-01-15T10:00:00",
    "endDate": null,
    "propertyLegalId": "PROP-001",
    "propertyTower": "A",
    "propertyFloor": 5,
    "propertyCode": "01",
    "userName": "Juan",
    "userLastName": "Pérez",
    "userLogin": "jperez",
    "userLegalId": "12345678",
    "isActive": true
  }
]
```

---

### 2. **GET /api/propertyowners/{propertyId}/{userId}**
Obtiene una relación específica entre propiedad y usuario.

**Autorización**: `Administrador`, `Director`, `Habitante`, `Super`

**Parameters**:
- `propertyId` (int): ID de la propiedad
- `userId` (int): ID del usuario

**Validaciones**:
- Habitantes solo pueden consultar sus propias relaciones

**Response (200)**:
```json
{
  "propertyId": 5,
  "userId": 3,
  "startDate": "2024-01-15T10:00:00",
  "endDate": null,
  "propertyLegalId": "PROP-001",
  "propertyTower": "A",
  "propertyFloor": 5,
  "propertyCode": "01",
  "userName": "Juan",
  "userLastName": "Pérez",
  "userLogin": "jperez",
  "userLegalId": "12345678",
  "isActive": true
}
```

**Response (404)**:
```json
"Relación propiedad-usuario no encontrada"
```

**Response (403)**:
```json
"No tienes permisos para ver las propiedades de otros usuarios"
```

---

### 3. **POST /api/propertyowners**
Crea una nueva relación propiedad-propietario.

**Autorización**: `Administrador`, `Super`

**Request Body**:
```json
{
  "propertyId": 5,
  "userId": 3
}
```

**Validaciones automáticas**:
- ? PropertyId y UserId son requeridos
- ? La propiedad debe existir
- ? El usuario debe existir
- ? No debe existir una relación activa entre ellos

**Response (201)**:
```json
{
  "propertyId": 5,
  "userId": 3,
  "startDate": "2024-12-19T14:30:00",  // Fecha actual automática
  "endDate": null,
  "propertyLegalId": "PROP-001",
  "propertyTower": "A",
  "propertyFloor": 5,
  "propertyCode": "01",
  "userName": "Juan",
  "userLastName": "Pérez",
  "userLogin": "jperez",
  "userLegalId": "12345678",
  "isActive": true
}
```

**Response (400) - Errores posibles**:
```json
"La propiedad especificada no existe"
"El usuario especificado no existe"
"Ya existe una relación activa entre esta propiedad y este usuario"
```

---

### 4. **DELETE /api/propertyowners/{propertyId}/{userId}**
Finaliza una relación propiedad-propietario (Soft Delete).

**Autorización**: `Administrador`, `Super`

**Parameters**:
- `propertyId` (int): ID de la propiedad
- `userId` (int): ID del usuario

**Comportamiento**:
- Establece `EndDate` con la fecha y hora actual
- No elimina físicamente el registro

**Response (204)**: Sin contenido (eliminación exitosa)

**Response (404)**:
```json
"Relación propiedad-usuario no encontrada"
```

**Response (400)**:
```json
"La relación ya está finalizada"
```

---

## ?? Características Técnicas

### Validaciones de Seguridad por Rol

#### **Administradores/Directores/Super**
```
? Pueden ver todas las propiedades
? Pueden crear relaciones para cualquier usuario/propiedad
? Pueden finalizar cualquier relación
? Acceso completo a filtros
```

#### **Habitantes**
```
? Solo pueden ver sus propias propiedades
? No pueden ver propiedades de otros usuarios
? No pueden crear relaciones
? No pueden finalizar relaciones
? Filtros automáticamente restringidos a su userId
```

### Soft Delete Implementation
```csharp
// Al crear relación
StartDate = DateTime.Now
EndDate = null  // Activa

// Al finalizar relación (DELETE)
EndDate = DateTime.Now  // Finalizada
```

### Filtro includeFinalized
```csharp
// includeFinalized = false (default)
query.Where(po => po.EndDate == null)  // Solo activas

// includeFinalized = true
// Sin filtro adicional, incluye todas
```

---

## ?? Casos de Uso y Ejemplos

### Caso 1: **Admin asigna propiedad a habitante**
```bash
# 1. Crear relación
POST /api/propertyowners
{
  "propertyId": 15,
  "userId": 8
}

# 2. Verificar asignación
GET /api/propertyowners?propertyId=15
# Response: Muestra la nueva relación activa

# 3. El habitante puede ver su propiedad
# Login como habitante (userId: 8)
GET /api/propertyowners?userId=8
# Response: Muestra sus propiedades asignadas
```

### Caso 2: **Habitante consulta sus propiedades**
```bash
# Como habitante autenticado
GET /api/propertyowners
# Automáticamente filtrado por su userId

GET /api/propertyowners?includeFinalized=true
# Ve sus propiedades actuales y pasadas
```

### Caso 3: **Admin transfiere propiedad**
```bash
# 1. Finalizar relación actual
DELETE /api/propertyowners/15/8

# 2. Crear nueva relación
POST /api/propertyowners
{
  "propertyId": 15,
  "userId": 12  # Nuevo propietario
}

# 3. Verificar historial completo
GET /api/propertyowners?propertyId=15&includeFinalized=true
# Muestra ambas relaciones: finalizada y nueva
```

### Caso 4: **Consulta específica de relación**
```bash
# Verificar relación específica
GET /api/propertyowners/15/8

# Si está activa: isActive = true, endDate = null
# Si está finalizada: isActive = false, endDate = "2024-12-19T..."
```

---

## ?? Estructura de Response Detallada

### PropertyOwnerResponse
```typescript
interface PropertyOwnerResponse {
  // IDs de relación
  propertyId: number;
  userId: number;
  
  // Fechas de relación
  startDate: string;     // ISO 8601
  endDate: string | null; // null = activa
  
  // Datos de propiedad
  propertyLegalId: string | null;
  propertyTower: string | null;
  propertyFloor: number | null;
  propertyCode: string | null;
  
  // Datos de usuario
  userName: string | null;
  userLastName: string | null;
  userLogin: string | null;
  userLegalId: string | null;
  
  // Estado calculado
  isActive: boolean;  // endDate == null
}
```

---

## ?? Integración con Sistema Existente

### Relación con Property
```bash
# Obtener propiedades y sus dueños
GET /api/property/5           # Info de propiedad
GET /api/propertyowners?propertyId=5  # Quiénes la poseen
```

### Relación con Users
```bash
# Obtener usuario y sus propiedades
GET /api/users/8              # Info del usuario
GET /api/propertyowners?userId=8      # Qué propiedades tiene
```

### Historial Completo
```bash
# Ver toda la historia de una propiedad
GET /api/propertyowners?propertyId=5&includeFinalized=true

# Ver toda la historia de un usuario
GET /api/propertyowners?userId=8&includeFinalized=true
```

---

## ?? Limitaciones y Consideraciones

### Claves Compuestas
- No hay un ID único auto-incremental
- La clave primaria es la combinación `(PropertyId, UserId, StartDate)`
- Una propiedad puede tener múltiples propietarios en el tiempo
- Un usuario puede tener múltiples propiedades

### Reasignaciones
- Para reasignar una propiedad: finalizar relación actual + crear nueva
- El historial se mantiene con `includeFinalized=true`
- No hay límite en el número de propietarios históricos

### Performance
- Consultas optimizadas con `Include()` para evitar N+1
- Filtros aplicados a nivel de BD, no en memoria
- Índices recomendados en `PropertyId` y `UserId`

---

## ?? Estados de Relación

| EndDate | Estado | Descripción | Visible por defecto |
|---------|---------|-------------|-------------------|
| **null** | ? Activa | Propietario actual | ? Sí |
| **fecha** | ? Finalizada | Ex-propietario | ? No (solo con `includeFinalized=true`) |

---

## ?? Seguridad Implementada

### Validación de Permisos
```csharp
// Para habitantes
if (!isAdmin && userId != currentUserId) {
    return Forbid("No tienes permisos para ver las propiedades de otros usuarios");
}
```

### Logging de Auditoría
```
[INFO] PropertyOwner relation created successfully. PropertyId: 15, UserId: 8
[INFO] PropertyOwner relation finalized successfully. PropertyId: 15, UserId: 8
[WARN] Attempt to view other user's properties by habitante
```

### Prevención de Duplicados
- Valida que no exista relación activa antes de crear nueva
- Permite recrear relación si la anterior está finalizada

¡El controller está completo y listo para gestionar propiedades y propietarios! ??