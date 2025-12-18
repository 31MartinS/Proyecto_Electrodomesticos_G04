# Script para configurar el cliente móvil automáticamente
# Ejecutar como Administrador

Write-Host "=== Configuración de Red para Cliente Móvil ===" -ForegroundColor Green
Write-Host ""

# 1. Obtener la IP local
Write-Host "1. Detectando tu IP local..." -ForegroundColor Yellow
$ip = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object {
    $_.InterfaceAlias -match "Wi-Fi|Ethernet" -and 
    $_.IPAddress -notmatch "^169\.|^127\."
} | Select-Object -First 1).IPAddress

if ($ip) {
    Write-Host "   ✓ IP detectada: $ip" -ForegroundColor Green
} else {
    Write-Host "   ✗ No se pudo detectar la IP automáticamente" -ForegroundColor Red
    $ip = Read-Host "   Por favor ingresa tu IP manualmente"
}

Write-Host ""

# 2. Actualizar appsettings.json
Write-Host "2. Actualizando appsettings.json..." -ForegroundColor Yellow
$appsettingsPath = Join-Path $PSScriptRoot "appsettings.json"

if (Test-Path $appsettingsPath) {
    $json = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
    $json.ServerConfiguration.ComercializadoraApiUrl = "http://${ip}:5001/api"
    $json.ServerConfiguration.BancoSoapServiceUrl = "http://${ip}:5000/BancoService.asmx"
    $json.DeviceType.UseLocalhost = $false
    
    $json | ConvertTo-Json -Depth 10 | Set-Content $appsettingsPath -Encoding UTF8
    Write-Host "   ✓ appsettings.json actualizado" -ForegroundColor Green
} else {
    Write-Host "   ✗ No se encontró appsettings.json" -ForegroundColor Red
}

Write-Host ""

# 3. Configurar Firewall
Write-Host "3. Configurando Firewall de Windows..." -ForegroundColor Yellow
Write-Host "   (Se requieren permisos de administrador)" -ForegroundColor Gray

try {
    # Verificar si las reglas ya existen
    $rule5000 = Get-NetFirewallRule -DisplayName "BancoSoapService" -ErrorAction SilentlyContinue
    $rule5001 = Get-NetFirewallRule -DisplayName "ComercializadoraAPI" -ErrorAction SilentlyContinue
    
    # Crear o actualizar regla para puerto 5000
    if ($rule5000) {
        Write-Host "   - Regla BancoSoapService ya existe" -ForegroundColor Gray
    } else {
        New-NetFirewallRule -DisplayName "BancoSoapService" `
                           -Direction Inbound `
                           -LocalPort 5000 `
                           -Protocol TCP `
                           -Action Allow `
                           -Profile Any | Out-Null
        Write-Host "   ✓ Regla creada para puerto 5000 (BancoSoapService)" -ForegroundColor Green
    }
    
    # Crear o actualizar regla para puerto 5001
    if ($rule5001) {
        Write-Host "   - Regla ComercializadoraAPI ya existe" -ForegroundColor Gray
    } else {
        New-NetFirewallRule -DisplayName "ComercializadoraAPI" `
                           -Direction Inbound `
                           -LocalPort 5001 `
                           -Protocol TCP `
                           -Action Allow `
                           -Profile Any | Out-Null
        Write-Host "   ✓ Regla creada para puerto 5001 (ComercializadoraAPI)" -ForegroundColor Green
    }
} catch {
    Write-Host "   ✗ Error al configurar firewall: $_" -ForegroundColor Red
    Write-Host "   Por favor ejecuta este script como Administrador" -ForegroundColor Yellow
}

Write-Host ""

# 4. Resumen
Write-Host "=== Configuración Completa ===" -ForegroundColor Green
Write-Host ""
Write-Host "Tu configuración:" -ForegroundColor Cyan
Write-Host "  • IP Local: $ip" -ForegroundColor White
Write-Host "  • API URL: http://${ip}:5001/api" -ForegroundColor White
Write-Host "  • SOAP URL: http://${ip}:5000/BancoService.asmx" -ForegroundColor White
Write-Host ""
Write-Host "Próximos pasos:" -ForegroundColor Yellow
Write-Host "  1. Asegúrate de que tu PC y tu dispositivo móvil estén en la misma WiFi" -ForegroundColor White
Write-Host "  2. Inicia los servicios backend:" -ForegroundColor White
Write-Host "     cd ..\soap_dotnet_pruebaproyecto\BancoSoapService" -ForegroundColor Gray
Write-Host "     dotnet run" -ForegroundColor Gray
Write-Host "     (En otra terminal)" -ForegroundColor Gray
Write-Host "     cd ..\ComercializadoraAPI" -ForegroundColor Gray
Write-Host "     dotnet run" -ForegroundColor Gray
Write-Host "  3. Compila y ejecuta el cliente móvil" -ForegroundColor White
Write-Host "  4. Prueba desde el navegador de tu móvil:" -ForegroundColor White
Write-Host "     http://${ip}:5001/api/productos" -ForegroundColor Gray
Write-Host ""
Write-Host "¡Listo para usar! 🚀" -ForegroundColor Green
Write-Host ""

# Preguntar si quiere probar la conectividad
$testConnection = Read-Host "¿Quieres probar la conectividad ahora? (S/N)"
if ($testConnection -eq "S" -or $testConnection -eq "s") {
    Write-Host ""
    Write-Host "Probando conexión local..." -ForegroundColor Yellow
    
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5001/api/productos" -TimeoutSec 5 -UseBasicParsing
        Write-Host "✓ API accesible desde localhost" -ForegroundColor Green
    } catch {
        Write-Host "✗ API no responde en localhost" -ForegroundColor Red
        Write-Host "  Asegúrate de que ComercializadoraAPI esté corriendo" -ForegroundColor Yellow
    }
    
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/BancoService.asmx" -TimeoutSec 5 -UseBasicParsing
        Write-Host "✓ SOAP Service accesible desde localhost" -ForegroundColor Green
    } catch {
        Write-Host "✗ SOAP Service no responde en localhost" -ForegroundColor Red
        Write-Host "  Asegúrate de que BancoSoapService esté corriendo" -ForegroundColor Yellow
    }
}

Write-Host ""
Read-Host "Presiona Enter para salir"
