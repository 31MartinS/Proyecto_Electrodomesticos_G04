using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class VentaCreditoCarritoWindow : Window
    {
        private readonly string _cedula;
        private readonly List<CarritoItem> _carritoItems;
        private readonly decimal _total;

        public VentaCreditoCarritoWindow(string cedula, List<CarritoItem> carritoItems, decimal total)
        {
            InitializeComponent();
            _cedula = cedula;
            _carritoItems = carritoItems;
            _total = total;
            
            TxtTotal.Text = _total.ToString("C2");
        }

        private async void BtnProcesar_Click(object sender, RoutedEventArgs e)
        {
            // Validar plazo
            if (!int.TryParse(TxtPlazo.Text, out int plazoMeses) || plazoMeses < 1 || plazoMeses > 36)
            {
                MessageBox.Show("El plazo debe estar entre 1 y 36 meses", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                BtnProcesar.IsEnabled = false;
                TxtEstado.Text = "Consultando Central de Riesgos (SOAP)... Espere.";

                // Evaluar crédito en el banco
                var requestCredito = new EvaluateCreditRequest
                {
                    Cedula = _cedula,
                    PrecioElectrodomestico = _total,
                    PlazoMeses = plazoMeses
                };

                var respuestaBanco = await BancoSoapService.EvaluateCreditAsync(requestCredito);

                if (respuestaBanco.Aprobado)
                {
                    TxtEstado.Text = $"✓ Crédito APROBADO. ID Crédito: {respuestaBanco.IdCredito}";

                    // Crear factura con el crédito aprobado
                    var createFacturaDto = new CreateFacturaDto
                    {
                        Cedula = _cedula,
                        FormaPago = "Credito",
                        PlazoMeses = plazoMeses,
                        IdCreditoBanco = respuestaBanco.IdCredito,
                        Detalles = _carritoItems.Select(item => new DetalleFacturaDto
                        {
                            IdProducto = item.IdProducto,
                            Cantidad = item.Cantidad
                        }).ToList()
                    };

                    var resultado = await ApiService.CreateFacturaAsync(createFacturaDto);

                    // Mostrar ventana de éxito con tabla de amortización
                    var ventaExitoWindow = new VentaCreditoExitoWindow(resultado);
                    this.Close();
                    ventaExitoWindow.ShowDialog();
                }
                else
                {
                    TxtEstado.Text = $"✗ Crédito RECHAZADO: {respuestaBanco.Mensaje}";
                    MessageBox.Show(
                        $"Crédito rechazado.\n\nMotivo: {respuestaBanco.Mensaje}\nMonto máximo sugerido: {respuestaBanco.MontoMaximo:C2}",
                        "Crédito Rechazado",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    BtnProcesar.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                TxtEstado.Text = $"✗ Error: {ex.Message}";
                MessageBox.Show($"Error al procesar el crédito: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BtnProcesar.IsEnabled = true;
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
