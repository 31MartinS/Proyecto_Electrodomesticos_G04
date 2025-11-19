using System;
using System.Windows;
using System.Windows.Controls;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class FacturasWindow : Window
    {
        public FacturasWindow()
        {
            InitializeComponent();
        }

        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string cedula = TxtCedula.Text.Trim();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                MessageBox.Show("Por favor ingrese una cédula", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cedula.Length != 10)
            {
                MessageBox.Show("La cédula debe tener 10 dígitos", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var facturas = await ApiService.GetFacturasByCedulaAsync(cedula);
                
                if (facturas == null || facturas.Count == 0)
                {
                    MessageBox.Show("No se encontraron facturas para esta cédula", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    DgFacturas.ItemsSource = null;
                    return;
                }

                DgFacturas.ItemsSource = facturas;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar facturas: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVerDetalle_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var factura = button?.Tag as FacturaListDto;

            if (factura != null)
            {
                var detalleWindow = new FacturaDetalleWindow(factura);
                detalleWindow.ShowDialog();
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
