USE Condominio2;

-- Verificar la versión actual y ejecutar solo si es menor a 0.3.2
DELIMITER $$

DROP PROCEDURE IF EXISTS UpdateToVersion_0_3_3$$

CREATE PROCEDURE UpdateToVersion_0_3_3()
BEGIN
    DECLARE current_version VARCHAR(20);
    DECLARE current_major INT DEFAULT 0;
    DECLARE current_minor INT DEFAULT 0;
    DECLARE current_patch INT DEFAULT 0;
    DECLARE target_major INT DEFAULT 0;
    DECLARE target_minor INT DEFAULT 3;
    DECLARE target_patch INT DEFAULT 3;
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
        -- Agregar tabla Audit_Logs        
        CREATE TABLE IF NOT EXISTS Audit_Logs
        (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            User_Id INT NOT NULL,
            Action VARCHAR(100) NOT NULL, -- insert, Update, Delete, Login, Logout, etc.
            Table_Name VARCHAR(100) NULL,
            Message TEXT NOT NULL,
            Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            FOREIGN KEY (User_Id) REFERENCES users(Id)
        );

        CREATE INDEX idx_auditlogs_userid ON Audit_Logs(User_Id);
        CREATE INDEX idx_auditlogs_timestamp ON Audit_Logs(Timestamp);
        CREATE INDEX idx_auditlogs_tablename ON Audit_Logs(Table_Name);
        
        INSERT INTO Versions(Version, Last_Updated) VALUES('0.3.3', NOW());
        SELECT CONCAT('✅ Actualización a la versión 0.3.3 completada exitosamente desde la versión ', current_version) AS Resultado;        
    ELSE
        SELECT CONCAT('⏭️ No se requiere actualización. Versión actual: ', current_version, ' (ya es >= 0.3.3)') AS Resultado;
    END IF;
END$$

DELIMITER ;

-- Ejecutar el procedimiento
CALL UpdateToVersion_0_3_3();

-- Opcional: Eliminar el procedimiento después de usarlo
-- DROP PROCEDURE IF EXISTS UpdateToVersion_0_3_3;