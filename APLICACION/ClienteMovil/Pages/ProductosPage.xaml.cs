using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class ProductosPage : ContentPage
{
    private readonly string _cedula;
    
    public ProductosPage(string cedula)
    {
        InitializeComponent();
        _cedula = cedula;
        LoadProductos();
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

    private async void OnRefreshing(object sender, EventArgs e)
    {
        LoadProductos();
        RefreshView.IsRefreshing = false;
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
}
