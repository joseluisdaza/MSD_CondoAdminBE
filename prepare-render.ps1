# Render Deployment Helper Script
# Este script te ayuda a preparar y verificar tu deployment en Render

param(
    [switch]$TestLocal,
    [switch]$GenerateJWT,
    [switch]$CheckDocker,
    [switch]$CreateEnvTemplate,
    [switch]$TestConnection,
    [string]$RenderURL = ""
)

# Colores para output
function Write-Success { param($msg) Write-Host $msg -ForegroundColor Green }
function Write-Info { param($msg) Write-Host $msg -ForegroundColor Cyan }
function Write-Warning { param($msg) Write-Host $msg -ForegroundColor Yellow }
function Write-Error { param($msg) Write-Host $msg -ForegroundColor Red }

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Render Deployment Helper - Condominio API" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ============================================
# GENERAR JWT SECRET KEY
# ============================================
if ($GenerateJWT) {
    Write-Info "Generando JWT Secret Key segura..."
    $jwtKey = -join ((65..90) + (97..122) + (48..57) | Get-Random -Count 64 | ForEach-Object {[char]$_})
    Write-Success "JWT Secret Key generada:"
    Write-Host $jwtKey -ForegroundColor Yellow
    Write-Host ""
    Write-Info "Copia esta clave y úsala en Render como variable de entorno JWT_SECRET_KEY"
    Set-Clipboard -Value $jwtKey
    Write-Success "✓ Clave copiada al clipboard"
    exit 0
}

# ============================================
# VERIFICAR DOCKER
# ============================================
if ($CheckDocker) {
    Write-Info "Verificando Docker Desktop..."
    
    try {
        $dockerVersion = docker --version
        Write-Success "✓ Docker instalado: $dockerVersion"
        
        $dockerRunning = docker ps 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Success "✓ Docker está corriendo"
        } else {
            Write-Error "✗ Docker no está corriendo"
            Write-Info "Inicia Docker Desktop y vuelve a intentar"
            exit 1
        }
    }
    catch {
        Write-Error "✗ Docker no está instalado"
        Write-Info "Descarga Docker Desktop desde: https://www.docker.com/products/docker-desktop/"
        exit 1
    }
    exit 0
}

# ============================================
# CREAR TEMPLATE DE VARIABLES DE ENTORNO
# ============================================
if ($CreateEnvTemplate) {
    Write-Info "Creando template de variables de entorno para Render..."
    
    $envTemplate = @"
# ====================================================
# VARIABLES DE ENTORNO PARA RENDER
# ====================================================
# Copia estos valores en Render Dashboard → Environment
# No subas este archivo a GitHub con valores reales
# ====================================================

# MySQL Database Configuration (FreeSQLDatabase o db4free)
DB_SERVER=sql.freesqldatabase.com
DB_NAME=sql12345678
DB_USER=sql12345678
DB_PASSWORD=tu_password_mysql_aqui

# JWT Configuration (genera una clave con: .\prepare-render.ps1 -GenerateJWT)
JWT_SECRET_KEY=tu_clave_jwt_segura_minimo_32_caracteres_aqui

# CORS Configuration
CORS_ALLOWED_ORIGIN=*

# Logging (Render usa /tmp para archivos temporales)
LOG_PATH=/tmp/logs/log-.txt

# ASP.NET Core Environment
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:8080

# ====================================================
# INSTRUCCIONES:
# ====================================================
# 1. Reemplaza los valores con tus credenciales reales
# 2. En Render Dashboard, ve a tu servicio
# 3. Click en "Environment" (tab)
# 4. Agrega cada variable con su valor
# 5. Click en "Save Changes"
# ====================================================
"@

    $envPath = Join-Path $PSScriptRoot ".env.render"
    Set-Content -Path $envPath -Value $envTemplate
    Write-Success "✓ Template creado en: $envPath"
    Write-Info "Edita este archivo con tus credenciales reales"
    exit 0
}

# ============================================
# PROBAR BUILD LOCAL CON DOCKER
# ============================================
if ($TestLocal) {
    Write-Info "Probando build de Docker localmente..."
    Write-Warning "Esto puede tomar 5-10 minutos..."
    Write-Host ""
    
    # Verificar que Docker esté corriendo
    docker ps 2>&1 | Out-Null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Docker no está corriendo. Inicia Docker Desktop."
        exit 1
    }
    
    # Navegar a la carpeta del Dockerfile
    $dockerfilePath = Join-Path $PSScriptRoot "CondominioAPI\Dockerfile"
    $contextPath = Join-Path $PSScriptRoot "CondominioAPI"
    
    if (-not (Test-Path $dockerfilePath)) {
        Write-Error "Dockerfile no encontrado en: $dockerfilePath"
        exit 1
    }
    
    Write-Info "Construyendo imagen Docker..."
    docker build -t condominio-api:test `
        --build-arg GIT_BRANCH=main `
        -f $dockerfilePath `
        $contextPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "✓ Build exitoso!"
        Write-Host ""
        Write-Info "Para correr localmente:"
        Write-Host "docker run -p 8080:8080 --env-file .env condominio-api:test" -ForegroundColor Yellow
    } else {
        Write-Error "✗ Build falló. Revisa los errores arriba."
        exit 1
    }
    exit 0
}

# ============================================
# PROBAR CONEXIÓN A RENDER
# ============================================
if ($TestConnection) {
    if ([string]::IsNullOrEmpty($RenderURL)) {
        Write-Error "Debes proporcionar la URL de Render"
        Write-Info "Uso: .\prepare-render.ps1 -TestConnection -RenderURL 'https://condominio-api.onrender.com'"
        exit 1
    }
    
    Write-Info "Probando conexión a Render..."
    Write-Info "URL: $RenderURL"
    Write-Host ""
    
    # Probar Swagger
    try {
        Write-Info "Probando Swagger UI..."
        $swaggerUrl = "$RenderURL/swagger/index.html"
        $response = Invoke-WebRequest -Uri $swaggerUrl -Method GET -TimeoutSec 60
        
        if ($response.StatusCode -eq 200) {
            Write-Success "✓ Swagger UI accesible"
            Write-Info "URL: $swaggerUrl"
        }
    }
    catch {
        Write-Warning "⚠ Swagger UI no accesible (puede estar deshabilitado en producción)"
    }
    
    Write-Host ""
    
    # Probar endpoint de API
    try {
        Write-Info "Probando endpoint de API..."
        $apiUrl = "$RenderURL/api/users"
        $response = Invoke-RestMethod -Uri $apiUrl -Method GET -TimeoutSec 60
        
        Write-Success "✓ API respondiendo correctamente"
        Write-Info "Endpoint: $apiUrl"
        Write-Host ""
        Write-Success "🎉 ¡Deployment exitoso!"
    }
    catch {
        Write-Error "✗ API no está respondiendo"
        Write-Info "Error: $($_.Exception.Message)"
        Write-Host ""
        Write-Info "Posibles causas:"
        Write-Host "  1. El servicio aún se está iniciando (espera 1-2 minutos)" -ForegroundColor Yellow
        Write-Host "  2. Variables de entorno incorrectas" -ForegroundColor Yellow
        Write-Host "  3. Error en la base de datos" -ForegroundColor Yellow
        Write-Host ""
        Write-Info "Revisa los logs en Render Dashboard"
    }
    exit 0
}

# ============================================
# MENÚ DE AYUDA
# ============================================
Write-Host "Uso: .\prepare-render.ps1 [OPCIÓN]" -ForegroundColor Yellow
Write-Host ""
Write-Host "Opciones disponibles:" -ForegroundColor Cyan
Write-Host ""
Write-Host "  -GenerateJWT" -ForegroundColor Green
Write-Host "    Genera una clave JWT segura para usar en Render"
Write-Host ""
Write-Host "  -CheckDocker" -ForegroundColor Green
Write-Host "    Verifica que Docker Desktop esté instalado y corriendo"
Write-Host ""
Write-Host "  -CreateEnvTemplate" -ForegroundColor Green
Write-Host "    Crea un template de variables de entorno (.env.render)"
Write-Host ""
Write-Host "  -TestLocal" -ForegroundColor Green
Write-Host "    Construye la imagen Docker localmente para probar"
Write-Host ""
Write-Host "  -TestConnection -RenderURL <url>" -ForegroundColor Green
Write-Host "    Prueba la conexión a tu deployment en Render"
Write-Host ""
Write-Host "Ejemplos:" -ForegroundColor Cyan
Write-Host ""
Write-Host "  .\prepare-render.ps1 -GenerateJWT" -ForegroundColor Yellow
Write-Host "  .\prepare-render.ps1 -CheckDocker" -ForegroundColor Yellow
Write-Host "  .\prepare-render.ps1 -TestLocal" -ForegroundColor Yellow
Write-Host "  .\prepare-render.ps1 -TestConnection -RenderURL 'https://condominio-api.onrender.com'" -ForegroundColor Yellow
Write-Host ""
