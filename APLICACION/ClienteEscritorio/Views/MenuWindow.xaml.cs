using System.Windows;

namespace ClienteEscritorio.Views
{
    public partial class MenuWindow : Window
    {
        public MenuWindow()
        {
            InitializeComponent();
        }

        private void BtnEditarInventario_Click(object sender, RoutedEventArgs e)
        {
            var inventarioWindow = new InventarioWindow();
            inventarioWindow.ShowDialog();
        }

        private void BtnRealizarVenta_Click(object sender, RoutedEventArgs e)
        {
            var ventaWindow = new VentaCedulaWindow();
            ventaWindow.ShowDialog();
        }

        private void BtnVerFacturas_Click(object sender, RoutedEventArgs e)
        {
            var facturasWindow = new FacturasWindow();
            facturasWindow.ShowDialog();
        }
    }
}
