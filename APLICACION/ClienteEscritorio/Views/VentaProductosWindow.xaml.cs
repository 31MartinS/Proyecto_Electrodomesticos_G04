using System;
using System.Windows;
using System.Windows.Controls;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class VentaProductosWindow : Window
    {
        private readonly string _cedula;

        public VentaProductosWindow(string cedula)
        {
            InitializeComponent();
            _cedula = cedula;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
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

        private void BtnEfectivo_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var producto = button?.Tag as ProductoDto;

            if (producto != null)
            {
                var ventaEfectivoWindow = new VentaEfectivoWindow(_cedula, producto);
                ventaEfectivoWindow.ShowDialog();
            }
        }

        private void BtnCredito_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var producto = button?.Tag as ProductoDto;

            if (producto != null)
            {
                var ventaCreditoWindow = new VentaCreditoWindow(_cedula, producto);
                ventaCreditoWindow.ShowDialog();
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
