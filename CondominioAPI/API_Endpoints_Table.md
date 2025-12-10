# ?? API Endpoints Documentation

## Tabla Completa de Endpoints del Sistema

| Endpoint | Método | Tag | Descripción | Parámetros |
|----------|--------|-----|-------------|------------|
| `/api/auth/login` | POST | Auth | Autenticación de usuario | `LoginRequest` (body) |
| `/api/authHealthCheck` | GET | AuthHealthCheck | Verificar salud de autenticación | - |
| `/api/health` | GET | Health | Verificar salud del API | - |
| `/api/users` | GET | Users | Obtener todos los usuarios | - |
| `/api/users/{id}` | GET | Users | Obtener usuario por ID | `id` (int32) |
| `/api/users` | POST | Users | Crear nuevo usuario | `NewUserRequest` (body) |
| `/api/users/{id}` | PUT | Users | Actualizar usuario | `id` (int32), `NewUserBaseRequest` (body) |
| `/api/users/{id}` | DELETE | Users | Eliminar usuario (soft delete) | `id` (int32) |
| `/api/users/{id}/password` | PUT | Users | Actualizar contraseña | `id` (int32), `UpdatePasswordRequest` (body) |
| `/api/role/{id}` | GET | Role | Obtener roles de usuario | `id` (int32) |
| `/api/role` | POST | Role | Asignar rol a usuario | `UserRoleRequest` (body) |
| `/api/role` | DELETE | Role | Remover rol de usuario | `UserRoleRequest` (body) |
| `/api/role` | GET | Role | Obtener todos los roles | - |
| `/api/role/bulk-update` | PUT | Role | Actualizar roles masivamente | `BulkUserRoleUpdateRequest` (body) |
| `/api/property` | GET | Property | Obtener todas las propiedades | - |
| `/api/property/ByUser` | GET | Property | Obtener propiedades del usuario actual | - |
| `/api/property/{id}` | GET | Property | Obtener propiedad por ID | `id` (int32) |
| `/api/property` | POST | Property | Crear nueva propiedad | `PropertyRequest` (body) |
| `/api/property/{id}` | PUT | Property | Actualizar propiedad | `id` (int32), `PropertyRequest` (body) |
| `/api/property/{id}` | DELETE | Property | Eliminar propiedad (soft delete) | `id` (int32) |
| `/api/propertytype` | GET | PropertyType | Obtener todos los tipos de propiedad | - |
| `/api/propertytype/{id}` | GET | PropertyType | Obtener tipo de propiedad por ID | `id` (int32) |
| `/api/propertytype` | POST | PropertyType | Crear nuevo tipo de propiedad | `PropertyTypeRequest` (body) |
| `/api/propertytype/{id}` | PUT | PropertyType | Actualizar tipo de propiedad | `id` (int32), `PropertyTypeRequest` (body) |
| `/api/propertytype/{id}` | DELETE | PropertyType | Eliminar tipo de propiedad (soft delete) | `id` (int32) |
| `/api/propertyowners` | GET | PropertyOwners | Obtener relaciones propiedad-usuario | `propertyId` (int32, query), `userId` (int32, query), `includeFinalized` (bool, query) |
| `/api/propertyowners/{propertyId}/{userId}` | GET | PropertyOwners | Obtener relación específica | `propertyId` (int32), `userId` (int32) |
| `/api/propertyowners` | POST | PropertyOwners | Crear relación propiedad-usuario | `PropertyOwnerRequest` (body) |
| `/api/propertyowners/{propertyId}/{userId}` | DELETE | PropertyOwners | Finalizar relación (soft delete) | `propertyId` (int32), `userId` (int32) |
| `/api/expenses` | GET | Expenses | Obtener todos los gastos | - |
| `/api/expenses/{id}` | GET | Expenses | Obtener gasto por ID | `id` (int32) |
| `/api/expenses/property/{propertyId}` | GET | Expenses | Obtener gastos por propiedad | `propertyId` (int32) |
| `/api/expenses/category/{categoryId}` | GET | Expenses | Obtener gastos por categoría | `categoryId` (int32) |
| `/api/expenses/status/{statusId}` | GET | Expenses | Obtener gastos por estado | `statusId` (int32) |
| `/api/expenses/date-range` | GET | Expenses | Obtener gastos por rango de fechas | `startDate` (DateTime, query), `endDate` (DateTime, query) |
| `/api/expenses` | POST | Expenses | Crear nuevo gasto | `ExpenseRequest` (body) |
| `/api/expenses/{id}` | PUT | Expenses | Actualizar gasto | `id` (int32), `ExpenseRequest` (body) |
| `/api/expenses/{id}` | DELETE | Expenses | Eliminar gasto | `id` (int32) |
| `/api/expensecategories` | GET | ExpenseCategories | Obtener todas las categorías de gastos | - |
| `/api/expensecategories/{id}` | GET | ExpenseCategories | Obtener categoría por ID | `id` (int32) |
| `/api/expensecategories/category/{categoryName}` | GET | ExpenseCategories | Obtener categoría por nombre | `categoryName` (string) |
| `/api/expensecategories` | POST | ExpenseCategories | Crear nueva categoría | `ExpenseCategoryRequest` (body) |
| `/api/expensecategories/{id}` | PUT | ExpenseCategories | Actualizar categoría | `id` (int32), `ExpenseCategoryRequest` (body) |
| `/api/expensecategories/{id}` | DELETE | ExpenseCategories | Eliminar categoría | `id` (int32) |
| `/api/payments` | GET | Payments | Obtener todos los pagos | - |
| `/api/payments/{id}` | GET | Payments | Obtener pago por ID | `id` (int32) |
| `/api/payments/receive-number/{receiveNumber}` | GET | Payments | Obtener pago por número de recibo | `receiveNumber` (string) |
| `/api/payments/date-range` | GET | Payments | Obtener pagos por rango de fechas | `startDate` (DateTime, query), `endDate` (DateTime, query) |
| `/api/payments` | POST | Payments | Crear nuevo pago | `PaymentRequest` (body) |
| `/api/payments/{id}` | PUT | Payments | Actualizar pago | `id` (int32), `PaymentRequest` (body) |
| `/api/payments/{id}` | DELETE | Payments | Eliminar pago | `id` (int32) |
| `/api/paymentstatus` | GET | PaymentStatus | Obtener todos los estados de pago | - |
| `/api/expensepayments` | GET | ExpensePayments | Obtener todas las relaciones gasto-pago | - |
| `/api/expensepayments/{id}` | GET | ExpensePayments | Obtener relación por ID | `id` (int32) |
| `/api/expensepayments/expense/{expenseId}` | GET | ExpensePayments | Obtener relaciones por gasto | `expenseId` (int32) |
| `/api/expensepayments/payment/{paymentId}` | GET | ExpensePayments | Obtener relaciones por pago | `paymentId` (int32) |
| `/api/expensepayments` | POST | ExpensePayments | Procesar pago automático de gasto | `CreateExpensePaymentRequest` (body) |
| `/api/expensepayments/{id}` | PUT | ExpensePayments | Actualizar relación gasto-pago | `id` (int32), `ExpensePaymentRequest` (body) |
| `/api/expensepayments/{id}` | DELETE | ExpensePayments | Eliminar relación gasto-pago | `id` (int32) |
| `/api/servicetypes` | GET | ServiceTypes | Obtener todos los tipos de servicio | - |
| `/api/servicetypes/{id}` | GET | ServiceTypes | Obtener tipo de servicio por ID | `id` (int32) |
| `/api/servicetypes/service/{serviceName}` | GET | ServiceTypes | Obtener tipo por nombre de servicio | `serviceName` (string) |
| `/api/servicetypes` | POST | ServiceTypes | Crear nuevo tipo de servicio | `ServiceTypeRequest` (body) |
| `/api/servicetypes/{id}` | PUT | ServiceTypes | Actualizar tipo de servicio | `id` (int32), `ServiceTypeRequest` (body) |
| `/api/servicetypes/{id}` | DELETE | ServiceTypes | Eliminar tipo de servicio | `id` (int32) |
| `/api/serviceexpenses` | GET | ServiceExpenses | Obtener todos los gastos de servicio | - |
| `/api/serviceexpenses/{id}` | GET | ServiceExpenses | Obtener gasto de servicio por ID | `id` (int32) |
| `/api/serviceexpenses/service-type/{serviceTypeId}` | GET | ServiceExpenses | Obtener gastos por tipo de servicio | `serviceTypeId` (int32) |
| `/api/serviceexpenses/status/{statusId}` | GET | ServiceExpenses | Obtener gastos por estado | `statusId` (int32) |
| `/api/serviceexpenses/date-range` | GET | ServiceExpenses | Obtener gastos por rango de fechas | `startDate` (DateTime, query), `endDate` (DateTime, query) |
| `/api/serviceexpenses` | POST | ServiceExpenses | Crear nuevo gasto de servicio | `ServiceExpenseRequest` (body) |
| `/api/serviceexpenses/{id}` | PUT | ServiceExpenses | Actualizar gasto de servicio | `id` (int32), `ServiceExpenseRequest` (body) |
| `/api/serviceexpenses/{id}` | DELETE | ServiceExpenses | Eliminar gasto de servicio | `id` (int32) |
| `/api/servicepayments` | GET | ServicePayments | Obtener todos los pagos de servicio | - |
| `/api/servicepayments/{id}` | GET | ServicePayments | Obtener pago de servicio por ID | `id` (int32) |
| `/api/servicepayments/receive-number/{receiveNumber}` | GET | ServicePayments | Obtener pago por número de recibo | `receiveNumber` (string) |
| `/api/servicepayments/status/{statusId}` | GET | ServicePayments | Obtener pagos por estado | `statusId` (int32) |
| `/api/servicepayments/date-range` | GET | ServicePayments | Obtener pagos por rango de fechas | `startDate` (DateTime, query), `endDate` (DateTime, query) |
| `/api/servicepayments` | POST | ServicePayments | Crear pago de servicio con relación automática | `ServicePaymentRequest` (body) |
| `/api/servicepayments/{id}` | PUT | ServicePayments | Actualizar pago de servicio | `id` (int32), `ServicePaymentRequest` (body) |
| `/api/servicepayments/{id}` | DELETE | ServicePayments | Eliminar pago de servicio | `id` (int32) |
| `/api/serviceexpensepayments` | GET | ServiceExpensePayments | Obtener todas las relaciones servicio-pago | - |
| `/api/serviceexpensepayments/{id}` | GET | ServiceExpensePayments | Obtener relación por ID | `id` (int32) |
| `/api/serviceexpensepayments/service-expense/{serviceExpenseId}` | GET | ServiceExpensePayments | Obtener relaciones por gasto de servicio | `serviceExpenseId` (int32) |
| `/api/serviceexpensepayments/payment/{paymentId}` | GET | ServiceExpensePayments | Obtener relaciones por pago | `paymentId` (int32) |
| `/api/serviceexpensepayments` | POST | ServiceExpensePayments | Crear relación servicio-pago | `ServiceExpensePaymentRequest` (body) |
| `/api/serviceexpensepayments/{id}` | PUT | ServiceExpensePayments | Actualizar relación servicio-pago | `id` (int32), `ServiceExpensePaymentRequest` (body) |
| `/api/serviceexpensepayments/{id}` | DELETE | ServiceExpensePayments | Eliminar relación servicio-pago | `id` (int32) |

---

## ?? Autorización por Endpoints

### Endpoints Públicos (sin autorización)
- `POST /api/auth/login`

### Solo Administradores/Super
- `GET /api/users`
- `POST /api/users`
- `PUT /api/users/{id}`
- `DELETE /api/users/{id}`
- `POST /api/role`
- `DELETE /api/role`
- `PUT /api/role/bulk-update`
- `POST /api/property`
- `PUT /api/property/{id}`
- `DELETE /api/property/{id}`
- `POST /api/propertytype`
- `PUT /api/propertytype/{id}`
- `DELETE /api/propertytype/{id}`
- `POST /api/propertyowners`
- `DELETE /api/propertyowners/{propertyId}/{userId}`
- `POST /api/expensecategories`
- `PUT /api/expensecategories/{id}`
- `DELETE /api/expensecategories/{id}`
- `POST /api/expenses`
- `PUT /api/expenses/{id}`
- `DELETE /api/expenses/{id}`
- `POST /api/payments`
- `PUT /api/payments/{id}`
- `DELETE /api/payments/{id}`
- `POST /api/expensepayments`
- `PUT /api/expensepayments/{id}`
- `DELETE /api/expensepayments/{id}`
- `POST /api/servicetypes`
- `PUT /api/servicetypes/{id}`
- `DELETE /api/servicetypes/{id}`
- `POST /api/serviceexpenses`
- `PUT /api/serviceexpenses/{id}`
- `DELETE /api/serviceexpenses/{id}`
- `POST /api/servicepayments`
- `PUT /api/servicepayments/{id}`
- `DELETE /api/servicepayments/{id}`
- `POST /api/serviceexpensepayments`
- `PUT /api/serviceexpensepayments/{id}`
- `DELETE /api/serviceexpensepayments/{id}`

### Administradores + Directores + Habitantes
- `GET /api/users/{id}` (con restricciones)
- `PUT /api/users/{id}/password` (con restricciones)
- `GET /api/property/ByUser`
- `GET /api/property/{id}` (con restricciones)
- `GET /api/propertyowners` (con restricciones por rol)
- `GET /api/propertyowners/{propertyId}/{userId}` (con restricciones)
- `GET /api/propertytype`
- `GET /api/propertytype/{id}`
- `GET /api/expensecategories/{id}`
- `GET /api/expensecategories/category/{categoryName}`
- `GET /api/paymentstatus`

### Administradores + Directores
- `GET /api/role/{id}`
- `GET /api/role`
- `GET /api/expensecategories`
- `GET /api/expenses`
- `GET /api/expenses/{id}`
- `GET /api/expenses/property/{propertyId}`
- `GET /api/expenses/category/{categoryId}`
- `GET /api/expenses/status/{statusId}`
- `GET /api/expenses/date-range`
- `GET /api/payments`
- `GET /api/payments/{id}`
- `GET /api/payments/receive-number/{receiveNumber}`
- `GET /api/payments/date-range`
- `GET /api/expensepayments`
- `GET /api/expensepayments/{id}`
- `GET /api/expensepayments/expense/{expenseId}`
- `GET /api/expensepayments/payment/{paymentId}`
- `GET /api/servicetypes`
- `GET /api/servicetypes/{id}`
- `GET /api/servicetypes/service/{serviceName}`
- `GET /api/serviceexpenses`
- `GET /api/serviceexpenses/{id}`
- `GET /api/serviceexpenses/service-type/{serviceTypeId}`
- `GET /api/serviceexpenses/status/{statusId}`
- `GET /api/serviceexpenses/date-range`
- `GET /api/servicepayments`
- `GET /api/servicepayments/{id}`
- `GET /api/servicepayments/receive-number/{receiveNumber}`
- `GET /api/servicepayments/status/{statusId}`
- `GET /api/servicepayments/date-range`
- `GET /api/serviceexpensepayments`
- `GET /api/serviceexpensepayments/{id}`
- `GET /api/serviceexpensepayments/service-expense/{serviceExpenseId}`
- `GET /api/serviceexpensepayments/payment/{paymentId}`

---

## ?? Resumen por Tags

| Tag | Total Endpoints | GET | POST | PUT | DELETE |
|-----|-----------------|-----|------|-----|--------|
| **Auth** | 1 | 0 | 1 | 0 | 0 |
| **AuthHealthCheck** | 1 | 1 | 0 | 0 | 0 |
| **Health** | 1 | 1 | 0 | 0 | 0 |
| **Users** | 6 | 2 | 1 | 2 | 1 |
| **Role** | 5 | 2 | 1 | 1 | 1 |
| **Property** | 6 | 3 | 1 | 1 | 1 |
| **PropertyType** | 5 | 2 | 1 | 1 | 1 |
| **PropertyOwners** | 4 | 2 | 1 | 0 | 1 |
| **Expenses** | 9 | 6 | 1 | 1 | 1 |
| **ExpenseCategories** | 6 | 3 | 1 | 1 | 1 |
| **Payments** | 7 | 4 | 1 | 1 | 1 |
| **PaymentStatus** | 1 | 1 | 0 | 0 | 0 |
| **ExpensePayments** | 7 | 4 | 1 | 1 | 1 |
| **ServiceTypes** | 6 | 3 | 1 | 1 | 1 |
| **ServiceExpenses** | 9 | 6 | 1 | 1 | 1 |
| **ServicePayments** | 9 | 6 | 1 | 1 | 1 |
| **ServiceExpensePayments** | 7 | 4 | 1 | 1 | 1 |

**Total: 90 endpoints**

---

## ?? Endpoints con Funcionalidades Especiales

### Procesamiento Automático
- `POST /api/expensepayments` - Crea pago automático y actualiza estado del gasto
- `POST /api/servicepayments` - Crea pago, relación y actualiza estado del servicio

### Búsquedas Avanzadas
- `GET /api/expenses/date-range` - Filtro por rango de fechas
- `GET /api/payments/date-range` - Filtro por rango de fechas  
- `GET /api/servicepayments/date-range` - Filtro por rango de fechas
- `GET /api/serviceexpenses/date-range` - Filtro por rango de fechas

### Operaciones Masivas
- `PUT /api/role/bulk-update` - Actualización masiva de roles de usuario

### Filtros Flexibles
- `GET /api/propertyowners` - Múltiples filtros opcionales (propertyId, userId, includeFinalized)

---

## ?? Notas Importantes

1. **Autorización**: La mayoría de endpoints requieren JWT token válido
2. **Soft Delete**: Muchos endpoints DELETE son soft delete (EndDate)
3. **Validaciones**: Todos los endpoints POST/PUT incluyen validaciones automáticas
4. **Logging**: Todas las operaciones están registradas con Serilog
5. **Error Handling**: Manejo consistente de errores con códigos HTTP apropiados