using System.Windows;

namespace ClienteEscritorio
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configurar el API base URL
            // CAMBIAR ESTA IP A LA IP DEL SERVIDOR
            Services.ApiService.BaseUrl = "http://10.40.20.89:5001/api";
            Services.BancoSoapService.ServiceUrl = "http://10.40.20.89:5000/BancoService.asmx";

            // Navegar a la pantalla de login
            var loginWindow = new Views.LoginWindow();
            loginWindow.Show();
        }
    }
}
