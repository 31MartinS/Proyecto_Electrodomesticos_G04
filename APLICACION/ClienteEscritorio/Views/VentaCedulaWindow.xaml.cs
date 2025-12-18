using System;
using System.Windows;
using ClienteEscritorio.Services;

namespace ClienteEscritorio.Views
{
    public partial class VentaCedulaWindow : Window
    {
        public VentaCedulaWindow()
        {
            InitializeComponent();
        }

        private async void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            string cedula = TxtCedula.Text.Trim();

            if (string.IsNullOrWhiteSpace(cedula))
            {
                MessageBox.Show("Por favor ingrese una cédula", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cedula.Length != 10)
            {
                MessageBox.Show("La cédula debe tener 10 dígitos", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Verificar si el cliente existe
                var cliente = await ApiService.GetClienteByCedulaAsync(cedula);
                
                if (cliente == null)
                {
                    MessageBox.Show("Cliente no encontrado. Por favor registre el cliente primero.", 
                        "Cliente no encontrado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Cliente existe, continuar a la pantalla de productos con carrito
                var carritoWindow = new VentaCarritoWindow(cedula);
                carritoWindow.ShowDialog();
                
                // Cerrar esta ventana si se completó la venta
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al validar cliente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVolver_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
