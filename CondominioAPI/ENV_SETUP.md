# Configuraci�n de Variables de Entorno

Este proyecto utiliza un archivo `.env` para manejar informaci�n sensible como credenciales de base de datos y claves secretas.

## Configuraci�n Inicial

1. **Copia el archivo de ejemplo:**
   ```bash
   cp CondominioAPI/.env.example CondominioAPI/.env
   ```

2. **Edita el archivo `.env` con tus valores:**
   ```
   DB_SERVER=tu_servidor_mysql
   DB_NAME=nombre_base_datos
   DB_USER=usuario_base_datos
   DB_PASSWORD=contrase�a_base_datos
   JWT_SECRET_KEY=tu_clave_secreta_jwt_aqui
   CORS_ALLOWED_ORIGIN=http://localhost:5173
   ```

## Variables de Entorno

### Base de Datos
- `DB_SERVER`: Servidor de MySQL (ej: localhost)
- `DB_NAME`: Nombre de la base de datos
- `DB_USER`: Usuario de MySQL
- `DB_PASSWORD`: Contrase�a de MySQL

### JWT
- `JWT_SECRET_KEY`: Clave secreta para firmar tokens JWT (m�nimo 32 caracteres recomendado)

### CORS
- `CORS_ALLOWED_ORIGIN`: URL permitida para solicitudes CORS (frontend)

## Seguridad

?? **IMPORTANTE**: El archivo `.env` contiene informaci�n sensible y **NO debe ser subido al repositorio**. Este archivo est� incluido en `.gitignore`.

## Producci�n

Para entornos de producci�n, se recomienda:
1. Usar variables de entorno del sistema operativo o servicio de hosting
2. Usar servicios de gesti�n de secretos (Azure Key Vault, AWS Secrets Manager, etc.)
3. Generar una clave JWT segura de al menos 256 bits
