 CREATE DATABASE IF NOT EXISTS condominio_demo;
 USE condominio_demo;

-- Users and roles
SELECT 'Creating Users and Roles Tables';

CREATE TABLE IF NOT EXISTS users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Login VARCHAR(30) NOT NULL UNIQUE,
    User_Name VARCHAR(100) NOT NULL,
    Last_Name VARCHAR(100) NOT NULL,
    Legal_Id VARCHAR(1000) NULL,
    Start_Date DATETIME NOT NULL,
    End_Date DATETIME NULL,
    PASSWORD VARCHAR(1000) NOT NULL
);

CREATE TABLE IF NOT EXISTS roles
(
  Id INT PRIMARY KEY,
  Rol_Name VARCHAR(50) NOT NULL,
  Description VARCHAR(1000)
);

CREATE TABLE IF NOT EXISTS user_roles
(
  Role_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  PRIMARY KEY (Role_Id, User_Id),
  FOREIGN KEY (Role_Id) REFERENCES roles(Id),
  FOREIGN KEY (User_Id) REFERENCES users(Id)
);

-- Properties
SELECT 'Creating Property Tables';
CREATE TABLE IF NOT EXISTS property_type
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Type VARCHAR (100) NOT NULL,
  Description VARCHAR (500) NOT NULL,
  Rooms INT DEFAULT 0,
  Bathrooms INT DEFAULT 0,
  Water_Service BOOLEAN DEFAULT FALSE,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL
);

CREATE TABLE IF NOT EXISTS property
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Legal_Id VARCHAR(100) NULL,
  Tower VARCHAR(20) NOT NULL,
  Floor INT NOT NULL,
  Code VARCHAR(2) NOT NULL,
  Property_Type INT NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Property_Type) REFERENCES property_type (Id)
);

CREATE TABLE IF NOT EXISTS property_owners
(
  Property_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  PRIMARY KEY (Property_Id, User_Id),
  FOREIGN KEY (Property_Id) REFERENCES property (Id),
  FOREIGN KEY (User_Id) REFERENCES users (Id)
);

-- Expenses and Payments
SELECT 'Creating Expenses and Payments Tables';
CREATE TABLE IF NOT EXISTS payment_status
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Status_Description VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS expense_categories
(
  Id INT AUTO_INCREMENT PRIMARY KEY, 
  Category VARCHAR(100) NOT NULL,
  Description VARCHAR(500) NOT NULL
);

CREATE TABLE IF NOT EXISTS expenses
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Category_Id INT NOT NULL,
  Property_Id INT,
  Start_Date DATETIME NOT NULL,
  Payment_Limit_Date DATETIME NOT NULL,
  Amount DECIMAL(10,2) NOT NULL,
  Interest_Amount DECIMAL(10,2) DEFAULT 0,
  Interest_Rate DECIMAL(5,2) DEFAULT 0,
  Description VARCHAR(500) NOT NULL,
  Status_Id INT NOT NULL DEFAULT 0, 
  
  FOREIGN KEY (Category_Id) REFERENCES expense_categories (Id),
  FOREIGN KEY (Property_Id) REFERENCES property (Id),
  FOREIGN KEY (Status_Id) REFERENCES payment_status (Id)
);

CREATE TABLE IF NOT EXISTS payments
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Receive_Number VARCHAR(100) NOT NULL,
  Payment_Date DATETIME NOT NULL,
  Amount DECIMAL(10,2) NOT NULL,	
  Description VARCHAR(500) NULL,
  Receive_Photo VARCHAR(1000) NOT NULL
);

CREATE TABLE IF NOT EXISTS expense_payments
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Expense_Id INT NOT NULL,
  Payment_Id INT NOT NULL,
  FOREIGN KEY (Expense_Id) REFERENCES expenses(Id),
  FOREIGN KEY (Payment_Id) REFERENCES payments(Id)
);


-- Service Expenses and Payments
SELECT 'Creating Service Expenses and Payments Tables';
CREATE TABLE IF NOT EXISTS service_types
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Service_Name VARCHAR(100) NOT NULL,
  Description VARCHAR(500) NOT NULL
);

CREATE TABLE IF NOT EXISTS service_expenses
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Service_Type_Id INT NOT NULL,
  Description VARCHAR(500) NOT NULL,
  Amount DECIMAL(10,2) NOT NULL,
  Start_Date DATETIME NOT NULL,
  Payment_Limit_Date DATETIME NOT NULL,
  Interest_Amount DECIMAL(10,2) DEFAULT 0,
  Total_Amount DECIMAL(10,2) NOT NULL,
  Status INT NOT NULL DEFAULT 1,  -- 1: Pending, 2: Paid, 3: Overdue, 4: Cancelled
  Expense_Date DATETIME NOT NULL,
  Status_Id INT NOT NULL DEFAULT 0, 
  
  FOREIGN KEY (Service_Type_Id) REFERENCES service_types(Id),
  FOREIGN KEY (Status_Id) REFERENCES payment_status (Id)
);

CREATE TABLE IF NOT EXISTS service_payments
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Receive_Number VARCHAR(100) NOT NULL,
  Payment_Date DATETIME NOT NULL,
  Amount DECIMAL(10,2) NOT NULL,	
  Description VARCHAR(500) NULL,
  Receive_Photo VARCHAR(1000) NOT NULL,
  Status_Id INT NOT NULL DEFAULT 0,
  FOREIGN KEY (Status_Id) REFERENCES payment_status (Id)
);

CREATE TABLE IF NOT EXISTS service_expense_payments
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Service_Expense_Id INT NOT NULL,
  Payment_Id INT NOT NULL,

  FOREIGN KEY (Service_Expense_Id) REFERENCES service_expenses(Id),
  FOREIGN KEY (Payment_Id) REFERENCES service_payments(Id)
);

CREATE TABLE IF NOT EXISTS styles 
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Style_Name VARCHAR(100) NOT NULL,
  Bold BOOLEAN NOT NULL DEFAULT FALSE,
  Italic BOOLEAN NOT NULL DEFAULT FALSE,
  Underline BOOLEAN NOT NULL DEFAULT FALSE,
  Font_Size INT NOT NULL DEFAULT 12,
  Font_Color VARCHAR(20) NOT NULL DEFAULT '#000000',
  Background_Color VARCHAR(20) NOT NULL DEFAULT '#FFFFFF',
  Horizontal_Alignment VARCHAR(20) NOT NULL DEFAULT 'left',
  Vertical_Alignment VARCHAR(20) NOT NULL DEFAULT 'top',
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,
  Width_Percentage INT NOT NULL DEFAULT 100
);

CREATE TABLE IF NOT EXISTS reports
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Report_Name VARCHAR(50) NOT NULL UNIQUE,
  Display_Name VARCHAR(150) NOT NULL,
  Title_Style INT NOT NULL DEFAULT -1,
  Display_Header BOOLEAN NOT NULL DEFAULT TRUE,
  Display_Footer BOOLEAN NOT NULL DEFAULT TRUE,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Title_Style) REFERENCES styles(Id)
);

CREATE TABLE IF NOT EXISTS report_roles
(
  Report_Id INT NOT NULL,
  Role_Id INT NOT NULL,

  PRIMARY KEY (Report_Id, Role_Id),
  FOREIGN KEY (Report_Id) REFERENCES reports(Id),
  FOREIGN KEY (Role_Id) REFERENCES roles(Id)
);

CREATE TABLE IF NOT EXISTS report_headers
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Report_Id INT NOT NULL,
  Display_Order INT NOT NULL,
  Style_Id INT NOT NULL DEFAULT -1,
  Display_Content TEXT NOT NULL,
  Is_Query BOOLEAN NOT NULL DEFAULT FALSE,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Report_Id) REFERENCES reports(Id),
  FOREIGN KEY (Style_Id) REFERENCES styles(Id)
);

CREATE TABLE IF NOT EXISTS report_sections
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Report_Id INT NOT NULL,
  Display_Order INT NOT NULL,
  Style_Id INT NOT NULL DEFAULT -1,
  Header_Style_Id INT NOT NULL DEFAULT -1,
  Display_Content TEXT NOT NULL,
  Is_Query BOOLEAN NOT NULL DEFAULT FALSE,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Report_Id) REFERENCES reports(Id),
  FOREIGN KEY (Header_Style_Id) REFERENCES styles(Id),
  FOREIGN KEY (Style_Id) REFERENCES styles(Id)
);

CREATE TABLE IF NOT EXISTS report_footers
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Report_Id INT NOT NULL,
  Display_Order INT NOT NULL,
  Style_Id INT NOT NULL DEFAULT -1,
  Display_Content TEXT NOT NULL,
  Is_Query BOOLEAN NOT NULL DEFAULT FALSE,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Report_Id) REFERENCES reports(Id),
  FOREIGN KEY (Style_Id) REFERENCES styles(Id)
);

CREATE TABLE IF NOT EXISTS report_params(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Report_Id INT NOT NULL,
  Param_Name VARCHAR(100) NOT NULL,
  Param_Type VARCHAR(50) NOT NULL,
  Param_Description VARCHAR(500) NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,
  FOREIGN KEY (Report_Id) REFERENCES reports(Id)
);

CREATE TABLE IF NOT EXISTS report_audits
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Report_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Parameters TEXT NOT NULL,
  Execution_Date DATETIME NOT NULL,

  FOREIGN KEY (Report_Id) REFERENCES reports(Id),
  FOREIGN KEY (User_Id) REFERENCES users(Id)
);



-- Version 1.3 2024.10.24
SELECT 'Creating Versions Table';
CREATE TABLE IF NOT EXISTS versions
(
  Version VARCHAR(20) NOT NULL PRIMARY KEY,
  Last_Updated DATETIME NOT NULL
);

INSERT INTO versions (Version, Last_Updated) VALUES ('0.1', NOW());

-- Insertar roles si no existen
-- INSERT INTO roles (Rol_Name, Description)
-- SELECT 'Defecto', 'Rol por defecto para operaciones b�sicas (health check, login)'
-- WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'Defecto');

-- INSERT INTO roles (Rol_Name, Description)
-- SELECT 'Habitante', 'Residente que puede ver su informaci�n personal y propiedades asignadas'
-- WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'Habitante');

-- INSERT INTO roles (Rol_Name, Description)
-- SELECT 'Administrador', 'Administrador del sistema con permisos completos de CRUD en Usuarios y Propiedades'
-- WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'Administrador');

-- INSERT INTO roles (Rol_Name, Description)
-- SELECT 'RoleAdmin', 'Administrador de roles con permisos para gestionar roles de usuarios'
-- WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'RoleAdmin');

--
-- Insert Default users

SELECT 'Inserting Default Users and Roles for version 0.3.2';
INSERT INTO users(Login, User_Name, Last_Name, Legal_Id, Start_Date, Password)
VALUES 
('usa', 'sa', 'sa', '-1', NOW(), '$2a$11$GUefHVMEvy8hXQHvnuVRcu3ZsTSWJTSypz6Ml2enRrkQVhNHDn3aG'),-- sa
('uadmin', 'admin', 'admin', '-1', NOW(), '$2a$11$frjA/I.pkPTZrUEQXeHdWeXjebMRBhgF4v3XeFGCfccuHyGKdwpzK'), -- admin
('udirector', 'director', 'director', '-1', NOW(), '$2a$11$TVPKa..EYzCIksqHx321IOYLO1qzhTCVGHii71706W5iOZC7N9esa'),-- director
('uhabitante', 'habitante', 'habitante', '-1', NOW(), '$2a$11$bXnuTxtq0wGe2C6oN8QhMubmwBFQyEEq3JfT9UvB6rWJrD6EmPmjS'),-- habitante
('uauxiliar', 'auxiliar', 'auxiliar', '-1', NOW(), '$2a$11$OQb6S.GLFsVNlTbia56dLegk64JYiBCi4rk1P4855xJofIp/Up3Ey'),-- auxiliar
('useguridad', 'seguridad', 'seguridad', '-1', NOW(), '$2a$11$m20UOSFeHoZ8dj6sDOATc.lecO6H58u40GadDdaetgROcUQklgq/W');-- seguridad
-- Insert Default roles
INSERT INTO roles(Id, Rol_Name, Description) VALUES
(1, 'super', 'Super Administrador'),
(2, 'admin', 'Administrador Condominio'),
(3, 'director', 'Miembro de la directiva'),
(4, 'habitante', 'Habitante regular'),
(5, 'auxiliar', 'Auxiliar de Administracion'),
(6, 'seguridad', 'Guardia de Seguridad');

-- Assign Roles to default users
INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
SELECT 1, u.Id, NOW() FROM users u WHERE u.Login = 'usa';

INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
SELECT 2, u.Id, NOW() FROM users u WHERE u.Login = 'uadmin';

INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
SELECT 3, u.Id, NOW() FROM users u WHERE u.Login = 'udirector';

INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
SELECT 4, u.Id, NOW() FROM users u WHERE u.Login = 'uhabitante';

INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
SELECT 5, u.Id, NOW() FROM users u WHERE u.Login = 'uauxiliar';

INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
SELECT 6, u.Id, NOW() FROM users u WHERE u.Login = 'useguridad';

INSERT INTO versions(Version, Last_Updated) VALUES('0.3.2', NOW());

-- Create payment status
INSERT INTO payment_status(Status_Description)
VALUES 
    ('Pendiente'),
    ('Pagado'),
    ('Verificado'),
    ('Anulado');


-- v1.0.1 - 2026.03.02
SELECT 'Creating New Tables for Version 2.0';
INSERT INTO versions(Version, Last_Updated) VALUES('1.0.0', NOW());

CREATE TABLE IF NOT EXISTS resources
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(150) NOT NULL,
  Description VARCHAR(500) NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,
  Photo VARCHAR(1000) NOT NULL
);

CREATE TABLE IF NOT EXISTS resource_costs
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Resource_Id INT NOT NULL,
  Booking_Price DECIMAL(10,2) NOT NULL,
  Booking_Warranty_Cost DECIMAL(10,2) NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,

  FOREIGN KEY (Resource_Id) REFERENCES resources(Id)
);

CREATE TABLE IF NOT EXISTS resource_bookings
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Resource_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Property_Id INT NOT NULL,
  Status_Id INT NOT NULL DEFAULT 0,
  Booking_Date DATETIME NOT NULL,
  Booking_End_Date DATETIME NOT NULL,
  Booking_Price DECIMAL(10,2) NOT NULL DEFAULT 0,
  Booking_Warranty_Cost DECIMAL(10,2) NOT NULL DEFAULT 0,
  Booking_Description VARCHAR(500) NULL,
  Booking_Photo VARCHAR(1000) NULL,

  FOREIGN KEY (Resource_Id) REFERENCES resources(Id),
  FOREIGN KEY (User_Id) REFERENCES users(Id),
  FOREIGN KEY (Property_Id) REFERENCES property(Id),
  FOREIGN KEY (Status_Id) REFERENCES payment_status(Id)
);

CREATE TABLE IF NOT EXISTS incident_types
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Type VARCHAR(100) NOT NULL,
  Description VARCHAR(500) NOT NULL
);

CREATE TABLE IF NOT EXISTS incident_costs
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Incident_Type_Id INT NOT NULL,
  Cost DECIMAL(10,2) NOT NULL,
  Start_Date DATETIME NOT NULL,
  End_Date DATETIME NULL,
  Description VARCHAR(500) NOT NULL,

  FOREIGN KEY (Incident_Type_Id) REFERENCES incident_types(Id)
);

CREATE TABLE IF NOT EXISTS incidents
(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Incident_Type_Id INT NOT NULL,
  User_Id INT NOT NULL,
  Property_Id INT NOT NULL,
  Status_Id INT NOT NULL DEFAULT 0,
  Incident_Date DATETIME NOT NULL,
  Incident_Description VARCHAR(500) NULL,
  Incident_Photo VARCHAR(1000) NULL,

  FOREIGN KEY (Incident_Type_Id) REFERENCES incident_types(Id),
  FOREIGN KEY (User_Id) REFERENCES users(Id),
  FOREIGN KEY (Property_Id) REFERENCES property(Id),
  FOREIGN KEY (Status_Id) REFERENCES payment_status(Id)
);

INSERT INTO versions(Version, Last_Updated) VALUES('1.0.1', NOW());

-- ============================================================
-- Script para agregar la columna Booking_End_Date a resource_bookings
-- Compatible con MySQL 5.7+
-- Solo la agrega si no existe
-- Luego actualiza los registros existentes SOLO UNA VEZ
-- ============================================================

-- Verificar si la columna ya existe
SET @columnExists = (
    SELECT COUNT(*)
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'resource_bookings' 
    AND COLUMN_NAME = 'Booking_End_Date'
    AND TABLE_SCHEMA = DATABASE()
);

-- 1. Agregar la columna solo si no existe
SET @alterSQL = IF(
    @columnExists = 0,
    'ALTER TABLE resource_bookings ADD COLUMN Booking_End_Date DATETIME NULL',
    'SELECT "Columna Booking_End_Date ya existe, no se realizarán cambios" as Mensaje'
);

PREPARE stmt FROM @alterSQL;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 2. Actualizar registros existentes SOLO si la columna fue recién agregada
-- Esto solo se ejecuta si @columnExists = 0 (columna no existía antes)
SET @updateSQL = IF(
    @columnExists = 0,
    'UPDATE resource_bookings SET Booking_End_Date = CONCAT(DATE(Booking_Date), \' 23:59:59\') WHERE Booking_End_Date IS NULL',
    'SELECT "No se requiere actualización de datos" as Mensaje'
);

PREPARE stmt FROM @updateSQL;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- 3. Cambiar la columna a NOT NULL solo si fue recién agregada
SET @modifySQL = IF(
    @columnExists = 0,
    'ALTER TABLE resource_bookings MODIFY COLUMN Booking_End_Date DATETIME NOT NULL',
    'SELECT "Columna ya configurada correctamente" as Mensaje'
);

PREPARE stmt FROM @modifySQL;
EXECUTE stmt;

INSERT INTO versions(Version, Last_Updated) VALUES('1.0.2', NOW());
-- Insertando estilos por defecto para los reportes del sistema.
SET @startdate = NOW();

INSERT INTO styles 
(Style_Name , Bold, Italic, Underline , Font_Size , Font_Color  , Background_Color, Horizontal_Alignment, Vertical_Alignment, Start_Date, Width_Percentage)
VALUES
('Texto'    , 0   , 0     , 0         , 13        , '#000000' , '#FFFFFF'     , 'center'            , 'middle', @startdate, 100),
('H1'       , 1   , 0     , 0         , 24        , '#FFFFFF' , '#1a1a1a'     , 'center'            , 'middle', @startdate, 100),
('H2'       , 1   , 0     , 0         , 18        , '#1a1a1a' , '#E8E8E8'     , 'left'              , 'middle', @startdate, 100),
('H3'       , 1   , 0     , 0         , 16        , '#1a1a1a' , '#F5F5F5'     , 'left'              , 'middle', @startdate, 100),
('Footer'   , 0   , 0     , 0         , 11        , '#666666' , '#FFFFFF'     , 'center'            , 'middle', @startdate, 100);


-- Insertando reporte de recibo de pago de expensa
INSERT INTO REPORTS (Report_Name, Display_Name, Title_Style , Display_Header, Display_Footer, Start_Date)
VALUES ('ReciboPagoDeExpensa', 'Recibo de Pago de Expensa'  , 1, 1, 1, @startdate);


SET @reportId = LAST_INSERT_ID();

INSERT INTO report_headers 
(Report_Id, Display_Order , Style_Id, Is_Query, Start_Date, Display_Content)
VALUES 
(@reportId, 0             , 2       , 0       , @startDate,'RECIBO PAGO DE EXPENSA'),
(@reportId, 1             , 4       , 1       , @startDate,' SELECT \'Id Expensa\' as f1, \'Categoria\' as f2, \'Descripcion\' as f3, \'Monto\' as f4, \'Interes\' as f5, \'Fecha limite\' as f6, \'Departamento\' as f7, \'Monto Pagado\' as f8, \'Recibo\' as f9'),
(@reportId, 2             , 1       , 1       , @startDate,'SELECT e.id AS f1, ec.Category AS f2, e.Description AS f3, e.Amount AS f4, e.Interest_Amount AS f5, e.Payment_Limit_Date AS f6, pr.Tower + \' \' + pr.Floor + \' \' + pr.Code AS f7, p.Amount AS f8, p.Receive_Number AS f9 FROM expenses e JOIN expense_payments ep ON e.id = ep.expense_id JOIN payments p ON ep.payment_id = p.id JOIN expense_categories ec ON e.Category_Id = ec.id JOIN property pr ON e.Property_Id = pr.id WHERE e.Id = @expenseId;');

INSERT INTO report_footers 
(Report_Id, Display_Order , Style_Id, Is_Query, Start_Date, Display_Content)
VALUES 
(@reportId, 0             , 5       , 0       , @startDate, 'Gracias por su pago.'),
(@reportId, 1             , 5       , 0       , @startDate, 'CONDOMINIO - AMBAR'  );

INSERT INTO report_roles (Report_Id, Role_Id)
SELECT @reportId, `Id` FROM roles
WHERE Rol_Name NOT IN ('seguridad');

INSERT INTO report_params (Report_Id, Param_Name, Param_Type, Param_Description, Start_Date, End_Date)
VALUES 
(@reportId, 'expenseId', 'INT', 'Id de la expensa', @startDate, NULL);

INSERT INTO versions(Version, Last_Updated) VALUES('1.0.3', NOW());