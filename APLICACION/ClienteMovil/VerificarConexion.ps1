# Script de Prueba de Conectividad para Dispositivo Móvil
# Ejecutar este script para verificar que todo está configurado correctamente

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "PRUEBA DE CONECTIVIDAD MÓVIL" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Obtener IP Local
Write-Host "1. Obteniendo tu IP local..." -ForegroundColor Yellow
$localIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -like "*Wi-Fi*" -and $_.IPAddress -notlike "169.254.*" }).IPAddress
Write-Host "   Tu IP: $localIP" -ForegroundColor Green
Write-Host ""

# Verificar si los puertos están escuchando
Write-Host "2. Verificando si los servicios están corriendo..." -ForegroundColor Yellow

$port5001 = Get-NetTCPConnection -LocalPort 5001 -State Listen -ErrorAction SilentlyContinue
$port5000 = Get-NetTCPConnection -LocalPort 5000 -State Listen -ErrorAction SilentlyContinue

if ($port5001) {
    Write-Host "   ✓ Puerto 5001 (API) está escuchando" -ForegroundColor Green
} else {
    Write-Host "   ✗ Puerto 5001 (API) NO está escuchando - Inicia ComercializadoraAPI" -ForegroundColor Red
}

if ($port5000) {
    Write-Host "   ✓ Puerto 5000 (SOAP) está escuchando" -ForegroundColor Green
} else {
    Write-Host "   ✗ Puerto 5000 (SOAP) NO está escuchando - Inicia BancoSoapService" -ForegroundColor Red
}
Write-Host ""

# Verificar reglas de firewall
Write-Host "3. Verificando reglas de firewall..." -ForegroundColor Yellow
$fw5001 = Get-NetFirewallRule -DisplayName "*5001*" -ErrorAction SilentlyContinue
$fw5000 = Get-NetFirewallRule -DisplayName "*5000*" -ErrorAction SilentlyContinue

if ($fw5001) {
    Write-Host "   ✓ Regla de firewall para puerto 5001 existe" -ForegroundColor Green
} else {
    Write-Host "   ✗ Falta regla de firewall para puerto 5001" -ForegroundColor Red
}

if ($fw5000) {
    Write-Host "   ✓ Regla de firewall para puerto 5000 existe" -ForegroundColor Green
} else {
    Write-Host "   ✗ Falta regla de firewall para puerto 5000" -ForegroundColor Red
}
Write-Host ""

# Instrucciones para el dispositivo móvil
Write-Host "4. Instrucciones para tu dispositivo móvil:" -ForegroundColor Yellow
Write-Host "   Abre el navegador en tu móvil y prueba estas URLs:" -ForegroundColor White
Write-Host "   http://$localIP:5001/api/productos" -ForegroundColor Cyan
Write-Host "   http://$localIP:5000/BancoService.asmx" -ForegroundColor Cyan
Write-Host ""

Write-Host "5. Configuración en appsettings.json:" -ForegroundColor Yellow
Write-Host "   Asegúrate que en ClienteMovil/appsettings.json tengas:" -ForegroundColor White
Write-Host "   ""ComercializadoraApiUrl"": ""http://$localIP:5001/api""" -ForegroundColor Cyan
Write-Host "   ""BancoSoapServiceUrl"": ""http://$localIP:5000/BancoService.asmx""" -ForegroundColor Cyan
Write-Host "   ""UseLocalhost"": false" -ForegroundColor Cyan
Write-Host ""

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Presiona cualquier tecla para salir..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
