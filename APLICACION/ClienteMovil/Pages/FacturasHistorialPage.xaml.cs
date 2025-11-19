using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class FacturasHistorialPage : ContentPage
{
    private readonly string _cedula;

    public FacturasHistorialPage(string cedula)
    {
        InitializeComponent();
        _cedula = cedula;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarFacturas();
    }

    private async Task CargarFacturas()
    {
        try
        {
            var facturas = await ApiService.GetFacturasByCedulaAsync(_cedula);
            
            StackFacturas.Children.Clear();

            if (facturas == null || facturas.Count == 0)
            {
                StackSinFacturas.IsVisible = true;
                return;
            }

            StackSinFacturas.IsVisible = false;

            foreach (var factura in facturas)
            {
                var facturaFrame = CrearFacturaFrame(factura);
                StackFacturas.Children.Add(facturaFrame);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar las facturas: {ex.Message}", "OK");
        }
    }

    private Frame CrearFacturaFrame(FacturaListDto factura)
    {
        var frame = new Frame
        {
            BorderColor = Color.FromArgb("#D9D9D9"),
            CornerRadius = 10,
            HasShadow = true,
            Padding = 15,
            BackgroundColor = Colors.White
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            },
            RowSpacing = 5
        };

        // Número de factura
        var lblNumero = new Label
        {
            Text = factura.NumeroFactura,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black
        };
        grid.Add(lblNumero, 0, 0);

        // Fecha
        var lblFecha = new Label
        {
            Text = factura.Fecha.ToString("dd/MM/yyyy HH:mm"),
            FontSize = 14,
            TextColor = Color.FromArgb("#666666")
        };
        grid.Add(lblFecha, 0, 1);

        // Forma de pago
        var formaPagoColor = factura.FormaPago.ToUpper() == "EFECTIVO" ? "#96FF96" : "#FFD700";
        var lblFormaPago = new Label
        {
            Text = factura.FormaPago,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Black,
            BackgroundColor = Color.FromArgb(formaPagoColor),
            Padding = new Thickness(8, 3),
            HorizontalOptions = LayoutOptions.Start
        };
        grid.Add(lblFormaPago, 0, 2);

        // Total
        var lblTotal = new Label
        {
            Text = $"${factura.Total:N2}",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#4A90E2"),
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetRowSpan(lblTotal, 3);
        grid.Add(lblTotal, 1, 0);

        // Cantidad de productos
        var cantidadProductos = factura.Detalles.Sum(d => d.Cantidad);
        var lblProductos = new Label
        {
            Text = $"{cantidadProductos} producto(s)",
            FontSize = 13,
            TextColor = Color.FromArgb("#666666")
        };
        grid.Add(lblProductos, 0, 3);

        // Botón Ver Detalle
        var btnDetalle = new Button
        {
            Text = "VER DETALLE",
            BackgroundColor = Color.FromArgb("#4A90E2"),
            TextColor = Colors.White,
            FontSize = 13,
            CornerRadius = 15,
            Padding = new Thickness(15, 5),
            Margin = new Thickness(0, 5, 0, 0)
        };
        btnDetalle.Clicked += async (s, e) => await OnVerDetalleClicked(factura);
        Grid.SetColumnSpan(btnDetalle, 2);
        grid.Add(btnDetalle, 0, 4);

        frame.Content = grid;

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += async (s, e) => await OnVerDetalleClicked(factura);
        frame.GestureRecognizers.Add(tapGesture);

        return frame;
    }

    private async Task OnVerDetalleClicked(FacturaListDto factura)
    {
        await Navigation.PushAsync(new FacturaDetallePage(factura));
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
