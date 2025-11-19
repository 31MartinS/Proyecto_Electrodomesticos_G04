using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class LoginPage : ContentPage
{
    private const string USUARIO_VALIDO = "MONSTER";
    private const string PASSWORD_VALIDA = "monster9";
    
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnIngresarClicked(object sender, EventArgs e)
    {
        ErrorPanel.IsVisible = false;
        
        string usuario = EntUsuario.Text?.Trim() ?? "";
        string password = EntPassword.Text ?? "";

        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Validación", "Por favor ingrese usuario y contraseña", "OK");
            return;
        }

        if (usuario == USUARIO_VALIDO && password == PASSWORD_VALIDA)
        {
            // Login exitoso
            ErrorPanel.IsVisible = false;
            await Navigation.PushAsync(new MenuPage());
        }
        else
        {
            // Mostrar error
            ErrorPanel.IsVisible = true;
            EntPassword.Text = "";
        }
    }
}
