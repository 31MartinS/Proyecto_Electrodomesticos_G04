using ClienteMovil.Models;

namespace ClienteMovil.Pages;

public partial class CompraEfectivoExitoPage : ContentPage
{
    public FacturaResponseDto Factura { get; set; }
    
    public CompraEfectivoExitoPage(FacturaResponseDto factura)
    {
        InitializeComponent();
        Factura = factura;
        BindingContext = this;
    }

    private async void OnFinalizarClicked(object sender, EventArgs e)
    {
        // Volver al menú principal
        await Navigation.PopToRootAsync();
    }
}
