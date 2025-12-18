using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using ClienteMovil.Services;

namespace ClienteMovil;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Cargar configuración desde appsettings.json embebido
		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream("ClienteMovil.appsettings.json");
		
		if (stream != null)
		{
			var config = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();

			// Configurar URLs según el tipo de dispositivo
			var useLocalhostStr = config["DeviceType:UseLocalhost"];
			bool useLocalhost = useLocalhostStr != null && bool.Parse(useLocalhostStr);
			
			if (useLocalhost)
			{
				// Emulador - usar localhost
				ApiService.BaseUrl = config["DeviceType:LocalhostApiUrl"] ?? "http://localhost:5001/api";
				BancoSoapService.ServiceUrl = config["DeviceType:LocalhostBancoUrl"] ?? "http://localhost:5000/BancoService.asmx";
			}
			else
			{
				// Dispositivo físico - usar IP de la PC
				ApiService.BaseUrl = config["ServerConfiguration:ComercializadoraApiUrl"] ?? "http://10.40.24.189:5001/api";
				BancoSoapService.ServiceUrl = config["ServerConfiguration:BancoSoapServiceUrl"] ?? "http://10.40.24.189:5000/BancoService.asmx";
			}
			
			System.Diagnostics.Debug.WriteLine($"[MauiProgram] API URL: {ApiService.BaseUrl}");
			System.Diagnostics.Debug.WriteLine($"[MauiProgram] Banco URL: {BancoSoapService.ServiceUrl}");
		}

		return builder.Build();
	}
}
