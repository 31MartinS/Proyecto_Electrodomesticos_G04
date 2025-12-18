# Configuración para Dispositivo Móvil Físico (USB)

## Tu IP Local Detectada: `10.40.24.189`

## 📱 Configuración Actual

El cliente móvil está configurado para usar:
- **API REST**: `http://10.40.24.189:5001/api`
- **Servicio SOAP**: `http://10.40.24.189:5000/BancoService.asmx`

## 🔧 Cómo Cambiar entre Emulador y Dispositivo Físico

Edita el archivo `appsettings.json` en la raíz del proyecto ClienteMovil:

### Para DISPOSITIVO FÍSICO (USB):
```json
"UseLocalhost": false
```

### Para EMULADOR:
```json
"UseLocalhost": true
```

## 🔥 Configurar el Firewall de Windows

Para que tu dispositivo móvil pueda conectarse a tu PC, debes permitir las conexiones en el firewall:

### Opción 1: PowerShell (Recomendado - Ejecutar como Administrador)
```powershell
# Permitir puerto 5001 (API)
New-NetFirewallRule -DisplayName "Comercializadora API" -Direction Inbound -LocalPort 5001 -Protocol TCP -Action Allow

# Permitir puerto 5000 (SOAP)
New-NetFirewallRule -DisplayName "Banco SOAP Service" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

### Opción 2: Firewall de Windows (Manual)
1. Abre "Firewall de Windows con seguridad avanzada"
2. Click en "Reglas de entrada" → "Nueva regla"
3. Tipo: Puerto → TCP → Puerto específico: 5001
4. Permitir la conexión
5. Aplicar a todos los perfiles
6. Nombre: "Comercializadora API"
7. Repetir para el puerto 5000 (Banco SOAP)

## ✅ Verificar Conexión

### 1. Desde tu dispositivo móvil, abre el navegador y prueba:
- `http://10.40.24.189:5001/api/productos`
- `http://10.40.24.189:5000/BancoService.asmx`

Si puedes ver contenido, la conexión funciona.

### 2. Verificar que los servicios están corriendo:
```powershell
# Verificar puerto 5001
Test-NetConnection -ComputerName 10.40.24.189 -Port 5001

# Verificar puerto 5000
Test-NetConnection -ComputerName 10.40.24.189 -Port 5000
```

## 📝 Si tu IP cambia

Si tu IP local cambia (por ejemplo, al reconectar al WiFi), actualiza en `appsettings.json`:

```json
"ServerConfiguration": {
  "ComercializadoraApiUrl": "http://TU_NUEVA_IP:5001/api",
  "BancoSoapServiceUrl": "http://TU_NUEVA_IP:5000/BancoService.asmx"
}
```

Para obtener tu IP actual:
```powershell
Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notlike "*Loopback*" }
```

## 🚀 Pasos para Ejecutar

1. ✅ Asegúrate que `UseLocalhost: false` en appsettings.json
2. ✅ Configura el firewall (comandos arriba)
3. ✅ Inicia ComercializadoraAPI (puerto 5001)
4. ✅ Inicia BancoSoapService (puerto 5000)
5. ✅ Conecta tu dispositivo por USB
6. ✅ Ejecuta la app desde Visual Studio

## 🐛 Problemas Comunes

### "No se puede conectar"
- Verifica que ambos servicios están corriendo
- Verifica el firewall
- Asegúrate que tu dispositivo y PC están en la misma red (WiFi)
- Prueba desde el navegador del móvil primero

### "Connection refused"
- Verifica que los puertos no estén bloqueados
- Revisa que no haya otro programa usando los puertos 5000 y 5001

### Android requiere conexión clara (no HTTPS)
Si ves errores de "Cleartext HTTP traffic not permitted", ya está configurado en el AndroidManifest.xml del proyecto.
