USE Condominio;

-- Verificar la versión actual y ejecutar solo si es menor a 0.3.2
DELIMITER $$

DROP PROCEDURE IF EXISTS UpdateToVersion_0_3_2$$

CREATE PROCEDURE UpdateToVersion_0_3_2()
BEGIN
    DECLARE current_version VARCHAR(20);
    DECLARE current_major INT DEFAULT 0;
    DECLARE current_minor INT DEFAULT 0;
    DECLARE current_patch INT DEFAULT 0;
    DECLARE target_major INT DEFAULT 0;
    DECLARE target_minor INT DEFAULT 3;
    DECLARE target_patch INT DEFAULT 2;
    DECLARE should_update BOOLEAN DEFAULT FALSE;
    
    -- Obtener la versión más reciente
    SELECT Version INTO current_version 
    FROM Versions 
    ORDER BY Last_Updated DESC 
    LIMIT 1;
    
    -- Si no hay versión, ejecutar actualización
    IF current_version IS NULL THEN
        SET should_update = TRUE;
    ELSE
        -- Extraer partes de la versión haciendo split por el punto
        SET current_major = CAST(SUBSTRING_INDEX(current_version, '.', 1) AS UNSIGNED);
        SET current_minor = CAST(SUBSTRING_INDEX(SUBSTRING_INDEX(current_version, '.', 2), '.', -1) AS UNSIGNED);
        SET current_patch = CAST(SUBSTRING_INDEX(current_version, '.', -1) AS UNSIGNED);
        
        -- Comparar versión actual con 0.3.2
        IF current_major < target_major THEN
            SET should_update = TRUE;
        ELSEIF current_major = target_major THEN
            IF current_minor < target_minor THEN
                SET should_update = TRUE;
            ELSEIF current_minor = target_minor THEN
                IF current_patch < target_patch THEN
                    SET should_update = TRUE;
                END IF;
            END IF;
        END IF;
    END IF;
    
    -- Ejecutar actualización solo si la versión es menor
    IF should_update THEN
        -- Insertar nueva versión
        INSERT INTO Versions(Version, Last_Updated) VALUES('0.3.2', NOW());
        
        -- Insert Default users
        INSERT INTO users(Login, User_Name, Last_Name, Legal_Id, Start_Date, Password)
        VALUES 
        ('usa', 'sa', 'sa', '-1', NOW(), 'sa'),
        ('uadmin', 'admin', 'admin', '-1', NOW(), 'admin'),
        ('udirector', 'director', 'director', '-1', NOW(), 'director'),
        ('uhabitante', 'habitante', 'habitante', '-1', NOW(), 'habitante'),
        ('uauxiliar', 'auxiliar', 'auxiliar', '-1', NOW(), 'auxiliar'),
        ('useguridad', 'seguridad', 'seguridad', '-1', NOW(), 'seguridad');
        
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
        SELECT 1, u.id, NOW() FROM users u WHERE u.Login = 'usa';
        
        INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
        SELECT 2, u.id, NOW() FROM users u WHERE u.Login = 'uadmin';
        
        INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
        SELECT 3, u.id, NOW() FROM users u WHERE u.Login = 'udirector';
        
        INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
        SELECT 4, u.id, NOW() FROM users u WHERE u.Login = 'uhabitante';
        
        INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
        SELECT 5, u.id, NOW() FROM users u WHERE u.Login = 'uauxiliar';
        
        INSERT INTO user_roles(Role_Id, User_Id, Start_Date)
        SELECT 6, u.id, NOW() FROM users u WHERE u.Login = 'useguridad';
        
        SELECT CONCAT('✅ Actualización a versión 0.3.2 completada desde versión: ', IFNULL(current_version, 'SIN VERSION')) AS Resultado;
    ELSE
        SELECT CONCAT('⏭️ No se requiere actualización. Versión actual: ', current_version, ' (ya es >= 0.3.2)') AS Resultado;
    END IF;
    
END$$

DELIMITER ;

-- Ejecutar el procedimiento
CALL UpdateToVersion_0_3_2();

-- Opcional: Eliminar el procedimiento después de usarlo
-- DROP PROCEDURE IF EXISTS UpdateToVersion_0_3_2;