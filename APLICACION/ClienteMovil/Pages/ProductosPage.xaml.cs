using ClienteMovil.Models;
using ClienteMovil.Services;
using System.Collections.ObjectModel;

namespace ClienteMovil.Pages;

public partial class ProductosPage : ContentPage
{
    private readonly string _cedula;
    private static ObservableCollection<CarritoItem> _carrito = new ObservableCollection<CarritoItem>();
    
    public ProductosPage(string cedula)
    {
        InitializeComponent();
        _cedula = cedula;
        LoadProductos();
        ActualizarBadgeCarrito();
    }

    private async void LoadProductos()
    {
        try
        {
            var productos = await ApiService.GetProductosAsync();
            ProductosCollection.ItemsSource = productos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar productos: {ex.Message}", "OK");
        }
    }

    private void OnRefreshing(object sender, EventArgs e)
    {
        LoadProductos();
        if (sender is RefreshView refreshView)
            refreshView.IsRefreshing = false;
    }

    private async void OnEfectivoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.CommandParameter as ProductoDto;
        
        if (producto != null)
        {
            await Navigation.PushAsync(new CompraEfectivoPage(_cedula, producto));
        }
    }

    private async void OnCreditoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.CommandParameter as ProductoDto;
        
        if (producto != null)
        {
            await Navigation.PushAsync(new CompraCreditoPage(_cedula, producto));
        }
    }

    private async void OnAgregarCarritoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var producto = button?.CommandParameter as ProductoDto;
        
        if (producto != null)
        {
            var itemExistente = _carrito.FirstOrDefault(i => i.IdProducto == producto.IdProducto);
            
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                _carrito.Add(new CarritoItem
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Precio = producto.PrecioVenta,
                    Cantidad = 1,
                    ImageUrl = producto.ImageUrl
                });
            }
            
            ActualizarBadgeCarrito();
            await DisplayAlert("Agregado", $"{producto.Nombre} agregado al carrito", "OK");
        }
    }

    private async void OnVerCarritoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CarritoPage(_cedula, _carrito));
    }

    private void ActualizarBadgeCarrito()
    {
        var cantidadTotal = _carrito.Sum(i => i.Cantidad);
        BadgeLabel.Text = cantidadTotal > 0 ? cantidadTotal.ToString() : "";
        BadgeLabel.IsVisible = cantidadTotal > 0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ActualizarBadgeCarrito();
    }
}
