use condominio2;

-- Reporte de propiedades por usuario
DROP PROCEDURE IF EXISTS sp_report_propiedades_por_usuario;
DROP PROCEDURE IF EXISTS sp_report_usuarios_por_propiedad;
DROP PROCEDURE IF EXISTS sp_report_expensas_por_propiedad;
DROP PROCEDURE IF EXISTS sp_report_expensas_por_usuario;
DROP PROCEDURE IF EXISTS sp_report_reservas_por_usuario;
DROP PROCEDURE IF EXISTS sp_report_reservas_rango_fechas;
DROP PROCEDURE IF EXISTS sp_report_reservas_canceladas;

-- REPORTES DE PROPIEDADES Y USUARIOS
DELIMITER $$

CREATE PROCEDURE sp_report_propiedades_por_usuario(IN p_user_id INT)
BEGIN
SELECT
	p.Id 			AS prop_id,
	p.Tower 		AS torre,
	p.Floor 		AS piso,
	p.Code 			AS codigo,
	po.Start_Date 	AS asociado_desde
FROM property_owners po
INNER JOIN users u		ON u.Id = po.User_Id
INNER JOIN property p	ON p.Id = po.Property_Id
WHERE u.Id = p_user_id
	AND po.Start_Date <= NOW()
	AND (po.End_Date IS NULL OR po.End_Date >= NOW());
END$$

-- Reporte de usuarios por propiedad
CREATE PROCEDURE sp_report_usuarios_por_propiedad(IN p_property_id INT)
BEGIN
SELECT
	u.Id 			AS user_id,
	u.User_Name 	AS nombre,
    u.Last_Name		AS apellido,
	po.Start_Date 	AS desde,
    po.End_Date		As hasta
    
FROM property_owners po
INNER JOIN users u		ON u.Id = po.User_Id
INNER JOIN property p	ON p.Id = po.Property_Id
WHERE p.Id = p_property_id;
END$$

DELIMITER ;

-- REPORTES DE EXPENSAS
-- Reporte de expensas por propiedad
-- Reporte de expensas por usuario
-- Reporte de reservas por usuario

DELIMITER $$

CREATE PROCEDURE sp_report_expensas_por_propiedad(IN p_property_id INT)
BEGIN
SELECT
	e.Id 					AS expense_id,
	ec.Category 			AS categoria,
	e.Description 			AS descripcion,
	e.Start_Date 			AS fecha_expensa,
	e.Payment_Limit_Date 	AS fecha_limite_pago,
	e.Amount 				AS monto,
	e.Interest_Amount 		AS monto_interes,
	e.Interest_Rate 		AS tasa_interes,
	ps.Status_Description 	AS estado
FROM expenses e
INNER JOIN expense_categories ec ON ec.Id = e.Category_Id
INNER JOIN payment_status ps ON ps.Id = e.Status_Id
WHERE e.Property_Id = p_property_id
ORDER BY e.Start_Date DESC;
END$$

CREATE PROCEDURE sp_report_expensas_por_usuario(IN p_user_id INT)
BEGIN
SELECT DISTINCT
	e.Id 					AS expense_id,
	p.Id 					AS prop_id,
	p.Tower 				AS torre,
	p.Floor 				AS piso,
	p.Code 					AS codigo,
	ec.Category 			AS categoria,
	e.Description 			AS descripcion,
	e.Start_Date 			AS fecha_expensa,
	e.Payment_Limit_Date 	AS fecha_limite_pago,
	e.Amount 				AS monto,
	e.Interest_Amount 		AS monto_interes,
	e.Interest_Rate 		AS tasa_interes,
	ps.Status_Description 	AS estado
FROM property_owners po
INNER JOIN property p ON p.Id = po.Property_Id
INNER JOIN expenses e ON e.Property_Id = p.Id
INNER JOIN expense_categories ec ON ec.Id = e.Category_Id
INNER JOIN payment_status ps ON ps.Id = e.Status_Id
WHERE po.User_Id = p_user_id
	AND po.Start_Date <= NOW()
	AND (po.End_Date IS NULL OR po.End_Date >= NOW())
ORDER BY e.Start_Date DESC;
END$$

CREATE PROCEDURE sp_report_reservas_por_usuario(IN p_user_id INT)
BEGIN
SELECT
	rb.Id 					AS reserva_id,
	r.Name 				AS recurso,
	p.Id 					AS prop_id,
	p.Tower 				AS torre,
	p.Floor 				AS piso,
	p.Code 					AS codigo,
	rb.Booking_Date 		AS fecha_inicio,
	rb.Booking_End_Date 	AS fecha_fin,
	rb.Booking_Price 		AS precio_reserva,
	rb.Booking_Warranty_Cost AS costo_garantia,
	ps.Status_Description 	AS estado,
	rb.Booking_Description 	AS descripcion
FROM resource_bookings rb
INNER JOIN resources r ON r.Id = rb.Resource_Id
INNER JOIN property p ON p.Id = rb.Property_Id
INNER JOIN payment_status ps ON ps.Id = rb.Status_Id
WHERE rb.User_Id = p_user_id
ORDER BY rb.Booking_Date DESC;
END$$


-- REPORTES DE RESERVAS
-- Reporte de reservas en un rango de fechas
-- Reporte de reservas canceladas

CREATE PROCEDURE sp_report_reservas_rango_fechas(IN p_fecha_inicio DATETIME, IN p_fecha_fin DATETIME)
BEGIN
SELECT
	rb.Id 					AS reserva_id,
	r.Name 				AS recurso,
	u.Id 					AS user_id,
	u.User_Name 			AS nombre,
	u.Last_Name 			AS apellido,
	p.Id 					AS prop_id,
	p.Tower 				AS torre,
	p.Floor 				AS piso,
	p.Code 					AS codigo,
	rb.Booking_Date 		AS fecha_inicio,
	rb.Booking_End_Date 	AS fecha_fin,
	rb.Booking_Price 		AS precio_reserva,
	rb.Booking_Warranty_Cost AS costo_garantia,
	ps.Status_Description 	AS estado,
	rb.Booking_Description 	AS descripcion
FROM resource_bookings rb
INNER JOIN resources r ON r.Id = rb.Resource_Id
INNER JOIN users u ON u.Id = rb.User_Id
INNER JOIN property p ON p.Id = rb.Property_Id
INNER JOIN payment_status ps ON ps.Id = rb.Status_Id
WHERE rb.Booking_Date <= p_fecha_fin
	AND rb.Booking_End_Date >= p_fecha_inicio
ORDER BY rb.Booking_Date DESC;
END$$

CREATE PROCEDURE sp_report_reservas_canceladas()
BEGIN
SELECT
	rb.Id 					AS reserva_id,
	r.Name 				AS recurso,
	u.Id 					AS user_id,
	u.User_Name 			AS nombre,
	u.Last_Name 			AS apellido,
	p.Id 					AS prop_id,
	p.Tower 				AS torre,
	p.Floor 				AS piso,
	p.Code 					AS codigo,
	rb.Booking_Date 		AS fecha_inicio,
	rb.Booking_End_Date 	AS fecha_fin,
	rb.Booking_Price 		AS precio_reserva,
	rb.Booking_Warranty_Cost AS costo_garantia,
	ps.Status_Description 	AS estado,
	rb.Booking_Description 	AS descripcion
FROM resource_bookings rb
INNER JOIN resources r ON r.Id = rb.Resource_Id
INNER JOIN users u ON u.Id = rb.User_Id
INNER JOIN property p ON p.Id = rb.Property_Id
INNER JOIN payment_status ps ON ps.Id = rb.Status_Id
WHERE ps.Status_Description = 'Anulado'
ORDER BY rb.Booking_Date DESC;
END$$

DELIMITER ;
