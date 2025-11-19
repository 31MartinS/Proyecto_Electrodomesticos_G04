using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoSoapService.Models
{
    [Table("CLIENTE")]
    public class Cliente
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

        [Required]
        [Column("fecha_nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(20)]
        [Column("estado_civil")]
        public string EstadoCivil { get; set; } = string.Empty;

        // Navegación
        public virtual ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();
        public virtual ICollection<Credito> Creditos { get; set; } = new List<Credito>();
    }
}
