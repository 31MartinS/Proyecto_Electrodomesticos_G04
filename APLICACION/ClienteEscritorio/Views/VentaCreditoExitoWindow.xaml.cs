using System.Windows;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;
using System;

namespace ClienteEscritorio.Views
{
    public partial class VentaCreditoExitoWindow : Window
    {
        private readonly FacturaResponseDto _factura;

        public VentaCreditoExitoWindow(FacturaResponseDto factura)
        {
            InitializeComponent();
            _factura = factura;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Obtener tabla de amortización
            if (_factura.IdCreditoBanco.HasValue)
            {
                try
                {
                    var response = await BancoSoapService.GetAmortizationScheduleAsync(_factura.IdCreditoBanco.Value);
                    DgAmortizacion.ItemsSource = response.Cuotas;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al obtener tabla de amortización: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
