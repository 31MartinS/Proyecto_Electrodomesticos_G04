using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class CompraEfectivoPage : ContentPage
{
    private readonly string _cedula;
    private readonly ProductoDto _producto;
    
    public decimal Subtotal => _producto.PrecioVenta;
    public decimal Descuento => Subtotal * 0.33m;
    public decimal Total => Subtotal - Descuento;
    
    public ProductoDto Producto => _producto;
    
    public CompraEfectivoPage(string cedula, ProductoDto producto)
    {
        InitializeComponent();
        _cedula = cedula;
        _producto = producto;
        BindingContext = this;
    }

    private async void OnConfirmarClicked(object sender, EventArgs e)
    {
        try
        {
            var factura = new CreateFacturaDto
            {
                Cedula = _cedula,
                FormaPago = "EFECTIVO",
                PlazoMeses = 0,
                Detalles = new List<DetalleFacturaDto>
                {
                    new DetalleFacturaDto { IdProducto = _producto.IdProducto, Cantidad = 1 }
                }
            };

            var response = await ApiService.CreateFacturaAsync(factura);
            
            await DisplayAlert(
                "¡Compra Exitosa!",
                $"Factura: {response.NumeroFactura}\n" +
                $"Total: ${response.Total:N2}\n" +
                $"Descuento aplicado: 33%",
                "OK");
            
            // Navegar a pantalla de éxito
            await Navigation.PushAsync(new CompraEfectivoExitoPage(response));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al procesar compra: {ex.Message}", "OK");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
