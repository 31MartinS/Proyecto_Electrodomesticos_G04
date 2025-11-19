using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoSoapService.Models
{
    [Table("CREDITO")]
    public class Credito
    {
        [Key]
        [Column("id_credito")]
        public int IdCredito { get; set; }

        [Required]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required]
        [Column("id_cuenta")]
        public int IdCuenta { get; set; }

        [Required]
        [Column("monto_credito", TypeName = "decimal(18,2)")]
        public decimal MontoCredito { get; set; }

        [Required]
        [Column("plazo_meses")]
        public int PlazoMeses { get; set; }

        [Required]
        [Column("tasa_anual", TypeName = "decimal(5,2)")]
        public decimal TasaAnual { get; set; }

        [Required]
        [Column("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [Required]
        [StringLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = string.Empty;

        // Navegación
        [ForeignKey("IdCliente")]
        public virtual Cliente? Cliente { get; set; }

        [ForeignKey("IdCuenta")]
        public virtual Cuenta? Cuenta { get; set; }

        public virtual ICollection<CuotaCredito> Cuotas { get; set; } = new List<CuotaCredito>();
    }
}
