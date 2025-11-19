using System.Windows;
using ClienteEscritorio.Models;

namespace ClienteEscritorio.Views
{
    public partial class VentaEfectivoExitoWindow : Window
    {
        private readonly FacturaResponseDto _factura;

        public VentaEfectivoExitoWindow(FacturaResponseDto factura)
        {
            InitializeComponent();
            _factura = factura;
        }

        private void BtnVolver_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
