using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BancoSoapService.Models
{
    [Table("CUOTA_CREDITO")]
    public class CuotaCredito
    {
        [Key]
        [Column("id_cuota")]
        public int IdCuota { get; set; }

        [Required]
        [Column("id_credito")]
        public int IdCredito { get; set; }

        [Required]
        [Column("numero_cuota")]
        public int NumeroCuota { get; set; }

        [Required]
        [Column("fecha_vencimiento")]
        public DateTime FechaVencimiento { get; set; }

        [Required]
        [Column("valor_cuota", TypeName = "decimal(18,2)")]
        public decimal ValorCuota { get; set; }

        [Required]
        [Column("interes_pagado", TypeName = "decimal(18,2)")]
        public decimal InteresPagado { get; set; }

        [Required]
        [Column("capital_pagado", TypeName = "decimal(18,2)")]
        public decimal CapitalPagado { get; set; }

        [Required]
        [Column("saldo_restante", TypeName = "decimal(18,2)")]
        public decimal SaldoRestante { get; set; }

        // Navegación
        [ForeignKey("IdCredito")]
        public virtual Credito? Credito { get; set; }
    }
}
