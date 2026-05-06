-- Database Schema for Condominio Management System
-- This script creates the necessary tables and relationships for managing users, roles, properties, expenses, and payments.
-- It ensures that the database and tables are created only if they do not already exist.
-- v0.1

 CREATE DATABASE IF NOT EXISTS Condominio2;
 USE Condominio2;
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


-- v2.0 - 2026.03.02
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