using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercializadoraAPI.Models
{
    [Table("FACTURA")]
    public class Factura
    {
        [Key]
        [Column("id_factura")]
        public int IdFactura { get; set; }

        [Required]
        [StringLength(30)]
        [Column("numero_factura")]
        public string NumeroFactura { get; set; } = string.Empty;

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(20)]
        [Column("forma_pago")]
        public string FormaPago { get; set; } = string.Empty;

        [Required]
        [Column("subtotal", TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column("descuento", TypeName = "decimal(18,2)")]
        public decimal Descuento { get; set; }

        [Required]
        [Column("total", TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Column("id_credito_banco")]
        public int? IdCreditoBanco { get; set; }

        // Navegación
        [ForeignKey("IdCliente")]
        public virtual ClienteComercializadora? Cliente { get; set; }

        public virtual ICollection<DetalleFactura> Detalles { get; set; } = new List<DetalleFactura>();
    }
}
