namespace ClienteMovil.Pages;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    private async void OnEditarInventarioClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InventarioPage());
    }

    private async void OnRealizarVentaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VentaCedulaPage());
    }

    private async void OnVerFacturasClicked(object sender, EventArgs e)
    {
        // Solicitar la cédula para ver facturas
        string cedula = await DisplayPromptAsync(
            "Ver Facturas",
            "Ingrese la cédula del cliente:",
            placeholder: "0123456789",
            maxLength: 10,
            keyboard: Keyboard.Numeric);

        if (!string.IsNullOrWhiteSpace(cedula) && cedula.Length == 10)
        {
            await Navigation.PushAsync(new FacturasHistorialPage(cedula));
        }
        else if (!string.IsNullOrWhiteSpace(cedula))
        {
            await DisplayAlert("Error", "La cédula debe tener 10 dígitos", "OK");
        }
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Cerrar Sesión",
            "¿Está seguro que desea cerrar sesión?",
            "Sí", "No");
        
        if (confirmar)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
