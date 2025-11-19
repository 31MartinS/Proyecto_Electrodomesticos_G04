# Cliente Móvil - Comercializadora

Aplicación móvil desarrollada con .NET MAUI para Android que permite a los clientes:
- Iniciar sesión con cédula
- Registrarse como nuevos clientes
- Ver catálogo de productos con imágenes
- Realizar compras en efectivo
- Solicitar créditos bancarios
- Ver información bancaria y amortizaciones

## Requisitos

### Software
- Visual Studio 2022 con carga de trabajo MAUI
- .NET 8.0 SDK
- Android SDK (API 21 o superior)
- Platform Tools de Android (adb)

### Hardware
- Dispositivo Android físico (recomendado) o emulador
- Dispositivo de prueba: Xiaomi 23129RA5FL (Android 15.0 - API 35)
- Conexión WiFi a la misma red que el servidor (10.40.20.89)

## Configuración del Dispositivo

### 1. Habilitar Modo Desarrollador
1. Ve a **Configuración** > **Acerca del teléfono**
2. Toca 7 veces en **Número de compilación** o **Versión MIUI**
3. Verás el mensaje "Ahora eres un desarrollador"

### 2. Activar Depuración USB
1. Ve a **Configuración** > **Ajustes adicionales** > **Opciones de desarrollador**
2. Activa **Depuración USB**
3. Activa **Instalar mediante USB** (si disponible)

### 3. Conectar Dispositivo
1. Conecta el dispositivo al PC mediante cable USB
2. En el dispositivo, autoriza la conexión USB (marca "Permitir siempre")
3. Verifica la conexión ejecutando:
   ```powershell
   adb devices
   ```
   Deberías ver tu dispositivo listado con estado "device"

### 4. Configurar Red
El dispositivo debe estar en la **misma red WiFi** que el servidor (IP: 10.40.20.89)

Para verificar conectividad desde el PC:
```powershell
adb shell ping -c 4 10.40.20.89
```

## Compilación y Despliegue

### Opción 1: Script Automático
Ejecuta el script de despliegue:
```powershell
.\desplegar-movil.ps1
```

### Opción 2: Manual desde Visual Studio
1. Abre el proyecto `ClienteMovil` en Visual Studio 2022
2. Selecciona el dispositivo Android en la barra de herramientas
3. Presiona F5 o clic en "Ejecutar"

### Opción 3: Línea de Comandos
```powershell
cd ClienteMovil
dotnet build -t:Run -f net8.0-android
```

## Estructura del Proyecto

```
ClienteMovil/
├── Models/
│   └── DTOs.cs                      # Modelos de datos
├── Pages/
│   ├── LoginPage.xaml               # Inicio de sesión
│   ├── RegistroClientePage.xaml     # Registro de clientes
│   ├── MenuPage.xaml                # Menú principal
│   ├── ProductosPage.xaml           # Lista de productos
│   ├── CompraEfectivoPage.xaml      # Compra en efectivo
│   ├── CompraEfectivoExitoPage.xaml # Confirmación efectivo
│   ├── CompraCreditoPage.xaml       # Solicitud de crédito
│   ├── CreditoExitoPage.xaml        # Tabla de amortización
│   └── InfoBancariaPage.xaml        # Info bancaria del cliente
├── Services/
│   ├── ApiService.cs                # Cliente REST API
│   ├── BancoContracts.cs            # Contratos SOAP
│   └── BancoSoapService.cs          # Cliente SOAP
├── Platforms/
│   └── Android/
│       └── AndroidManifest.xml      # Permisos y configuración
├── App.xaml                         # Configuración global
├── MauiProgram.cs                   # Punto de entrada
└── ClienteMovil.csproj              # Configuración del proyecto
```

## Servicios Conectados

### REST API (Comercializadora)
- **URL Base**: http://10.40.20.89:5001/api
- **Endpoints**:
  - `POST /Clientes` - Registro de clientes
  - `GET /Clientes/{cedula}` - Obtener cliente
  - `GET /Productos` - Listar productos
  - `POST /Facturas` - Crear factura

### SOAP (Banco)
- **URL**: http://10.40.20.89:5000/BancoService.asmx
- **Operaciones**:
  - `GetClientInfo` - Información bancaria
  - `EvaluateCredit` - Evaluar solicitud de crédito
  - `GetAmortizationSchedule` - Tabla de amortización

## Funcionalidades Implementadas

### ✅ Autenticación
- Login con cédula
- Validación de cliente existente
- Navegación a registro si no existe

### ✅ Gestión de Clientes
- Registro de nuevos clientes
- Validación de formularios
- Integración con API REST

### ✅ Catálogo de Productos
- Lista con imágenes
- Carga desde servidor
- Visualización de precio y stock

### ✅ Compra en Efectivo
- Selección de cantidad
- Cálculo de total
- Creación de factura
- Página de confirmación

### ✅ Compra a Crédito
- Evaluación de crédito vía SOAP
- Selección de cuotas (3, 6, 12 meses)
- Validación de aprobación
- Reutilización de crédito aprobado
- Tabla de amortización detallada

### ✅ Información Bancaria
- Consulta de datos del cliente
- Listado de cuentas bancarias
- Visualización de saldos

## Solución de Problemas

### No se detecta el dispositivo
- Verifica que el cable USB funcione para transferencia de datos (no solo carga)
- Reinstala los drivers USB del dispositivo
- Ejecuta `adb kill-server` y luego `adb start-server`

### Error de compilación "SDK not found"
- Abre Visual Studio Installer
- Modifica la instalación y asegúrate de tener instalado:
  - ".NET Multi-platform App UI development"
  - "Android SDK"

### La app no se conecta al servidor
- Verifica que el servidor esté ejecutándose: `.\iniciar-servidores.ps1`
- Confirma que el dispositivo está en la misma red WiFi
- Prueba hacer ping desde el dispositivo a 10.40.20.89
- Verifica el firewall de Windows: `.\configurar-firewall.ps1`

### Error "Unauthorized" en SOAP
- Verifica que los contratos SOAP tengan el atributo `[DataMember(Order = X)]`
- Confirma que el servicio SOAP esté activo en http://10.40.20.89:5000/BancoService.asmx

## Desarrollo

### Agregar una nueva página
1. Crea archivos `.xaml` y `.xaml.cs` en la carpeta `Pages/`
2. Define la interfaz en XAML con controles MAUI
3. Implementa la lógica en el code-behind
4. Navega usando: `await Navigation.PushAsync(new NuevaPagina())`

### Modificar servicios
- **ApiService.cs**: Agrega métodos para nuevos endpoints REST
- **BancoContracts.cs**: Define nuevos contratos SOAP con `[DataMember(Order)]`
- **BancoSoapService.cs**: Implementa llamadas a nuevas operaciones SOAP

### Testing
Para probar sin dispositivo físico:
1. Crea un emulador Android en Visual Studio
2. Asegúrate de que tenga acceso a la red del host
3. Usa la IP 10.0.2.2 para acceder al localhost del PC desde el emulador

## Notas Técnicas

- **Framework**: .NET MAUI 8.0
- **Target**: Android 21+ (API Level 21)
- **Probado en**: Xiaomi 23129RA5FL, Android 15.0 (API 35)
- **Arquitectura**: Cliente-servidor con REST API y SOAP
- **Serialización**: Newtonsoft.Json para REST, DataContractSerializer para SOAP
- **Navegación**: NavigationPage con stack de páginas

## Próximos Pasos

- [ ] Implementar caché local de productos
- [ ] Agregar manejo de sesión persistente
- [ ] Implementar notificaciones push
- [ ] Agregar historial de compras
- [ ] Soporte para múltiples idiomas
- [ ] Modo offline con sincronización
