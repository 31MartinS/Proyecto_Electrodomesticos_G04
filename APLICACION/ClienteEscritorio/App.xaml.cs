using System.Windows;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ClienteEscritorio
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Leer configuración desde appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Configurar el API base URL desde appsettings.json
            Services.ApiService.BaseUrl = configuration["ServerConfiguration:ComercializadoraApiUrl"] ?? "http://localhost:5001/api";
            Services.BancoSoapService.ServiceUrl = configuration["ServerConfiguration:BancoSoapServiceUrl"] ?? "http://localhost:5000/BancoService.asmx";

            // Navegar a la pantalla de login
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}
