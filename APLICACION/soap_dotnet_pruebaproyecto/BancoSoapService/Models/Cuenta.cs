using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoSoapService.Models
{
    [Table("CUENTA")]
    public class Cuenta
    {
        [Key]
        [Column("id_cuenta")]
        public int IdCuenta { get; set; }

        [Required]
        [StringLength(20)]
        [Column("numero_cuenta")]
        public string NumeroCuenta { get; set; } = string.Empty;

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(20)]
        [Column("tipo_cuenta")]
        public string TipoCuenta { get; set; } = string.Empty;

        [Required]
        [Column("saldo", TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }

        [Required]
        [Column("fecha_apertura")]
        public DateTime FechaApertura { get; set; }

        // Navegación
        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }
        
        public virtual ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
        public virtual ICollection<Credito> Creditos { get; set; } = new List<Credito>();
    }
}
