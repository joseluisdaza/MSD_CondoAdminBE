@echo off
echo ================================
echo   Diagnóstico CondominioAPI
echo ================================
echo.

echo 1. Verificando archivos necesarios...
if exist "CondominioAPI.exe" (
    echo ✅ CondominioAPI.exe encontrado
) else (
    echo ❌ CondominioAPI.exe NO encontrado
    goto :error
)

if exist ".env" (
    echo ✅ .env encontrado
) else (
    echo ❌ .env NO encontrado
    if exist ".env.example" (
        echo 📝 Copiando .env.example como .env...
        copy .env.example .env > nul
    ) else (
        echo ❌ .env.example tampoco existe
        goto :error
    )
)

echo.
echo 2. Verificando .NET Runtime...
dotnet --version > nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ .NET Runtime disponible
    dotnet --version
) else (
    echo ⚠️ .NET Runtime no encontrado (normal si usas self-contained)
)

echo.
echo 3. Creando carpeta de logs...
if not exist "Logs" mkdir Logs
echo ✅ Carpeta Logs creada

echo.
echo 4. Intentando ejecutar la aplicación...
echo ⏳ Iniciando CondominioAPI.exe...
echo    (Si se cierra inmediatamente, revisa el error arriba)
echo.

CondominioAPI.exe

goto :end

:error
echo.
echo ❌ Error en la configuración
echo    Revisa los archivos faltantes arriba
pause
exit /b 1

:end
echo.
echo 👋 Aplicación terminada
pause