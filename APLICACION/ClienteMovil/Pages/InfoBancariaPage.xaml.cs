using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class InfoBancariaPage : ContentPage
{
    private readonly string _cedula;
    
    public InfoBancariaPage(string cedula)
    {
        InitializeComponent();
        _cedula = cedula;
        LoadInfo();
    }

    private async void LoadInfo()
    {
        try
        {
            Loading.IsRunning = true;
            
            var request = new GetClientInfoRequest { Cedula = _cedula };
            var response = await BancoSoapService.GetClientInfoAsync(request);
            
            if (response.Cliente != null)
            {
                LblNombre.Text = response.Cliente.NombreCompleto;
                LblCedula.Text = $"Cédula: {response.Cliente.Cedula}";
                LblFechaNacimiento.Text = $"Fecha de Nacimiento: {response.Cliente.FechaNacimiento:dd/MM/yyyy}";
                LblEstadoCivil.Text = $"Estado Civil: {response.Cliente.EstadoCivil}";
                
                CuentasCollection.ItemsSource = response.Cliente.Cuentas;
                
                FrameInfo.IsVisible = true;
            }
            else
            {
                LblError.Text = "No se encontró información bancaria";
                LblError.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            LblError.Text = $"Error: {ex.Message}";
            LblError.IsVisible = true;
        }
        finally
        {
            Loading.IsRunning = false;
        }
    }
}
