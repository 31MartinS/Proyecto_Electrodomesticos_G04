using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using ClienteEscritorio.Models;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class VentaCreditoWindow : Window
    {
        private readonly string _cedula;
        private readonly ProductoDto _producto;
        private decimal _montoMaximo;
        private bool _aprobado;
        private string _mensajeEvaluacion = "";

        public VentaCreditoWindow(string cedula, ProductoDto producto)
        {
            InitializeComponent();
            _cedula = cedula;
            _producto = producto;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // SOLO verificar elegibilidad sin crear el crédito
                var request = new ClienteEscritorio.Services.CheckCreditEligibilityRequest
                {
                    Cedula = _cedula,
                    PrecioElectrodomestico = _producto.PrecioVenta
                };

                var response = await BancoSoapService.CheckCreditEligibilityAsync(request);

                _montoMaximo = response.MontoMaximo;
                _aprobado = response.Aprobado;
                _mensajeEvaluacion = response.Mensaje;

                TxtMontoMaximo.Text = _montoMaximo.ToString("C2");
                PanelElegible.Visibility = _aprobado ? Visibility.Visible : Visibility.Collapsed;
                PanelNoElegible.Visibility = !_aprobado ? Visibility.Visible : Visibility.Collapsed;
                TxtMensajeNoElegible.Text = _mensajeEvaluacion;
            }
            catch (Exception ex)
            {
                PanelElegible.Visibility = Visibility.Collapsed;
                PanelNoElegible.Visibility = Visibility.Visible;
                TxtMensajeNoElegible.Text = $"Error al evaluar crédito: {ex.Message}";
            }
        }


        private async void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!_aprobado)
            {
                MessageBox.Show("No es elegible para crédito", "Crédito rechazado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbCuotas.SelectedItem is ComboBoxItem item && int.TryParse(item.Content.ToString(), out int cuotas))
            {
                try
                {
                    // Crear el crédito en el banco
                    var creditRequest = new ClienteEscritorio.Services.CreateCreditRequest
                    {
                        Cedula = _cedula,
                        MontoCredito = _producto.PrecioVenta,
                        PlazoMeses = cuotas
                    };

                    var creditResponse = await BancoSoapService.CreateCreditAsync(creditRequest);

                    if (!creditResponse.Aprobado)
                    {
                        MessageBox.Show(creditResponse.Mensaje, "Error al crear crédito", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Crear la factura en la comercializadora
                    var factura = new CreateFacturaDto
                    {
                        Cedula = _cedula,
                        FormaPago = "CREDITO",
                        PlazoMeses = cuotas,
                        IdCreditoBanco = creditResponse.IdCredito, // Usar el ID del crédito recién creado
                        Detalles = new List<DetalleFacturaDto>
                        {
                            new DetalleFacturaDto { IdProducto = _producto.IdProducto, Cantidad = 1 }
                        }
                    };

                    var response = await ApiService.CreateFacturaAsync(factura);

                    if (!response.Aprobado)
                    {
                        MessageBox.Show(response.Mensaje, "Error al generar factura", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Mostrar pantalla de éxito y tabla de amortización
                    var exitoWindow = new VentaCreditoExitoWindow(response);
                    exitoWindow.ShowDialog();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al procesar compra: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Seleccione el número de cuotas", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
