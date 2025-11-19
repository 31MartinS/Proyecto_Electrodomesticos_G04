using ClienteMovil.Models;
using ClienteMovil.Services;

namespace ClienteMovil.Pages;

public partial class CreditoExitoPage : ContentPage
{
    public FacturaResponseDto Factura { get; set; }
    
    public CreditoExitoPage(FacturaResponseDto factura, GetAmortizationScheduleResponse amortizacion)
    {
        InitializeComponent();
        Factura = factura;
        BindingContext = this;
        CuotasCollection.ItemsSource = amortizacion.Cuotas;
    }

    private async void OnFinalizarClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}
