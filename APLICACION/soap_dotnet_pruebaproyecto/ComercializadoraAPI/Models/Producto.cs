using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercializadoraAPI.Models
{
    [Table("PRODUCTO")]
    public class Producto
    {
        [Key]
        [Column("id_producto")]
        public int IdProducto { get; set; }

        [Required]
        [StringLength(20)]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Required]
        [Column("precio_venta", TypeName = "decimal(18,2)")]
        public decimal PrecioVenta { get; set; }

        // Navegación
        public virtual ICollection<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();

        [StringLength(255)]
        [Column("image_filename")]
        public string? ImageFileName { get; set; }
    }
}
