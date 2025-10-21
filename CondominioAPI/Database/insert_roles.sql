-- Script para crear los roles iniciales del sistema CondominioAPI
-- Ejecutar este script después de crear la base de datos

USE condominio;

-- Insertar roles si no existen
INSERT INTO roles (Rol_Name, Description)
SELECT 'Defecto', 'Rol por defecto para operaciones básicas (health check, login)'
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'Defecto');

INSERT INTO roles (Rol_Name, Description)
SELECT 'Habitante', 'Residente que puede ver su información personal y propiedades asignadas'
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'Habitante');

INSERT INTO roles (Rol_Name, Description)
SELECT 'Administrador', 'Administrador del sistema con permisos completos de CRUD en Usuarios y Propiedades'
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'Administrador');

INSERT INTO roles (Rol_Name, Description)
SELECT 'RoleAdmin', 'Administrador de roles con permisos para gestionar roles de usuarios'
WHERE NOT EXISTS (SELECT 1 FROM roles WHERE Rol_Name = 'RoleAdmin');

-- Verificar roles creados
SELECT * FROM roles;

-- Ejemplo: Asignar rol de Administrador al primer usuario (ajusta el User_Id según tu BD)
-- INSERT INTO user_roles (Role_Id, User_Id, Start_Date, End_Date)
-- SELECT 
--     (SELECT Id FROM roles WHERE Rol_Name = 'Administrador'),
--     1, -- ID del usuario administrador
--     NOW(),
--     NULL
-- WHERE NOT EXISTS (
--     SELECT 1 FROM user_roles 
--     WHERE User_Id = 1 
--     AND Role_Id = (SELECT Id FROM roles WHERE Rol_Name = 'Administrador')
--     AND End_Date IS NULL
-- );
