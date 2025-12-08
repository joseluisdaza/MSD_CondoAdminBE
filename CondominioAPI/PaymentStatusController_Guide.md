# ?? PaymentStatusController - API Documentation

## Descripción
Controller simple para obtener los estados de pago disponibles en el sistema. Solo expone un endpoint de consulta que devuelve únicamente los campos esenciales: `Id` y `StatusDescription`.

---

## ?? Endpoint Disponible

### **GET /api/paymentstatus**
Obtiene todos los estados de pago disponibles en el sistema.

**Autorización**: `Administrador`, `Director`, `Habitante`, `Super`

**Response (200)**:
```json
[
  {
    "id": 0,
    "statusDescription": "Undefined"
  },
  {
    "id": 1,
    "statusDescription": "Pending"
  },
  {
    "id": 2,
    "statusDescription": "Paid"
  },
  {
    "id": 3,
    "statusDescription": "Overdue"
  },
  {
    "id": 4,
    "statusDescription": "Cancelled"
  }
]
```

**Response (401)**:
```json
{
  "message": "Unauthorized"
}
```

**Response (500)**:
```json
"Error interno del servidor"
```

---

## ?? Características Técnicas

### Funcionalidad
- ? **Solo lectura**: Endpoint únicamente de consulta
- ? **Campos mínimos**: Solo devuelve `Id` y `StatusDescription`
- ? **Sin paginación**: Devuelve todos los registros (la tabla es pequeña)
- ? **Sin filtros**: Endpoint simple sin parámetros

### Autorización
- ? **Múltiples roles**: Accesible para Admin, Director, Habitante y Super
- ? **JWT requerido**: Requiere token de autenticación válido

### Logging
- ? **Información**: Registra cada consulta con el usuario
- ? **Conteo**: Log del número de registros devueltos
- ? **Errores**: Captura y registra excepciones

### Performance
- ? **Consulta simple**: Sin joins ni includes innecesarios
- ? **Respuesta ligera**: Solo campos esenciales
- ? **Caché-friendly**: Datos que raramente cambian

---

## ?? Ejemplos de Uso

### Ejemplo básico
```bash
GET /api/paymentstatus
Authorization: Bearer {jwt_token}
```

### Con curl
```bash
curl -X GET "https://localhost:7001/api/paymentstatus" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### Con JavaScript (fetch)
```javascript
const response = await fetch('/api/paymentstatus', {
  method: 'GET',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});

const paymentStatuses = await response.json();
console.log(paymentStatuses);
```

---

## ?? Casos de Uso

### 1. **Dropdowns/Select Lists**
```javascript
// Cargar opciones para un select de estados
fetch('/api/paymentstatus')
  .then(response => response.json())
  .then(statuses => {
    const select = document.getElementById('statusSelect');
    statuses.forEach(status => {
      const option = new Option(status.statusDescription, status.id);
      select.add(option);
    });
  });
```

### 2. **Validación de Estados**
```javascript
// Verificar si un estado es válido
const validStatuses = await fetch('/api/paymentstatus').then(r => r.json());
const isValidStatus = (statusId) => validStatuses.some(s => s.id === statusId);
```

### 3. **Mapeo de Estados**
```javascript
// Crear mapeo para mostrar descripciones
const statuses = await fetch('/api/paymentstatus').then(r => r.json());
const statusMap = statuses.reduce((map, status) => {
  map[status.id] = status.statusDescription;
  return map;
}, {});

// Uso: statusMap[1] => "Pending"
```

---

## ?? Integración con Otros Endpoints

Este endpoint es complementario y se usa típicamente junto con:

### Expenses
```javascript
// Obtener gastos y sus estados
const [expenses, statuses] = await Promise.all([
  fetch('/api/expenses').then(r => r.json()),
  fetch('/api/paymentstatus').then(r => r.json())
]);

// Mapear descripciones de estado
expenses.forEach(expense => {
  const status = statuses.find(s => s.id === expense.statusId);
  expense.statusName = status?.statusDescription || 'Unknown';
});
```

### Service Expenses
```javascript
// Similar para gastos de servicios
const serviceExpenses = await fetch('/api/serviceexpenses').then(r => r.json());
const statuses = await fetch('/api/paymentstatus').then(r => r.json());
// ... mapeo similar
```

---

## ?? Estructura de Datos

### PaymentStatusSimpleResponse
```typescript
interface PaymentStatusSimpleResponse {
  id: number;           // Identificador único del estado
  statusDescription: string;  // Descripción legible del estado
}
```

### Estados Típicos del Sistema
| ID | StatusDescription | Uso |
|----|------------------|-----|
| 0 | Undefined | Estado por defecto/no definido |
| 1 | Pending | Pagos pendientes |
| 2 | Paid | Pagos completados |
| 3 | Overdue | Pagos vencidos |
| 4 | Cancelled | Pagos cancelados |

---

## ?? Características del Controller

### Simplicidad
- ? **Un solo endpoint**: Funcionalidad específica y clara
- ? **Sin parámetros**: No requiere filtros ni paginación
- ? **Respuesta directa**: Array simple de objetos

### Eficiencia
- ? **Consulta rápida**: Sin joins complejos
- ? **Payload mínimo**: Solo campos necesarios
- ? **Cache-friendly**: Datos estables, ideales para caché

### Mantenibilidad
- ? **Código limpio**: Lógica simple y directa
- ? **Logging básico**: Suficiente para debugging
- ? **Error handling**: Manejo estándar de excepciones

---

## ?? Seguridad

### Autenticación
- ? **JWT requerido**: No acceso anónimo
- ? **Token validation**: Validación automática del token

### Autorización
- ? **Roles múltiples**: Accesible para la mayoría de roles
- ? **Sin datos sensibles**: Solo metadatos del sistema

### Audit Trail
- ? **Logging**: Cada consulta se registra con el usuario
- ? **No modificaciones**: Endpoint de solo lectura

---

## ?? Notas de Implementación

### DTO Específico
Se creó `PaymentStatusSimpleResponse` en lugar de reusar el DTO completo para:
- Mayor claridad en la API
- Mejor performance (menos datos)
- Evolución independiente del endpoint

### Logging Incluido
```csharp
Log.Information("GET > PaymentStatus > GetAll. User: {0}", User.Identity?.Name);
Log.Information("PaymentStatus GetAll completed successfully. Count: {0}", simpleResponses.Count);
```

### Error Handling
```csharp
try { /* operation */ }
catch (Exception ex) {
    Log.Error(ex, "Error getting all payment statuses");
    return StatusCode(500, "Error interno del servidor");
}
```

¡El controller está listo y es perfecto para cargar listas de estados de pago! ??