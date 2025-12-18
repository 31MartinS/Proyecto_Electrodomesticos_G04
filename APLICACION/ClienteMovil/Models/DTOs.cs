using System;
using System.Collections.Generic;

namespace ClienteMovil.Models
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateProductoDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
    }

    public class ClienteComercializadora
    {
        public int IdCliente { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
    }

    public class DetalleFacturaDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class CreateFacturaDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty;
        public int PlazoMeses { get; set; }
        public int? IdCreditoBanco { get; set; }
        public List<DetalleFacturaDto> Detalles { get; set; } = new List<DetalleFacturaDto>();
    }

    public class FacturaResponseDto
    {
        public int IdFactura { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public int? IdCreditoBanco { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool Aprobado { get; set; }
    }

    public class AmortizationCuota
    {
        public int NumeroCuota { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public decimal ValorCuota { get; set; }
        public decimal InteresPagado { get; set; }
        public decimal CapitalPagado { get; set; }
        public decimal SaldoRestante { get; set; }
    }

    public class EvaluateCreditResponse
    {
        public bool SujetoCredito { get; set; }
        public decimal MontoMaximo { get; set; }
        public bool Aprobado { get; set; }
        public int IdCredito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }

    public class FacturaListDto
    {
        public int IdFactura { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string FormaPago { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public int? IdCreditoBanco { get; set; }
        public ClienteFacturaDto Cliente { get; set; } = new ClienteFacturaDto();
        public List<DetalleFacturaListDto> Detalles { get; set; } = new List<DetalleFacturaListDto>();
    }

    public class ClienteFacturaDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
    }

    public class DetalleFacturaListDto
    {
        public int IdDetalle { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal TotalLinea { get; set; }
        public ProductoFacturaDto Producto { get; set; } = new ProductoFacturaDto();
    }

    public class ProductoFacturaDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class AmortizacionResponseDto
    {
        public List<AmortizationCuota> Cuotas { get; set; } = new List<AmortizationCuota>();
        public string Mensaje { get; set; } = string.Empty;
    }

    // Carrito de compras
    public class CarritoItem
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal => Cantidad * Precio;
        public string? ImageUrl { get; set; }
    }
}
