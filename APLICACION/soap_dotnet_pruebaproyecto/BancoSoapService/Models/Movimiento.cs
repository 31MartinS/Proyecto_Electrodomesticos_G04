using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoSoapService.Models
{
    [Table("MOVIMIENTO")]
    public class Movimiento
    {
        [Key]
        [Column("id_movimiento")]
        public int IdMovimiento { get; set; }

        [Required]
        [Column("id_cuenta")]
        public int IdCuenta { get; set; }

        [Required]
        [Column("fecha_movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [Required]
        [StringLength(20)]
        [Column("tipo_movimiento")]
        public string TipoMovimiento { get; set; } = string.Empty;

        [Required]
        [Column("monto", TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [StringLength(255)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        // Navegación
        [ForeignKey("IdCuenta")]
        public virtual Cuenta? Cuenta { get; set; }
    }
}
