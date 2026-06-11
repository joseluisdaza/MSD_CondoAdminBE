use condominio2;

-- Limpiar procedimientos antiguos
DROP PROCEDURE IF EXISTS sp_report_propiedades_por_usuario;
DROP PROCEDURE IF EXISTS sp_report_usuarios_por_propiedad;
DROP PROCEDURE IF EXISTS sp_report_expensas_por_propiedad;
DROP PROCEDURE IF EXISTS sp_report_expensas_por_usuario;
DROP PROCEDURE IF EXISTS sp_report_reservas_por_usuario;
DROP PROCEDURE IF EXISTS sp_report_reservas_rango_fechas;
DROP PROCEDURE IF EXISTS sp_report_reservas_canceladas;

-- Limpiar vistas anteriores
DROP VIEW IF EXISTS vw_report_propiedades_por_usuario;
DROP VIEW IF EXISTS vw_report_usuarios_por_propiedad;
DROP VIEW IF EXISTS vw_report_expensas_por_propiedad;
DROP VIEW IF EXISTS vw_report_expensas_por_usuario;
DROP VIEW IF EXISTS vw_report_reservas_por_usuario;
DROP VIEW IF EXISTS vw_report_reservas;
DROP VIEW IF EXISTS vw_report_reservas_canceladas;

-- REPORTES DE PROPIEDADES Y USUARIOS
CREATE VIEW vw_report_propiedades_por_usuario AS
SELECT
	u.Id AS user_id,
	p.Id AS prop_id,
	p.Tower AS torre,
	p.Floor AS piso,
	p.Code AS codigo,
	po.Start_Date AS asociado_desde
FROM property_owners po
INNER JOIN users u ON u.Id = po.User_Id
INNER JOIN property p ON p.Id = po.Property_Id
WHERE po.Start_Date <= NOW()
	AND (po.End_Date IS NULL OR po.End_Date >= NOW());

CREATE VIEW vw_report_usuarios_por_propiedad AS
SELECT
	p.Id AS prop_id,
	u.Id AS user_id,
	u.User_Name AS nombre,
	u.Last_Name AS apellido,
	po.Start_Date AS desde,
	po.End_Date AS hasta
FROM property_owners po
INNER JOIN users u ON u.Id = po.User_Id
INNER JOIN property p ON p.Id = po.Property_Id;

-- REPORTES DE EXPENSAS
CREATE VIEW vw_report_expensas_por_propiedad AS
SELECT
	e.Property_Id AS prop_id,
	e.Id AS expense_id,
	ec.Category AS categoria,
	e.Description AS descripcion,
	e.Start_Date AS fecha_expensa,
	e.Payment_Limit_Date AS fecha_limite_pago,
	e.Amount AS monto,
	e.Interest_Amount AS monto_interes,
	e.Interest_Rate AS tasa_interes,
	ps.Status_Description AS estado
FROM expenses e
INNER JOIN expense_categories ec ON ec.Id = e.Category_Id
INNER JOIN payment_status ps ON ps.Id = e.Status_Id;

CREATE VIEW vw_report_expensas_por_usuario AS
SELECT DISTINCT
	po.User_Id AS user_id,
	e.Id AS expense_id,
	p.Id AS prop_id,
	p.Tower AS torre,
	p.Floor AS piso,
	p.Code AS codigo,
	ec.Category AS categoria,
	e.Description AS descripcion,
	e.Start_Date AS fecha_expensa,
	e.Payment_Limit_Date AS fecha_limite_pago,
	e.Amount AS monto,
	e.Interest_Amount AS monto_interes,
	e.Interest_Rate AS tasa_interes,
	ps.Status_Description AS estado
FROM property_owners po
INNER JOIN property p ON p.Id = po.Property_Id
INNER JOIN expenses e ON e.Property_Id = p.Id
INNER JOIN expense_categories ec ON ec.Id = e.Category_Id
INNER JOIN payment_status ps ON ps.Id = e.Status_Id
WHERE po.Start_Date <= NOW()
	AND (po.End_Date IS NULL OR po.End_Date >= NOW());

-- REPORTES DE RESERVAS
CREATE VIEW vw_report_reservas_por_usuario AS
SELECT
	rb.User_Id AS user_id,
	rb.Id AS reserva_id,
	r.Name AS recurso,
	p.Id AS prop_id,
	p.Tower AS torre,
	p.Floor AS piso,
	p.Code AS codigo,
	rb.Booking_Date AS fecha_inicio,
	rb.Booking_End_Date AS fecha_fin,
	rb.Booking_Price AS precio_reserva,
	rb.Booking_Warranty_Cost AS costo_garantia,
	ps.Status_Description AS estado,
	rb.Booking_Description AS descripcion
FROM resource_bookings rb
INNER JOIN resources r ON r.Id = rb.Resource_Id
INNER JOIN property p ON p.Id = rb.Property_Id
INNER JOIN payment_status ps ON ps.Id = rb.Status_Id;

CREATE VIEW vw_report_reservas AS
SELECT
	rb.Id AS reserva_id,
	r.Name AS recurso,
	u.Id AS user_id,
	u.User_Name AS nombre,
	u.Last_Name AS apellido,
	p.Id AS prop_id,
	p.Tower AS torre,
	p.Floor AS piso,
	p.Code AS codigo,
	rb.Booking_Date AS fecha_inicio,
	rb.Booking_End_Date AS fecha_fin,
	rb.Booking_Price AS precio_reserva,
	rb.Booking_Warranty_Cost AS costo_garantia,
	ps.Status_Description AS estado,
	rb.Booking_Description AS descripcion
FROM resource_bookings rb
INNER JOIN resources r ON r.Id = rb.Resource_Id
INNER JOIN users u ON u.Id = rb.User_Id
INNER JOIN property p ON p.Id = rb.Property_Id
INNER JOIN payment_status ps ON ps.Id = rb.Status_Id;

CREATE VIEW vw_report_reservas_canceladas AS
SELECT
	rb.Id AS reserva_id,
	r.Name AS recurso,
	u.Id AS user_id,
	u.User_Name AS nombre,
	u.Last_Name AS apellido,
	p.Id AS prop_id,
	p.Tower AS torre,
	p.Floor AS piso,
	p.Code AS codigo,
	rb.Booking_Date AS fecha_inicio,
	rb.Booking_End_Date AS fecha_fin,
	rb.Booking_Price AS precio_reserva,
	rb.Booking_Warranty_Cost AS costo_garantia,
	ps.Status_Description AS estado,
	rb.Booking_Description AS descripcion
FROM resource_bookings rb
INNER JOIN resources r ON r.Id = rb.Resource_Id
INNER JOIN users u ON u.Id = rb.User_Id
INNER JOIN property p ON p.Id = rb.Property_Id
INNER JOIN payment_status ps ON ps.Id = rb.Status_Id
WHERE ps.Status_Description = 'Anulado';

-- Ejemplos de uso:
-- SELECT * FROM vw_report_propiedades_por_usuario WHERE user_id = 10;
-- SELECT * FROM vw_report_usuarios_por_propiedad WHERE prop_id = 4;
-- SELECT * FROM vw_report_expensas_por_propiedad WHERE prop_id = 4 ORDER BY fecha_expensa DESC;
-- SELECT * FROM vw_report_expensas_por_usuario WHERE user_id = 10 ORDER BY fecha_expensa DESC;
-- SELECT * FROM vw_report_reservas_por_usuario WHERE user_id = 10 ORDER BY fecha_inicio DESC;
-- SELECT * FROM vw_report_reservas WHERE fecha_inicio <= '2026-12-31' AND fecha_fin >= '2026-01-01' ORDER BY fecha_inicio DESC;
-- SELECT * FROM vw_report_reservas_canceladas ORDER BY fecha_inicio DESC;
