using System.Collections.Generic;
using BancoServiceRef;

namespace ClienteWeb.Models
{
    public class VentaCedulaViewModel
    {
        public string Cedula { get; set; } = string.Empty;
        public string? Error { get; set; }
    }

    public class VentaProductosViewModel
    {
        public string Cedula { get; set; } = string.Empty;
        public List<ProductoDto> Productos { get; set; } = new();
    }

    public class VentaCreditoViewModel
    {
        public string Cedula { get; set; } = string.Empty;
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioProducto { get; set; }

        public EvaluateCreditResponse Evaluacion { get; set; } = new();
        public int IdCreditoBanco { get; set; }
        public int PlazoMesesSeleccionado { get; set; }

        // 🔥 ESTA PROPIEDAD FALTABA
        public List<int> OpcionesPlazo { get; set; } = new() { 3, 6, 9, 12, 18, 24 };
    }



    public class VentaCreditoExitoViewModel
    {
        public FacturaResponseDto Factura { get; set; } = new();
        public List<AmortizationCuota> Cuotas { get; set; } = new();
    }

    public class VentaEfectivoViewModel
    {
        public string Cedula { get; set; } = string.Empty;

        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioProducto { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
    }

    public class VentaEfectivoExitoViewModel
    {
        public FacturaResponseDto Factura { get; set; } = new FacturaResponseDto();
    }
}
