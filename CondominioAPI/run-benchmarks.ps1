#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Script para ejecutar benchmarks de CondoAdmin Backend
.DESCRIPTION
    Facilita la ejecución de benchmarks con BenchmarkDotNet con diferentes opciones
.PARAMETER Filter
    Filtrar benchmarks por nombre (ej: *PasswordHasher*)
.PARAMETER All
    Ejecutar todos los benchmarks
.PARAMETER PasswordHasher
    Ejecutar solo benchmarks de PasswordHasher
.PARAMETER Linq
    Ejecutar solo benchmarks de LINQ
.PARAMETER DataStructure
    Ejecutar solo benchmarks de estructuras de datos
.PARAMETER Json
    Ejecutar solo benchmarks de JSON
.PARAMETER Dto
    Ejecutar solo benchmarks de DTOs
.PARAMETER List
    Listar todos los benchmarks disponibles
.PARAMETER Quick
    Ejecución rápida (1 warmup, 3 iteraciones)
.PARAMETER Precise
    Ejecución precisa (5 warmups, 10 iteraciones)
.EXAMPLE
    ./run-benchmarks.ps1 -PasswordHasher
    ./run-benchmarks.ps1 -Linq -Precise
    ./run-benchmarks.ps1 -All
#>

param(
    [string]$Filter,
    [switch]$All,
    [switch]$PasswordHasher,
    [switch]$Linq,
    [switch]$DataStructure,
    [switch]$Json,
    [switch]$Dto,
    [switch]$List,
    [switch]$Quick,
    [switch]$Precise
)

$BenchmarkProject = "Condominio.Benchmarks"
$ProjectPath = ".\Condominio.Benchmarks"

Write-Host "🚀 CondoAdmin Backend Benchmarks Runner" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Determinar filtro
if ($PasswordHasher) {
    $Filter = "*PasswordHasher*"
    Write-Host "📊 Ejecutando: PasswordHasher Benchmarks" -ForegroundColor Yellow
}
elseif ($Linq) {
    $Filter = "*Linq*"
    Write-Host "📊 Ejecutando: LINQ Benchmarks" -ForegroundColor Yellow
}
elseif ($DataStructure) {
    $Filter = "*DataStructure*"
    Write-Host "📊 Ejecutando: DataStructure Benchmarks" -ForegroundColor Yellow
}
elseif ($Json) {
    $Filter = "*JsonSerialization*"
    Write-Host "📊 Ejecutando: JSON Serialization Benchmarks" -ForegroundColor Yellow
}
elseif ($Dto) {
    $Filter = "*Dto*"
    Write-Host "📊 Ejecutando: DTO Benchmarks" -ForegroundColor Yellow
}
elseif ($All) {
    Write-Host "📊 Ejecutando: TODOS los benchmarks" -ForegroundColor Yellow
}
elseif ($List) {
    Write-Host "📋 Listando benchmarks disponibles..." -ForegroundColor Yellow
}
elseif ($Filter) {
    Write-Host "📊 Filtro: $Filter" -ForegroundColor Yellow
}
else {
    Write-Host "Modo: TODOS los benchmarks (por defecto)" -ForegroundColor Yellow
}

Write-Host ""

# Construir argumentos
$args = @("run", "--configuration", "Release")

# Agregar filtro si existe
if ($Filter) {
    $args += @("--", "--filter", $Filter)
}
elseif ($List) {
    $args += @("--", "--list")
}
else {
    # Sin filtro, ejecutar todos
}

# Agregar opciones de ejecución
if ($Quick) {
    $args += "-w 1 -i 3"
    Write-Host "⚡ Modo: Rápido (1 warmup, 3 iteraciones)" -ForegroundColor Cyan
}
elseif ($Precise) {
    $args += "-w 5 -i 10"
    Write-Host "🎯 Modo: Preciso (5 warmups, 10 iteraciones)" -ForegroundColor Cyan
}
else {
    Write-Host "📈 Modo: Estándar (3 warmups, 5 iteraciones)" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "⏱️  Iniciando benchmarks..." -ForegroundColor Green
Write-Host ""

# Ejecutar dotnet
cd $ProjectPath
dotnet @args

Write-Host ""
Write-Host "✅ Benchmarks completados" -ForegroundColor Green
Write-Host ""
Write-Host "📁 Resultados guardados en:" -ForegroundColor Cyan
Write-Host "   bin/Release/net8.0/BenchmarkDotNet.Artifacts/" -ForegroundColor Gray
Write-Host ""
