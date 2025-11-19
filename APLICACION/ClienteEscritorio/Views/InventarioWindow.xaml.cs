using System.Windows;
using System.Windows.Controls;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ClienteEscritorio.Views
{
    public partial class InventarioWindow : Window
    {
        public InventarioWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private async Task CargarProductos()
        {
            try
            {
                var productos = await ApiService.GetProductosAsync();
                DgProductos.ItemsSource = productos;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCrear_Click(object sender, RoutedEventArgs e)
        {
            var crearWindow = new ProductoFormWindow();
            if (crearWindow.ShowDialog() == true)
            {
                _ = CargarProductos();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var producto = button?.Tag as ProductoDto;
            
            if (producto != null)
            {
                var editarWindow = new ProductoFormWindow(producto);
                if (editarWindow.ShowDialog() == true)
                {
                    _ = CargarProductos();
                }
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var producto = button?.Tag as ProductoDto;
            
            if (producto != null)
            {
                var result = MessageBox.Show(
                    $"¿Está seguro de eliminar el producto '{producto.Nombre}'?",
                    "Confirmar eliminación",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await ApiService.DeleteProductoAsync(producto.IdProducto);
                        await CargarProductos();
                        MessageBox.Show("Producto eliminado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al eliminar producto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
