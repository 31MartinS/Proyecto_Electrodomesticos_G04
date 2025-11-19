using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class CompraCreditoPage : ContentPage
{
    private readonly string _cedula;
    private readonly ProductoDto _producto;
    private int _idCreditoBanco;
    
    public ProductoDto Producto => _producto;
    
    public CompraCreditoPage(string cedula, ProductoDto producto)
    {
        InitializeComponent();
        _cedula = cedula;
        _producto = producto;
        BindingContext = this;
        
        // Agregar opciones de cuotas
        PickerCuotas.Items.Add("6");
        PickerCuotas.Items.Add("12");
        PickerCuotas.Items.Add("18");
        PickerCuotas.Items.Add("24");
        
        EvaluarCredito();
    }

    private async void EvaluarCredito()
    {
        try
        {
            Loading.IsRunning = true;
            
            var request = new EvaluateCreditRequest
            {
                Cedula = _cedula,
                PrecioElectrodomestico = _producto.PrecioVenta,
                PlazoMeses = 12
            };

            var response = await BancoSoapService.EvaluateCreditAsync(request);
            
            if (response.Aprobado)
            {
                _idCreditoBanco = response.IdCredito;
                FrameEvaluacion.IsVisible = true;
                FrameInfoCredito.IsVisible = true;
                LblMontoMaximo.Text = $"${response.MontoMaximo:N2}";
                PanelCuotas.IsVisible = true;
            }
            else
            {
                // Mostrar pantalla de NO elegible
                await DisplayAlert(
                    "Usuario no es elegible para crédito",
                    response.Mensaje,
                    "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al evaluar crédito: {ex.Message}", "OK");
        }
        finally
        {
            Loading.IsRunning = false;
        }
    }

    private async void OnConfirmarCompraClicked(object sender, EventArgs e)
    {
        if (PickerCuotas.SelectedIndex == -1)
        {
            await DisplayAlert("Validación", "Seleccione el número de cuotas", "OK");
            return;
        }

        try
        {
            Loading.IsRunning = true;
            
            var cuotas = int.Parse(PickerCuotas.SelectedItem.ToString()!);
            
            var factura = new CreateFacturaDto
            {
                Cedula = _cedula,
                FormaPago = "CREDITO",
                PlazoMeses = cuotas,
                IdCreditoBanco = _idCreditoBanco,
                Detalles = new List<DetalleFacturaDto>
                {
                    new DetalleFacturaDto { IdProducto = _producto.IdProducto, Cantidad = 1 }
                }
            };

            var response = await ApiService.CreateFacturaAsync(factura);
            
            // Obtener tabla de amortización
            var amortizacion = await BancoSoapService.GetAmortizationScheduleAsync(_idCreditoBanco);
            
            await Navigation.PushAsync(new CreditoExitoPage(response, amortizacion));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al procesar compra: {ex.Message}", "OK");
        }
        finally
        {
            Loading.IsRunning = false;
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
