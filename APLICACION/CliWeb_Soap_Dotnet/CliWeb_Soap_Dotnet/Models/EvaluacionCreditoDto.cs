using System.Collections.Generic;
using BancoServiceRef;

namespace ClienteWeb.Models
{
    public class EvaluacionCreditoDto
    {
        public string Cedula { get; set; } = string.Empty;
        public ProductoDto Producto { get; set; } = new ProductoDto();
        public int PlazoMeses { get; set; }

        public EvaluateCreditResponse Evaluacion { get; set; }
            = new EvaluateCreditResponse();

        public List<AmortizationCuota> TablaAmortizacion { get; set; }
            = new List<AmortizationCuota>();

        public bool CompraAprobada => Evaluacion.Aprobado;
        public decimal MontoMaximo => Evaluacion.MontoMaximo;
        public int IdCredito => Evaluacion.IdCredito;
    }
}
