using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComercializadoraAPI.Models
{
    [Table("CLIENTE_COMERCIALIZADORA")]
    public class ClienteComercializadora
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(20)]
        [Column("cedula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("apellidos")]
        public string Apellidos { get; set; } = string.Empty;

        [StringLength(200)]
        [Column("direccion")]
        public string? Direccion { get; set; }

        [StringLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [StringLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        // Navegación
        public virtual ICollection<Factura> Facturas { get; set; } = new List<Factura>();
    }
}
