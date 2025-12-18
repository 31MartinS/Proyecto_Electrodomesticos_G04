using ClienteMovil.Models;
using ClienteMovil.Services;
using System.Collections.ObjectModel;

namespace ClienteMovil.Pages;

public partial class CarritoPage : ContentPage
{
    private readonly string _cedula;
    private ObservableCollection<CarritoItem> _carritoItems;

    public CarritoPage(string cedula, ObservableCollection<CarritoItem> carritoItems)
    {
        InitializeComponent();
        _cedula = cedula;
        _carritoItems = carritoItems;
        
        CarritoCollection.ItemsSource = _carritoItems;
        ActualizarTotal();
        
        // Suscribirse a cambios en la colección
        _carritoItems.CollectionChanged += (s, e) => ActualizarTotal();
    }

    private void ActualizarTotal()
    {
        var total = _carritoItems.Sum(i => i.Subtotal);
        TotalLabel.Text = total.ToString("C2");
    }

    private void OnDisminuirClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.CommandParameter as CarritoItem;
        
        if (item != null && item.Cantidad > 1)
        {
            item.Cantidad--;
            // Forzar actualización
            var index = _carritoItems.IndexOf(item);
            _carritoItems[index] = item;
            ActualizarTotal();
        }
    }

    private void OnAumentarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.CommandParameter as CarritoItem;
        
        if (item != null)
        {
            item.Cantidad++;
            // Forzar actualización
            var index = _carritoItems.IndexOf(item);
            _carritoItems[index] = item;
            ActualizarTotal();
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var item = button?.CommandParameter as CarritoItem;
        
        if (item != null)
        {
            bool confirmar = await DisplayAlert(
                "Eliminar producto",
                $"¿Desea eliminar {item.Nombre} del carrito?",
                "Sí",
                "No");
            
            if (confirmar)
            {
                _carritoItems.Remove(item);
            }
        }
    }

    private async void OnPagarEfectivoClicked(object sender, EventArgs e)
    {
        if (_carritoItems.Count == 0)
        {
            await DisplayAlert("Carrito vacío", "Agregue productos al carrito antes de pagar.", "OK");
            return;
        }

        try
        {
            var factura = new CreateFacturaDto
            {
                Cedula = _cedula,
                FormaPago = "EFECTIVO",
                PlazoMeses = 0,
                Detalles = _carritoItems.Select(item => new DetalleFacturaDto
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                }).ToList()
            };

            var response = await ApiService.CreateFacturaAsync(factura);
            
            // Limpiar carrito
            _carritoItems.Clear();
            
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

    private async void OnPagarCreditoClicked(object sender, EventArgs e)
    {
        if (_carritoItems.Count == 0)
        {
            await DisplayAlert("Carrito vacío", "Agregue productos al carrito antes de pagar.", "OK");
            return;
        }

        var total = _carritoItems.Sum(i => i.Subtotal);
        
        // Solicitar plazo
        string plazoStr = await DisplayPromptAsync(
            "Pago a Crédito",
            $"Total: ${total:N2}\n\n¿En cuántos meses desea pagar? (3-24)",
            "Continuar",
            "Cancelar",
            "12",
            keyboard: Keyboard.Numeric);
        
        if (string.IsNullOrEmpty(plazoStr))
            return;
        
        if (!int.TryParse(plazoStr, out int plazoMeses) || plazoMeses < 3 || plazoMeses > 24)
        {
            await DisplayAlert("Error", "El plazo debe estar entre 3 y 24 meses", "OK");
            return;
        }

        try
        {
            // Evaluar crédito
            var request = new EvaluateCreditRequest
            {
                Cedula = _cedula,
                PrecioElectrodomestico = total,
                PlazoMeses = plazoMeses
            };
            var evaluacion = await BancoSoapService.EvaluateCreditAsync(request);
            
            if (!evaluacion.Aprobado)
            {
                await DisplayAlert(
                    "Crédito Rechazado",
                    evaluacion.Mensaje,
                    "OK");
                return;
            }

            var factura = new CreateFacturaDto
            {
                Cedula = _cedula,
                FormaPago = "CREDITO",
                PlazoMeses = plazoMeses,
                IdCreditoBanco = evaluacion.IdCredito,
                Detalles = _carritoItems.Select(item => new DetalleFacturaDto
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad
                }).ToList()
            };

            var response = await ApiService.CreateFacturaAsync(factura);
            
            // Limpiar carrito
            _carritoItems.Clear();
            
            // Obtener tabla de amortización
            var amortizacion = await BancoSoapService.GetAmortizationScheduleAsync(response.IdCreditoBanco ?? 0);
            
            // Navegar a pantalla de éxito
            await Navigation.PushAsync(new CreditoExitoPage(response, amortizacion));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al procesar crédito: {ex.Message}", "OK");
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
