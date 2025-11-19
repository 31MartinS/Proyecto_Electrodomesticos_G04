using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class VentaCedulaPage : ContentPage
{
    public VentaCedulaPage()
    {
        InitializeComponent();
    }

    private async void OnContinuarClicked(object sender, EventArgs e)
    {
        string cedula = EntCedula.Text?.Trim();

        LblMensaje.IsVisible = false;

        if (string.IsNullOrWhiteSpace(cedula))
        {
            LblMensaje.Text = "Por favor ingrese una cédula";
            LblMensaje.IsVisible = true;
            return;
        }

        if (cedula.Length != 10)
        {
            LblMensaje.Text = "La cédula debe tener 10 dígitos";
            LblMensaje.IsVisible = true;
            return;
        }

        try
        {
            BtnContinuar.IsEnabled = false;
            Loading.IsRunning = true;
            
            System.Diagnostics.Debug.WriteLine($"[VentaCedulaPage] Verificando cliente: {cedula}");
            
            // Verificar si el cliente existe en la API
            var cliente = await ApiService.GetClienteByCedulaAsync(cedula);
            
            System.Diagnostics.Debug.WriteLine($"[VentaCedulaPage] Cliente encontrado: {cliente != null}");
            
            if (cliente == null)
            {
                System.Diagnostics.Debug.WriteLine($"[VentaCedulaPage] Cliente NULL, ofreciendo registro");
                
                // Cliente no existe, ofrecer registro
                bool registrar = await DisplayAlert(
                    "Cliente no encontrado",
                    $"No existe un cliente con la cédula {cedula} en el sistema.\\n\\n¿Desea registrarlo?",
                    "Sí", "No");
                
                if (registrar)
                {
                    await Navigation.PushAsync(new RegistroClientePage(cedula));
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[VentaCedulaPage] Cliente encontrado: {cliente.Nombres} {cliente.Apellidos}");
                
                // Guardar cédula en el estado global
                AppState.CedulaActual = cedula;
                
                // Cliente existe, continuar a productos
                await Navigation.PushAsync(new ProductosPage(cedula));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[VentaCedulaPage] Exception: {ex.Message}");
            
            await DisplayAlert("Error de Conexión", 
                $"No se pudo conectar con el servidor.\\n\\nVerifique:\\n• El servidor está ejecutándose\\n• Está en la misma red WiFi\\n• IP del servidor: 10.40.20.89\\n\\nError técnico: {ex.Message}", 
                "OK");
        }
        finally
        {
            BtnContinuar.IsEnabled = true;
            Loading.IsRunning = false;
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
