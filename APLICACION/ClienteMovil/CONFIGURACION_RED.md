# Configuración de Red para Cliente Móvil (Dispositivo Físico)

## Paso 1: Obtener tu IP local

### En Windows:
```powershell
ipconfig
```

Busca la sección de tu adaptador de red WiFi o Ethernet activo y anota la **IPv4 Address**:
```
Adaptador de LAN inalámbrica Wi-Fi:
   IPv4 Address. . . . . . . . . . . : 192.168.1.100  <-- Esta es tu IP
```

### En macOS/Linux:
```bash
ifconfig | grep "inet "
# o
ip addr show
```

## Paso 2: Actualizar appsettings.json

Abre `ClienteMovil/appsettings.json` y reemplaza `192.168.1.100` con tu IP:

```json
{
  "ServerConfiguration": {
    "ComercializadoraApiUrl": "http://TU_IP_AQUI:5001/api",
    "BancoSoapServiceUrl": "http://TU_IP_AQUI:5000/BancoService.asmx"
  },
  "DeviceType": {
    "UseLocalhost": false,
    ...
  }
}
```

## Paso 3: Configurar el Firewall de Windows

Necesitas permitir conexiones entrantes en los puertos 5000 y 5001:

### Opción A: Script PowerShell (Ejecutar como Administrador)
```powershell
# Permitir puerto 5000 (BancoSoapService)
New-NetFirewallRule -DisplayName "BancoSoapService" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow

# Permitir puerto 5001 (ComercializadoraAPI)
New-NetFirewallRule -DisplayName "ComercializadoraAPI" -Direction Inbound -LocalPort 5001 -Protocol TCP -Action Allow
```

### Opción B: Manualmente (GUI)
1. Abre "Firewall de Windows Defender con seguridad avanzada"
2. Clic en "Reglas de entrada" > "Nueva regla"
3. Tipo de regla: Puerto
4. TCP, puerto específico: 5000
5. Permitir la conexión
6. Aplicar a todos los perfiles
7. Nombre: "BancoSoapService"
8. Repetir para el puerto 5001 con nombre "ComercializadoraAPI"

## Paso 4: Configurar las APIs para escuchar en todas las interfaces

### BancoSoapService/Program.cs
Asegúrate de que escuche en `http://0.0.0.0:5000`:
```csharp
builder.WebHost.UseUrls("http://0.0.0.0:5000");
```

### ComercializadoraAPI/Program.cs o launchSettings.json
Asegúrate de que escuche en `http://0.0.0.0:5001`:
```csharp
builder.WebHost.UseUrls("http://0.0.0.0:5001");
```

O en `Properties/launchSettings.json`:
```json
"applicationUrl": "http://0.0.0.0:5001"
```

## Paso 5: Verificar Conectividad

### Desde tu PC:
```powershell
# Probar API
curl http://localhost:5001/api/productos

# Probar SOAP
curl http://localhost:5000/BancoService.asmx
```

### Desde tu dispositivo móvil:
Abre el navegador del móvil y visita:
- `http://TU_IP:5001/api/productos`
- `http://TU_IP:5000/BancoService.asmx`

Si ves respuesta JSON o XML, ¡la conexión funciona! ✅

## Paso 6: Asegúrate de que ambos dispositivos estén en la misma red WiFi

⚠️ **IMPORTANTE**: Tu PC y tu dispositivo móvil deben estar conectados a la misma red WiFi.

## Solución de Problemas

### Error: "No se puede conectar"
1. ✅ Verifica que ambos dispositivos estén en la misma WiFi
2. ✅ Verifica que la IP sea correcta (usar `ipconfig`)
3. ✅ Verifica que el firewall permita los puertos 5000 y 5001
4. ✅ Verifica que las APIs estén corriendo (`dotnet run`)
5. ✅ Verifica que las APIs escuchen en `0.0.0.0` y no solo en `localhost`

### Error: "Connection refused"
- Las APIs probablemente no estén escuchando en todas las interfaces (0.0.0.0)
- Verifica la configuración de URLs en Program.cs o launchSettings.json

### Error: "Timeout"
- Firewall bloqueando las conexiones
- Verifica las reglas del firewall

## Configuración Rápida para Desarrollo

Si solo quieres probar rápido, puedes deshabilitar temporalmente el firewall:
```powershell
# ⚠️ SOLO PARA DESARROLLO - No hacer en producción
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False

# Para volver a habilitarlo:
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled True
```
