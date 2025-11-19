using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class InventarioPage : ContentPage
{
    public InventarioPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarProductos();
    }

    private async Task CargarProductos()
    {
        try
        {
            var productos = await ApiService.GetProductosAsync();
            ProductosCollection.ItemsSource = productos;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los productos: {ex.Message}", "OK");
        }
    }

    private async void OnNuevoProductoClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProductoFormPage());
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ProductoDto producto)
        {
            await Navigation.PushAsync(new ProductoFormPage(producto));
        }
    }

    private async void OnEliminarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is ProductoDto producto)
        {
            bool confirmar = await DisplayAlert(
                "Confirmar Eliminación",
                $"¿Está seguro de eliminar el producto '{producto.Nombre}'?",
                "Sí", "No");

            if (confirmar)
            {
                try
                {
                    await ApiService.DeleteProductoAsync(producto.IdProducto);
                    await DisplayAlert("Éxito", "Producto eliminado exitosamente", "OK");
                    await CargarProductos();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo eliminar el producto: {ex.Message}", "OK");
                }
            }
        }
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
