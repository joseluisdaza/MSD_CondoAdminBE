# ?? ExpenseCategoriesController - API Documentation

## Descripción
Controller completo para la gestión de categorías de gastos en el sistema de administración de condominios.

## Base URL
`/api/expensecategories`

## Autorización
- **Requerido**: `Bearer Token` para todos los endpoints
- **Roles específicos** por operación (ver detalles por endpoint)

---

## ?? Endpoints Disponibles

### 1. **GET /api/expensecategories**
Obtiene todas las categorías de gastos con el conteo de gastos relacionados.

**Autorización**: `Administrador`, `Director`, `Super`

**Response (200)**:
```json
[
  {
    "id": 1,
    "category": "Administración",
    "description": "Gastos administrativos del condominio",
    "totalExpenses": 15
  },
  {
    "id": 2,
    "category": "Mantenimiento",
    "description": "Gastos de mantenimiento general",
    "totalExpenses": 8
  }
]
```

---

### 2. **GET /api/expensecategories/{id}**
Obtiene una categoría específica por ID.

**Autorización**: `Administrador`, `Director`, `Habitante`, `Super`

**Parameters**:
- `id` (int): ID de la categoría

**Response (200)**:
```json
{
  "id": 1,
  "category": "Administración",
  "description": "Gastos administrativos del condominio",
  "totalExpenses": 15
}
```

**Response (404)**:
```json
"Categoría de gasto con ID 999 no encontrada"
```

---

### 3. **GET /api/expensecategories/category/{categoryName}**
Obtiene una categoría específica por nombre.

**Autorización**: `Administrador`, `Director`, `Habitante`, `Super`

**Parameters**:
- `categoryName` (string): Nombre de la categoría

**Example**: `GET /api/expensecategories/category/Administración`

**Response (200)**:
```json
{
  "id": 1,
  "category": "Administración",
  "description": "Gastos administrativos del condominio",
  "totalExpenses": 15
}
```

---

### 4. **POST /api/expensecategories**
Crea una nueva categoría de gasto.

**Autorización**: `Administrador`, `Super`

**Request Body**:
```json
{
  "id": 0,
  "category": "Servicios Públicos",
  "description": "Gastos relacionados con agua, luz, gas, etc."
}
```

**Response (201)**:
```json
{
  "id": 3,
  "category": "Servicios Públicos",
  "description": "Gastos relacionados con agua, luz, gas, etc.",
  "totalExpenses": 0
}
```

**Response (400) - Duplicado**:
```json
"Ya existe una categoría de gasto con el nombre 'Servicios Públicos'"
```

**Validaciones**:
- ? `Category` requerido, máximo 100 caracteres
- ? `Description` requerido, máximo 500 caracteres
- ? Nombre único (case-insensitive)

---

### 5. **PUT /api/expensecategories/{id}**
Actualiza una categoría existente.

**Autorización**: `Administrador`, `Super`

**Parameters**:
- `id` (int): ID de la categoría a actualizar

**Request Body**:
```json
{
  "id": 3,
  "category": "Servicios Públicos",
  "description": "Gastos relacionados con servicios públicos como agua, electricidad y gas"
}
```

**Response (200)**:
```json
{
  "id": 3,
  "category": "Servicios Públicos",
  "description": "Gastos relacionados con servicios públicos como agua, electricidad y gas",
  "totalExpenses": 5
}
```

**Response (400) - ID no coincide**:
```json
"El ID del parámetro no coincide con el ID del objeto"
```

**Response (400) - Nombre duplicado**:
```json
"Ya existe otra categoría de gasto con el nombre 'Mantenimiento'"
```

---

### 6. **DELETE /api/expensecategories/{id}**
Elimina una categoría de gasto.

**Autorización**: `Administrador`, `Super`

**Parameters**:
- `id` (int): ID de la categoría a eliminar

**Response (204)**: Sin contenido (eliminación exitosa)

**Response (400) - Tiene gastos relacionados**:
```json
"No se puede eliminar la categoría de gasto porque tiene gastos relacionados. Elimine primero los gastos o asígnelos a otra categoría."
```

**Response (404)**:
```json
"Categoría de gasto con ID 999 no encontrada"
```

---

## ?? Características Técnicas

### Validaciones Implementadas
- ? **Duplicados**: Previene categorías con nombres duplicados (case-insensitive)
- ? **Integridad referencial**: No permite eliminar categorías con gastos asociados
- ? **DataAnnotations**: Validación automática de campos requeridos y longitudes
- ? **ID matching**: En updates, valida consistencia de IDs

### Logging
- ? **Información**: Operaciones exitosas con detalles del usuario
- ? **Advertencias**: Intentos de crear/actualizar con nombres duplicados
- ? **Errores**: Excepciones y errores del sistema con stack traces

### Performance
- ? **Eager loading**: Incluye conteo de gastos relacionados
- ? **Consultas optimizadas**: Búsquedas case-insensitive eficientes
- ? **Async operations**: Todas las operaciones son asíncronas

---

## ?? Ejemplos de Testing

### Test 1: Crear categoría válida
```bash
POST /api/expensecategories
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "id": 0,
  "category": "Seguridad",
  "description": "Gastos relacionados con la seguridad del condominio"
}
```

### Test 2: Intentar crear duplicado (debe fallar)
```bash
POST /api/expensecategories
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "id": 0,
  "category": "SEGURIDAD",  // Same name, different case
  "description": "Otra descripción"
}
```

### Test 3: Actualizar categoría
```bash
PUT /api/expensecategories/1
Authorization: Bearer {admin_token}
Content-Type: application/json

{
  "id": 1,
  "category": "Administración General",
  "description": "Gastos administrativos generales del condominio"
}
```

### Test 4: Buscar por nombre
```bash
GET /api/expensecategories/category/Administración
Authorization: Bearer {habitante_token}
```

### Test 5: Intentar eliminar con gastos (debe fallar)
```bash
DELETE /api/expensecategories/1
Authorization: Bearer {admin_token}
```

---

## ?? Códigos de Respuesta

| Código | Descripción | Cuándo ocurre |
|--------|-------------|---------------|
| **200** | ? OK | Operación exitosa (GET, PUT) |
| **201** | ? Created | Categoría creada exitosamente |
| **204** | ? No Content | Categoría eliminada exitosamente |
| **400** | ? Bad Request | Validaciones fallidas, duplicados, ID mismatch |
| **401** | ? Unauthorized | Token inválido o expirado |
| **403** | ? Forbidden | Usuario sin permisos para la operación |
| **404** | ? Not Found | Categoría no encontrada |
| **500** | ? Internal Server Error | Error interno del servidor |

---

## ?? Relaciones con Otras Entidades

### Dependencias
- **Expenses**: Las categorías pueden tener múltiples gastos asociados
- **Users**: Requiere usuario autenticado con roles específicos

### Restricciones
- No se puede eliminar una categoría que tenga gastos asociados
- Los nombres de categoría deben ser únicos
- Solo administradores pueden crear/modificar/eliminar

---

## ?? Casos de Uso Comunes

### 1. **Setup Inicial del Sistema**
```
1. POST /api/expensecategories (crear "Administración")
2. POST /api/expensecategories (crear "Mantenimiento")
3. POST /api/expensecategories (crear "Servicios Públicos")
4. GET /api/expensecategories (verificar creación)
```

### 2. **Gestión Regular**
```
1. GET /api/expensecategories (listar todas)
2. GET /api/expensecategories/1 (ver detalles específicos)
3. PUT /api/expensecategories/1 (actualizar descripción)
4. GET /api/expensecategories/category/Administración (buscar por nombre)
```

### 3. **Cleanup/Mantenimiento**
```
1. GET /api/expensecategories (identificar categorías sin uso)
2. DELETE /api/expensecategories/5 (eliminar si no tiene gastos)
3. Si tiene gastos: reasignar o eliminar gastos primero
```

¡El controller está completamente implementado y listo para usar! ??