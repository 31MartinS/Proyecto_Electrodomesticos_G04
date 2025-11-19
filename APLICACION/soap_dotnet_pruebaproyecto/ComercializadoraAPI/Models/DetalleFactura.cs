using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercializadoraAPI.Models
{
    [Table("DETALLE_FACTURA")]
    public class DetalleFactura
    {
        [Key]
        [Column("id_detalle")]
        public int IdDetalle { get; set; }

        [Required]
        [Column("id_factura")]
        public int IdFactura { get; set; }

        [Required]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario", TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column("total_linea", TypeName = "decimal(18,2)")]
        public decimal TotalLinea { get; set; }

        // Navegación
        [ForeignKey("IdFactura")]
        public virtual Factura? Factura { get; set; }

        [ForeignKey("IdProducto")]
        public virtual Producto? Producto { get; set; }
    }
}
