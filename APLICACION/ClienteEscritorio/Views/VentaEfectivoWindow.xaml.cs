using System;
using System.Collections.Generic;
using System.Windows;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class VentaEfectivoWindow : Window
    {
        private readonly string _cedula;
        private readonly ProductoDto _producto;
        private decimal _subtotal;
        private decimal _descuento;
        private decimal _total;

        public VentaEfectivoWindow(string cedula, ProductoDto producto)
        {
            InitializeComponent();
            _cedula = cedula;
            _producto = producto;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _subtotal = _producto.PrecioVenta;
            _descuento = _subtotal * 0.33m;
            _total = _subtotal - _descuento;
            TxtTotal.Text = _total.ToString("C2");
        }

        private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var factura = new CreateFacturaDto
                {
                    Cedula = _cedula,
                    FormaPago = "EFECTIVO",
                    Detalles = new List<DetalleFacturaDto>
                    {
                        new DetalleFacturaDto { IdProducto = _producto.IdProducto, Cantidad = 1 }
                    }
                };

                var response = await ApiService.CreateFacturaAsync(factura);

                // Mostrar pantalla de éxito
                var exitoWindow = new VentaEfectivoExitoWindow(response);
                exitoWindow.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar factura: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
