using System;
using System.Linq;
using System.Windows;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class FacturaDetalleWindow : Window
    {
        private readonly FacturaListDto _factura;

        public FacturaDetalleWindow(FacturaListDto factura)
        {
            InitializeComponent();
            _factura = factura;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatosFactura();

            if (_factura.IdCreditoBanco.HasValue)
            {
                await CargarAmortizacion();
            }
        }

        private void CargarDatosFactura()
        {
            TxtNumero.Text = _factura.NumeroFactura;
            TxtFecha.Text = _factura.Fecha.ToString("dd/MM/yyyy HH:mm");
            TxtCliente.Text = $"{_factura.Cliente.Nombres} {_factura.Cliente.Apellidos} ({_factura.Cliente.Cedula})";
            TxtFormaPago.Text = _factura.FormaPago;

            // Cargar productos
            DgProductos.ItemsSource = _factura.Detalles;

            // Mostrar totales
            TxtSubtotal.Text = _factura.Subtotal.ToString("C2");

            if (_factura.Descuento > 0)
            {
                LblDescuento.Visibility = Visibility.Visible;
                TxtDescuento.Visibility = Visibility.Visible;
                TxtDescuento.Text = $"-{_factura.Descuento.ToString("C2")}";
            }
            else
            {
                LblDescuento.Visibility = Visibility.Collapsed;
                TxtDescuento.Visibility = Visibility.Collapsed;
            }

            TxtTotal.Text = _factura.Total.ToString("C2");
        }

        private async System.Threading.Tasks.Task CargarAmortizacion()
        {
            try
            {
                var response = await BancoSoapService.GetAmortizationScheduleAsync(_factura.IdCreditoBanco!.Value);

                if (response.Cuotas != null && response.Cuotas.Any())
                {
                    BorderAmortizacion.Visibility = Visibility.Visible;
                    DgAmortizacion.ItemsSource = response.Cuotas;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar tabla de amortización: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
