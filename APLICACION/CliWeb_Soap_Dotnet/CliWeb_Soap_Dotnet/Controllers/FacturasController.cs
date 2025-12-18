using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClienteWeb.Models;
using ClienteWeb.Services;
using Microsoft.AspNetCore.Mvc;
using AmortizationCuotaModel = ClienteWeb.Models.AmortizationCuota;

namespace ClienteWeb.Controllers
{
    public class FacturasController : Controller
    {
        private readonly BancoSoapService _banco;

        public FacturasController(BancoSoapService banco)
        {
            _banco = banco;
        }

        // Listar todas las facturas
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var facturas = await ApiService.GetFacturasAsync();
            var vm = new FacturasViewModel
            {
                Facturas = facturas
            };
            return View(vm);
        }

        // Ver detalle de una factura
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var factura = await ApiService.GetFacturaAsync(id);
            
            List<AmortizationCuotaModel>? cuotas = null;
            if (factura.IdCreditoBanco.HasValue)
            {
                var amortizacion = await _banco.GetAmortizationScheduleAsync(factura.IdCreditoBanco.Value);
                if (amortizacion.Cuotas != null)
                {
                    cuotas = amortizacion.Cuotas
                        .Select(c => new AmortizationCuotaModel
                        {
                            NumeroCuota = c.NumeroCuota,
                            FechaVencimiento = c.FechaVencimiento,
                            ValorCuota = c.ValorCuota,
                            InteresPagado = c.InteresPagado,
                            CapitalPagado = c.CapitalPagado,
                            SaldoRestante = c.SaldoRestante
                        })
                        .ToList();
                }
            }

            var vm = new FacturaDetalleViewModel
            {
                Factura = factura,
                Cuotas = cuotas
            };

            return View(vm);
        }
    }
}
