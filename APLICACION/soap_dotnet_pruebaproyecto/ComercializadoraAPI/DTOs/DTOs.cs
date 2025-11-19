namespace ComercializadoraAPI.DTOs
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

    public class DetalleFacturaDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
    }

    public class CreateFacturaDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string FormaPago { get; set; } = string.Empty; // EFECTIVO o CREDITO
        public int PlazoMeses { get; set; }
        public int? IdCreditoBanco { get; set; } // ID del crédito ya aprobado (opcional, solo para crédito)
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
}
