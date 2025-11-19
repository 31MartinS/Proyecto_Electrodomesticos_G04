using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class RegistroClientePage : ContentPage
{
    private readonly string _cedula;
    
    public RegistroClientePage(string cedula)
    {
        InitializeComponent();
        _cedula = cedula;
        EntCedula.Text = cedula;
    }

    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntNombres.Text) ||
            string.IsNullOrWhiteSpace(EntApellidos.Text))
        {
            await DisplayAlert("Validación", "Nombres y apellidos son requeridos", "OK");
            return;
        }

        try
        {
            var cliente = new ClienteComercializadora
            {
                Cedula = _cedula,
                Nombres = EntNombres.Text.Trim(),
                Apellidos = EntApellidos.Text.Trim(),
                Direccion = EntDireccion.Text?.Trim(),
                Telefono = EntTelefono.Text?.Trim(),
                Email = EntEmail.Text?.Trim()
            };

            await ApiService.CreateClienteAsync(cliente);
            
            await DisplayAlert("Éxito", "Cliente registrado correctamente. Ahora puede realizar la venta.", "OK");
            
            // Regresar a la pantalla de cédula que ahora podrá encontrar el cliente
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al registrar: {ex.Message}", "OK");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
