using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class FacturaDetallePage : ContentPage
{
    private readonly FacturaListDto _factura;

    public FacturaDetallePage(FacturaListDto factura)
    {
        InitializeComponent();
        _factura = factura;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        CargarDatosFactura();
        if (_factura.IdCreditoBanco.HasValue)
        {
            await CargarAmortizacion();
        }
    }

    private void CargarDatosFactura()
    {
        LblNumero.Text = _factura.NumeroFactura;
        LblFecha.Text = _factura.Fecha.ToString("dd/MM/yyyy HH:mm");
        LblCliente.Text = $"{_factura.Cliente.Nombres} {_factura.Cliente.Apellidos} ({_factura.Cliente.Cedula})";
        LblFormaPago.Text = _factura.FormaPago;

        // Cargar productos
        StackProductos.Children.Clear();
        foreach (var detalle in _factura.Detalles)
        {
            var productoFrame = CrearProductoFrame(detalle);
            StackProductos.Children.Add(productoFrame);
        }

        // Mostrar totales
        LblSubtotal.Text = $"${_factura.Subtotal:N2}";
        
        if (_factura.Descuento > 0)
        {
            LblDescuentoLabel.IsVisible = true;
            LblDescuento.IsVisible = true;
            LblDescuento.Text = $"-${_factura.Descuento:N2}";
        }
        else
        {
            LblDescuentoLabel.IsVisible = false;
            LblDescuento.IsVisible = false;
        }

        LblTotal.Text = $"${_factura.Total:N2}";
    }

    private Frame CrearProductoFrame(DetalleFacturaListDto detalle)
    {
        var frame = new Frame
        {
            BorderColor = Color.FromArgb("#E0E0E0"),
            CornerRadius = 8,
            Padding = 10,
            BackgroundColor = Color.FromArgb("#F9F9F9"),
            HasShadow = false
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(60) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            ColumnSpacing = 10,
            RowSpacing = 5
        };

        // Imagen
        var imagen = new Image
        {
            Source = detalle.Producto.ImageUrl,
            Aspect = Aspect.AspectFit,
            WidthRequest = 50,
            HeightRequest = 50,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetRowSpan(imagen, 2);
        grid.Add(imagen, 0, 0);

        // Nombre del producto
        var lblNombre = new Label
        {
            Text = detalle.Producto.Nombre,
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        };
        grid.Add(lblNombre, 1, 0);

        // Cantidad y precio unitario
        var lblDetalle = new Label
        {
            Text = $"Cantidad: {detalle.Cantidad} × ${detalle.PrecioUnitario:N2}",
            FontSize = 13,
            TextColor = Color.FromArgb("#666666")
        };
        grid.Add(lblDetalle, 1, 1);

        // Total línea
        var lblTotal = new Label
        {
            Text = $"${detalle.TotalLinea:N2}",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#4A90E2"),
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetRowSpan(lblTotal, 2);
        grid.Add(lblTotal, 2, 0);

        frame.Content = grid;
        return frame;
    }

    private async Task CargarAmortizacion()
    {
        try
        {
            var response = await ApiService.GetAmortizacionAsync(_factura.IdCreditoBanco!.Value);
            
            if (response.Cuotas != null && response.Cuotas.Count > 0)
            {
                FrameAmortizacion.IsVisible = true;
                StackAmortizacion.Children.Clear();

                // Encabezado de la tabla
                var headerGrid = new Grid
                {
                    BackgroundColor = Color.FromArgb("#FFD700"),
                    Padding = new Thickness(10, 8),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(60) },  // Cuota
                        new ColumnDefinition { Width = new GridLength(100) }, // Fecha
                        new ColumnDefinition { Width = new GridLength(80) },  // Valor
                        new ColumnDefinition { Width = new GridLength(80) },  // Interés
                        new ColumnDefinition { Width = new GridLength(80) },  // Capital
                        new ColumnDefinition { Width = new GridLength(80) }   // Saldo
                    }
                };

                headerGrid.Add(CrearLabelTabla("Cuota", true), 0, 0);
                headerGrid.Add(CrearLabelTabla("Fecha", true), 1, 0);
                headerGrid.Add(CrearLabelTabla("Valor", true), 2, 0);
                headerGrid.Add(CrearLabelTabla("Interés", true), 3, 0);
                headerGrid.Add(CrearLabelTabla("Capital", true), 4, 0);
                headerGrid.Add(CrearLabelTabla("Saldo", true), 5, 0);

                StackAmortizacion.Children.Add(headerGrid);

                // Filas de datos
                foreach (var cuota in response.Cuotas)
                {
                    var rowGrid = new Grid
                    {
                        BackgroundColor = Colors.White,
                        Padding = new Thickness(10, 8),
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = new GridLength(60) },
                            new ColumnDefinition { Width = new GridLength(100) },
                            new ColumnDefinition { Width = new GridLength(80) },
                            new ColumnDefinition { Width = new GridLength(80) },
                            new ColumnDefinition { Width = new GridLength(80) },
                            new ColumnDefinition { Width = new GridLength(80) }
                        }
                    };

                    rowGrid.Add(CrearLabelTabla(cuota.NumeroCuota.ToString(), false), 0, 0);
                    rowGrid.Add(CrearLabelTabla(cuota.FechaVencimiento.ToString("dd/MM/yy"), false), 1, 0);
                    rowGrid.Add(CrearLabelTabla($"${cuota.ValorCuota:N2}", false), 2, 0);
                    rowGrid.Add(CrearLabelTabla($"${cuota.InteresPagado:N2}", false), 3, 0);
                    rowGrid.Add(CrearLabelTabla($"${cuota.CapitalPagado:N2}", false), 4, 0);
                    rowGrid.Add(CrearLabelTabla($"${cuota.SaldoRestante:N2}", false), 5, 0);

                    // Línea separadora
                    var separator = new BoxView
                    {
                        HeightRequest = 1,
                        Color = Color.FromArgb("#E0E0E0")
                    };

                    StackAmortizacion.Children.Add(rowGrid);
                    StackAmortizacion.Children.Add(separator);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar la tabla de amortización: {ex.Message}", "OK");
        }
    }

    private Label CrearLabelTabla(string texto, bool esHeader)
    {
        return new Label
        {
            Text = texto,
            FontSize = esHeader ? 13 : 12,
            FontAttributes = esHeader ? FontAttributes.Bold : FontAttributes.None,
            TextColor = Colors.Black,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
