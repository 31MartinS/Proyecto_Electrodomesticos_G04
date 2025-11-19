# Script para crear la estructura de carpetas necesaria para imágenes

$projectRoot = $PSScriptRoot
$wwwrootPath = Join-Path $projectRoot "wwwroot"
$imagesPath = Join-Path $wwwrootPath "images"
$productsPath = Join-Path $imagesPath "products"

Write-Host "Creando estructura de carpetas..." -ForegroundColor Cyan

# Crear wwwroot si no existe
if (-not (Test-Path $wwwrootPath)) {
    New-Item -ItemType Directory -Path $wwwrootPath -Force | Out-Null
    Write-Host "✓ Carpeta wwwroot creada" -ForegroundColor Green
} else {
    Write-Host "✓ Carpeta wwwroot ya existe" -ForegroundColor Yellow
}

# Crear images si no existe
if (-not (Test-Path $imagesPath)) {
    New-Item -ItemType Directory -Path $imagesPath -Force | Out-Null
    Write-Host "✓ Carpeta images creada" -ForegroundColor Green
} else {
    Write-Host "✓ Carpeta images ya existe" -ForegroundColor Yellow
}

# Crear products si no existe
if (-not (Test-Path $productsPath)) {
    New-Item -ItemType Directory -Path $productsPath -Force | Out-Null
    Write-Host "✓ Carpeta products creada" -ForegroundColor Green
} else {
    Write-Host "✓ Carpeta products ya existe" -ForegroundColor Yellow
}

Write-Host "`nEstructura de carpetas lista:" -ForegroundColor Cyan
Write-Host "  $productsPath" -ForegroundColor White

Write-Host "`nLas imágenes se guardarán en: wwwroot/images/products/" -ForegroundColor Green
