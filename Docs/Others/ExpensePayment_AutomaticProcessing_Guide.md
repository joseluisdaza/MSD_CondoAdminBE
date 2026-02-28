# ?? ExpensePayment - Automatic Payment Processing

## Descripción
El endpoint `POST /api/expensepayments` ha sido modificado para procesar automáticamente el pago de gastos pendientes. Ahora solo requiere el `ExpenseId` y realiza todo el proceso de pago automáticamente.

---

## ?? Nuevo Flujo de Proceso de Pago

### Endpoint Modificado
**`POST /api/expensepayments`**

### Request Body Simplificado
```json
{
  "expenseId": 123
}
```

---

## ?? Proceso Automático Implementado

### 1. **Validación del Gasto**
```
? Verificar que el gasto existe
? Verificar que el statusId = 1 (Pendiente)
? Verificar que no tenga pagos previos
```

### 2. **Cálculo del Monto**
```javascript
if (InterestRate == null) {
    paymentAmount = Amount + InterestAmount
} else {
    paymentAmount = Amount * (1 + InterestRate/100)
}
```

### 3. **Creación Automática del Payment**
```json
{
  "receiveNumber": "AUTO-{ExpenseId}",
  "paymentDate": "2024-12-19T10:30:00",
  "amount": 1575.00,
  "description": "Pago automático para gasto: {ExpenseDescription}",
  "receivePhoto": "AUTO_PAYMENT"
}
```

### 4. **Creación de la Relación ExpensePayment**
```json
{
  "expenseId": 123,
  "paymentId": 456
}
```

### 5. **Actualización del Estado del Gasto**
```
StatusId: 1 (Pending) ? 2 (Paid)
```

---

## ?? Ejemplos de Uso

### ? Caso Exitoso - Gasto Sin Interés
**Request:**
```json
POST /api/expensepayments
{
  "expenseId": 15
}
```

**Gasto Original:**
```json
{
  "id": 15,
  "amount": 1500.00,
  "interestAmount": 75.00,
  "interestRate": null,
  "statusId": 1,
  "description": "Cuota de administración"
}
```

**Payment Creado:**
```json
{
  "id": 201,
  "receiveNumber": "AUTO-15",
  "paymentDate": "2024-12-19T10:30:00",
  "amount": 1575.00,  // 1500 + 75
  "description": "Pago automático para gasto: Cuota de administración",
  "receivePhoto": "AUTO_PAYMENT"
}
```

**Response (201):**
```json
{
  "id": 301,
  "expenseId": 15,
  "paymentId": 201,
  "expenseDescription": "Cuota de administración",
  "expenseAmount": 1500.00,
  "paymentReceiveNumber": "AUTO-15",
  "paymentAmount": 1575.00,
  "paymentDate": "2024-12-19T10:30:00"
}
```

---

### ? Caso Exitoso - Gasto Con InterestRate
**Request:**
```json
POST /api/expensepayments
{
  "expenseId": 25
}
```

**Gasto Original:**
```json
{
  "id": 25,
  "amount": 2000.00,
  "interestAmount": 100.00,
  "interestRate": 5.0,
  "statusId": 1
}
```

**Cálculo del Amount:**
```
paymentAmount = 2000.00 * (1 + 5.0/100) = 2000.00 * 1.05 = 2100.00
```

**Payment Creado:**
```json
{
  "amount": 2100.00  // Calculado con InterestRate
}
```

---

## ? Casos de Error

### Error: Gasto No Encontrado
**Request:**
```json
{
  "expenseId": 999
}
```

**Response (400):**
```json
"El gasto especificado no existe"
```

### Error: Gasto Ya Pagado
**Request:**
```json
{
  "expenseId": 10
}
```

**Response (400):**
```json
"No se puede procesar el pago. El gasto ya está pagado"
```

### Error: Gasto Cancelado
**Response (400):**
```json
"No se puede procesar el pago. El gasto está cancelado"
```

### Error: Gasto Ya Tiene Pago
**Response (400):**
```json
"El gasto ya tiene un pago asociado"
```

---

## ?? Estados de Gasto Válidos

| StatusId | Descripción | ¿Permite Pago? | Mensaje de Error |
|----------|-------------|----------------|------------------|
| **0** | Undefined | ? | "El gasto tiene estado indefinido" |
| **1** | Pending | ? | - |
| **2** | Paid | ? | "El gasto ya está pagado" |
| **3** | Overdue | ? | "El gasto está vencido" |
| **4** | Cancelled | ? | "El gasto está cancelado" |

---

## ?? Lógica de Cálculo de Montos

### Caso 1: InterestRate es NULL
```csharp
if (expense.InterestRate == null)
{
    paymentAmount = expense.Amount + (expense.InterestAmount ?? 0);
}
```

**Ejemplos:**
- Amount: $1000, InterestAmount: $50, InterestRate: null ? **$1050**
- Amount: $2000, InterestAmount: null, InterestRate: null ? **$2000**

### Caso 2: InterestRate tiene valor
```csharp
else
{
    paymentAmount = expense.Amount * (1 + expense.InterestRate.Value / 100);
}
```

**Ejemplos:**
- Amount: $1000, InterestRate: 5% ? $1000 * 1.05 = **$1050**
- Amount: $1500, InterestRate: 10% ? $1500 * 1.10 = **$1650**

---

## ?? Logging Implementado

### Logs de Información
```
[INFO] POST > ExpensePayments > Create. User: admin, ExpenseId: 15
[INFO] Automatic payment created. PaymentId: 201, Amount: 1575.00, ExpenseId: 15
[INFO] ExpensePayment relation created. Id: 301, ExpenseId: 15, PaymentId: 201
[INFO] Expense status updated to Paid. ExpenseId: 15, Old Status: 1, New Status: 2
[INFO] Expense payment process completed successfully. ExpenseId: 15, PaymentId: 201, Amount: 1575.00
```

### Logs de Advertencia
```
[WARN] Payment creation failed - Invalid status. ExpenseId: 15, StatusId: 2
```

### Logs de Error
```
[ERROR] Error processing expense payment for ExpenseId: 15
```

---

## ?? Proceso Completo Paso a Paso

### Antes del Proceso
```json
// Expense Estado Inicial
{
  "id": 15,
  "amount": 1500.00,
  "statusId": 1,  // Pending
  "description": "Cuota administración"
}

// Payment: No existe
// ExpensePayment: No existe
```

### Después del Proceso
```json
// Expense Estado Final
{
  "id": 15,
  "amount": 1500.00,
  "statusId": 2,  // Paid ?
  "description": "Cuota administración"
}

// Payment Creado
{
  "id": 201,
  "receiveNumber": "AUTO-15",
  "amount": 1575.00,
  "paymentDate": "2024-12-19T10:30:00"
}

// ExpensePayment Relación
{
  "id": 301,
  "expenseId": 15,
  "paymentId": 201
}
```

---

## ?? Testing Scenarios

### Test 1: Pago Exitoso Sin InterestRate
```bash
# 1. Crear gasto pendiente
POST /api/expenses
{
  "amount": 1000,
  "interestAmount": 50,
  "interestRate": null,
  "statusId": 1
}

# 2. Procesar pago
POST /api/expensepayments
{
  "expenseId": {created_expense_id}
}

# 3. Verificar resultado
GET /api/expenses/{created_expense_id}
# Debe tener statusId = 2
```

### Test 2: Pago Exitoso Con InterestRate
```bash
# 1. Crear gasto con interés
POST /api/expenses
{
  "amount": 2000,
  "interestRate": 5.0,
  "statusId": 1
}

# 2. Procesar pago
POST /api/expensepayments
{
  "expenseId": {created_expense_id}
}

# 3. Verificar amount = 2000 * 1.05 = 2100
```

### Test 3: Error - Gasto Ya Pagado
```bash
# 1. Procesar pago (primera vez)
POST /api/expensepayments
{
  "expenseId": 15
}

# 2. Intentar pagar de nuevo (debe fallar)
POST /api/expensepayments
{
  "expenseId": 15
}
# Response: 400 - "El gasto ya está pagado"
```

---

## ? Beneficios de la Nueva Implementación

1. **? Simplicidad**: Solo requiere ExpenseId
2. **? Automatización**: Crea payment automáticamente
3. **? Cálculo correcto**: Maneja InterestRate y InterestAmount
4. **? Integridad**: Actualiza estado del gasto automáticamente
5. **? Validaciones**: Previene pagos duplicados y estados inválidos
6. **? Auditoría**: Logging completo del proceso
7. **? Transaccional**: Todo el proceso es atómico

¡El endpoint está listo para procesar pagos automáticamente! ??