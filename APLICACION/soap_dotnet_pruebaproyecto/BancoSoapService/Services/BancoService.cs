using BancoSoapService.Data;
using BancoSoapService.Models;
using BancoSoapService.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BancoSoapService.Services
{
    public class BancoService : IBancoService
    {
        private readonly BancoDbContext _context;
        private readonly ILogger<BancoService> _logger;

        public BancoService(BancoDbContext context, ILogger<BancoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public EvaluateCreditResponse EvaluateCredit(EvaluateCreditRequest request)
        {
            try
            {
                _logger.LogInformation($"Evaluando crédito para cédula: {request.Cedula}");

                var response = new EvaluateCreditResponse
                {
                    SujetoCredito = false,
                    MontoMaximo = 0,
                    Aprobado = false,
                    IdCredito = 0,
                    Mensaje = ""
                };

                // 1. Validar que el cliente existe
                var cliente = _context.Clientes
                    .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                    .FirstOrDefault(c => c.Cedula == request.Cedula);

                if (cliente == null)
                {
                    response.Mensaje = "Cliente no existe en el sistema";
                    return response;
                }

                // 2. Validar edad (mayor de 21 años)
                var edad = CalcularEdad(cliente.FechaNacimiento);
                if (edad < 21)
                {
                    response.Mensaje = "Cliente debe ser mayor de 21 años";
                    return response;
                }

                // 3. Validar estado civil (no puede estar casado)
                if (cliente.EstadoCivil.ToUpper() == "CASADO")
                {
                    response.Mensaje = "Cliente casado no puede solicitar crédito";
                    return response;
                }

                // 4. Validar que tenga al menos una cuenta
                if (!cliente.Cuentas.Any())
                {
                    response.Mensaje = "Cliente no tiene cuentas registradas";
                    return response;
                }

                // 5. Validar que no tenga créditos activos
                var creditosActivos = _context.Creditos
                    .Where(cr => cr.IdCliente == cliente.IdCliente && cr.Estado == "ACTIVO")
                    .Any();

                if (creditosActivos)
                {
                    response.Mensaje = "Cliente tiene créditos activos";
                    return response;
                }

                // 6. Validar depósitos en el último mes
                var fechaHaceUnMes = DateTime.Now.AddMonths(-1);
                var cuentasIds = cliente.Cuentas.Select(c => c.IdCuenta).ToList();
                
                var depositosUltimoMes = _context.Movimientos
                    .Where(m => cuentasIds.Contains(m.IdCuenta) &&
                               m.TipoMovimiento == "DEPOSITO" &&
                               m.FechaMovimiento >= fechaHaceUnMes)
                    .Any();

                if (!depositosUltimoMes)
                {
                    response.Mensaje = "Cliente no tiene depósitos en el último mes";
                    return response;
                }

                // Cliente es sujeto de crédito
                response.SujetoCredito = true;

                // 7. Calcular monto máximo de crédito
                var fechaHaceTresMeses = DateTime.Now.AddMonths(-3);
                
                var depositosTresMeses = _context.Movimientos
                    .Where(m => cuentasIds.Contains(m.IdCuenta) &&
                               m.TipoMovimiento == "DEPOSITO" &&
                               m.FechaMovimiento >= fechaHaceTresMeses)
                    .Sum(m => (decimal?)m.Monto) ?? 0;

                var retirosTresMeses = _context.Movimientos
                    .Where(m => cuentasIds.Contains(m.IdCuenta) &&
                               m.TipoMovimiento == "RETIRO" &&
                               m.FechaMovimiento >= fechaHaceTresMeses)
                    .Sum(m => (decimal?)m.Monto) ?? 0;

                decimal promedioDepositos = depositosTresMeses / 3;
                decimal promedioRetiros = retirosTresMeses / 3;

                // Fórmula: MontoMáximo = (PromedioDepósitos - PromedioRetiros) * 10
                decimal montoMaximo = (promedioDepositos - promedioRetiros) * 10;

                if (montoMaximo <= 0)
                {
                    response.Mensaje = "Monto máximo de crédito no es suficiente";
                    return response;
                }

                response.MontoMaximo = montoMaximo;

                // 8. Validar si el precio del electrodoméstico está dentro del monto máximo
                if (request.PrecioElectrodomestico > montoMaximo)
                {
                    response.Mensaje = $"Precio del electrodoméstico ({request.PrecioElectrodomestico:C}) excede el monto máximo ({montoMaximo:C})";
                    return response;
                }

                // 9. Validar plazo de meses (entre 1 y 36)
                if (request.PlazoMeses < 1 || request.PlazoMeses > 36)
                {
                    response.Mensaje = "El plazo debe estar entre 1 y 36 meses";
                    return response;
                }

                // 10. Crear el crédito
                var cuenta = cliente.Cuentas.First();
                var credito = new Credito
                {
                    IdCliente = cliente.IdCliente,
                    IdCuenta = cuenta.IdCuenta,
                    MontoCredito = request.PrecioElectrodomestico,
                    PlazoMeses = request.PlazoMeses,
                    TasaAnual = 16.00m, // 16% anual
                    FechaInicio = DateTime.Now,
                    Estado = "ACTIVO"
                };

                _context.Creditos.Add(credito);
                _context.SaveChanges();

                // 11. Generar tabla de amortización
                GenerarTablaAmortizacion(credito);

                response.Aprobado = true;
                response.IdCredito = credito.IdCredito;
                response.Mensaje = "Crédito aprobado exitosamente";

                _logger.LogInformation($"Crédito aprobado. IdCredito: {credito.IdCredito}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al evaluar crédito");
                return new EvaluateCreditResponse
                {
                    SujetoCredito = false,
                    MontoMaximo = 0,
                    Aprobado = false,
                    IdCredito = 0,
                    Mensaje = $"Error interno: {ex.Message}"
                };
            }
        }

        public GetAmortizationScheduleResponse GetAmortizationSchedule(GetAmortizationScheduleRequest request)
        {
            try
            {
                var cuotas = _context.CuotasCredito
                    .Where(cc => cc.IdCredito == request.IdCredito)
                    .OrderBy(cc => cc.NumeroCuota)
                    .Select(cc => new AmortizationCuota
                    {
                        NumeroCuota = cc.NumeroCuota,
                        FechaVencimiento = cc.FechaVencimiento,
                        ValorCuota = cc.ValorCuota,
                        InteresPagado = cc.InteresPagado,
                        CapitalPagado = cc.CapitalPagado,
                        SaldoRestante = cc.SaldoRestante
                    })
                    .ToList();

                if (!cuotas.Any())
                {
                    return new GetAmortizationScheduleResponse
                    {
                        Cuotas = new List<AmortizationCuota>(),
                        Mensaje = "No se encontró tabla de amortización para el crédito especificado"
                    };
                }

                return new GetAmortizationScheduleResponse
                {
                    Cuotas = cuotas,
                    Mensaje = "Tabla de amortización obtenida exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener tabla de amortización");
                return new GetAmortizationScheduleResponse
                {
                    Cuotas = new List<AmortizationCuota>(),
                    Mensaje = $"Error interno: {ex.Message}"
                };
            }
        }

        public GetClientInfoResponse GetClientInfo(GetClientInfoRequest request)
        {
            try
            {
                var cliente = _context.Clientes
                    .Include(c => c.Cuentas)
                    .FirstOrDefault(c => c.Cedula == request.Cedula);

                if (cliente == null)
                {
                    return new GetClientInfoResponse
                    {
                        Cliente = null,
                        Mensaje = "Cliente no encontrado"
                    };
                }

                var clientInfo = new ClientInfo
                {
                    Cedula = cliente.Cedula,
                    NombreCompleto = $"{cliente.Nombres} {cliente.Apellidos}",
                    FechaNacimiento = cliente.FechaNacimiento,
                    EstadoCivil = cliente.EstadoCivil,
                    Cuentas = cliente.Cuentas.Select(c => new CuentaInfo
                    {
                        NumeroCuenta = c.NumeroCuenta,
                        TipoCuenta = c.TipoCuenta,
                        Saldo = c.Saldo
                    }).ToList()
                };

                return new GetClientInfoResponse
                {
                    Cliente = clientInfo,
                    Mensaje = "Información obtenida exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del cliente");
                return new GetClientInfoResponse
                {
                    Cliente = null,
                    Mensaje = $"Error interno: {ex.Message}"
                };
            }
        }

        public CheckCreditEligibilityResponse CheckCreditEligibility(CheckCreditEligibilityRequest request)
        {
            try
            {
                _logger.LogInformation($"Verificando elegibilidad de crédito para cédula: {request.Cedula}");

                var response = new CheckCreditEligibilityResponse
                {
                    SujetoCredito = false,
                    MontoMaximo = 0,
                    Aprobado = false,
                    Mensaje = ""
                };

                // 1. Validar que el cliente existe
                var cliente = _context.Clientes
                    .Include(c => c.Cuentas)
                    .ThenInclude(cu => cu.Movimientos)
                    .FirstOrDefault(c => c.Cedula == request.Cedula);

                if (cliente == null)
                {
                    response.Mensaje = "Cliente no existe en el sistema";
                    return response;
                }

                // 2. Validar edad (mayor de 21 años)
                var edad = CalcularEdad(cliente.FechaNacimiento);
                if (edad < 21)
                {
                    response.Mensaje = "Cliente debe ser mayor de 21 años";
                    return response;
                }

                // 3. Validar estado civil (no puede estar casado)
                if (cliente.EstadoCivil.ToUpper() == "CASADO")
                {
                    response.Mensaje = "Cliente casado no puede solicitar crédito";
                    return response;
                }

                // 4. Validar que tenga al menos una cuenta
                if (!cliente.Cuentas.Any())
                {
                    response.Mensaje = "Cliente no tiene cuentas registradas";
                    return response;
                }

                // 5. Validar que no tenga créditos activos
                var creditosActivos = _context.Creditos
                    .Where(cr => cr.IdCliente == cliente.IdCliente && cr.Estado == "ACTIVO")
                    .Any();

                if (creditosActivos)
                {
                    response.Mensaje = "Cliente tiene créditos activos";
                    return response;
                }

                // 6. Validar depósitos en el último mes
                var fechaHaceUnMes = DateTime.Now.AddMonths(-1);
                var cuentasIds = cliente.Cuentas.Select(c => c.IdCuenta).ToList();
                
                var depositosUltimoMes = _context.Movimientos
                    .Where(m => cuentasIds.Contains(m.IdCuenta) &&
                               m.TipoMovimiento == "DEPOSITO" &&
                               m.FechaMovimiento >= fechaHaceUnMes)
                    .Any();

                if (!depositosUltimoMes)
                {
                    response.Mensaje = "Cliente no tiene depósitos en el último mes";
                    return response;
                }

                // Cliente es sujeto de crédito
                response.SujetoCredito = true;

                // 7. Calcular monto máximo de crédito
                var fechaHaceTresMeses = DateTime.Now.AddMonths(-3);
                
                var depositosTresMeses = _context.Movimientos
                    .Where(m => cuentasIds.Contains(m.IdCuenta) &&
                               m.TipoMovimiento == "DEPOSITO" &&
                               m.FechaMovimiento >= fechaHaceTresMeses)
                    .Sum(m => (decimal?)m.Monto) ?? 0;

                var retirosTresMeses = _context.Movimientos
                    .Where(m => cuentasIds.Contains(m.IdCuenta) &&
                               m.TipoMovimiento == "RETIRO" &&
                               m.FechaMovimiento >= fechaHaceTresMeses)
                    .Sum(m => (decimal?)m.Monto) ?? 0;

                decimal promedioDepositos = depositosTresMeses / 3;
                decimal promedioRetiros = retirosTresMeses / 3;

                // Fórmula: MontoMáximo = (PromedioDepósitos - PromedioRetiros) * 10
                decimal montoMaximo = (promedioDepositos - promedioRetiros) * 10;

                if (montoMaximo <= 0)
                {
                    response.Mensaje = "Monto máximo de crédito no es suficiente";
                    return response;
                }

                response.MontoMaximo = montoMaximo;

                // 8. Validar si el precio del electrodoméstico está dentro del monto máximo
                if (request.PrecioElectrodomestico > montoMaximo)
                {
                    response.Mensaje = $"Precio del electrodoméstico ({request.PrecioElectrodomestico:C}) excede el monto máximo ({montoMaximo:C})";
                    return response;
                }

                // TODO: Aprobado sin crear el crédito
                response.Aprobado = true;
                response.Mensaje = "Cliente elegible para crédito";

                _logger.LogInformation($"Cliente elegible. MontoMaximo: {montoMaximo}");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar elegibilidad");
                return new CheckCreditEligibilityResponse
                {
                    SujetoCredito = false,
                    MontoMaximo = 0,
                    Aprobado = false,
                    Mensaje = $"Error interno: {ex.Message}"
                };
            }
        }

        public CreateCreditResponse CreateCredit(CreateCreditRequest request)
        {
            try
            {
                _logger.LogInformation($"Creando crédito para cédula: {request.Cedula}");

                // 1. Validar que el cliente existe
                var cliente = _context.Clientes
                    .Include(c => c.Cuentas)
                    .FirstOrDefault(c => c.Cedula == request.Cedula);

                if (cliente == null)
                {
                    return new CreateCreditResponse
                    {
                        Aprobado = false,
                        IdCredito = 0,
                        Mensaje = "Cliente no existe en el sistema"
                    };
                }

                // 2. Validar que no tenga créditos activos
                var creditosActivos = _context.Creditos
                    .Where(cr => cr.IdCliente == cliente.IdCliente && cr.Estado == "ACTIVO")
                    .Any();

                if (creditosActivos)
                {
                    return new CreateCreditResponse
                    {
                        Aprobado = false,
                        IdCredito = 0,
                        Mensaje = "Cliente tiene créditos activos"
                    };
                }

                // 3. Validar plazo de meses (entre 1 y 36)
                if (request.PlazoMeses < 1 || request.PlazoMeses > 36)
                {
                    return new CreateCreditResponse
                    {
                        Aprobado = false,
                        IdCredito = 0,
                        Mensaje = "El plazo debe estar entre 1 y 36 meses"
                    };
                }

                // 4. Crear el crédito
                var cuenta = cliente.Cuentas.First();
                var credito = new Credito
                {
                    IdCliente = cliente.IdCliente,
                    IdCuenta = cuenta.IdCuenta,
                    MontoCredito = request.MontoCredito,
                    PlazoMeses = request.PlazoMeses,
                    TasaAnual = 16.00m,
                    FechaInicio = DateTime.Now,
                    Estado = "ACTIVO"
                };

                _context.Creditos.Add(credito);
                _context.SaveChanges();

                // 5. Generar tabla de amortización
                GenerarTablaAmortizacion(credito);

                _logger.LogInformation($"Crédito creado exitosamente. IdCredito: {credito.IdCredito}");

                return new CreateCreditResponse
                {
                    Aprobado = true,
                    IdCredito = credito.IdCredito,
                    Mensaje = "Crédito creado exitosamente"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear crédito");
                return new CreateCreditResponse
                {
                    Aprobado = false,
                    IdCredito = 0,
                    Mensaje = $"Error interno: {ex.Message}"
                };
            }
        }

        private int CalcularEdad(DateTime fechaNacimiento)
        {
            var hoy = DateTime.Today;
            var edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
            return edad;
        }

        private void GenerarTablaAmortizacion(Credito credito)
        {
            // Tasa mensual
            decimal tasaMensual = credito.TasaAnual / 12 / 100;

            // Cuota fija usando fórmula de amortización francesa
            decimal cuotaFija = credito.MontoCredito * 
                (tasaMensual * (decimal)Math.Pow((double)(1 + tasaMensual), credito.PlazoMeses)) /
                ((decimal)Math.Pow((double)(1 + tasaMensual), credito.PlazoMeses) - 1);

            decimal saldoRestante = credito.MontoCredito;
            DateTime fechaVencimiento = credito.FechaInicio.AddMonths(1);

            for (int i = 1; i <= credito.PlazoMeses; i++)
            {
                decimal interesPagado = saldoRestante * tasaMensual;
                decimal capitalPagado = cuotaFija - interesPagado;
                saldoRestante -= capitalPagado;

                // Ajuste para última cuota por redondeos
                if (i == credito.PlazoMeses && saldoRestante != 0)
                {
                    capitalPagado += saldoRestante;
                    cuotaFija = interesPagado + capitalPagado;
                    saldoRestante = 0;
                }

                var cuota = new CuotaCredito
                {
                    IdCredito = credito.IdCredito,
                    NumeroCuota = i,
                    FechaVencimiento = fechaVencimiento,
                    ValorCuota = Math.Round(cuotaFija, 2),
                    InteresPagado = Math.Round(interesPagado, 2),
                    CapitalPagado = Math.Round(capitalPagado, 2),
                    SaldoRestante = Math.Round(Math.Max(0, saldoRestante), 2)
                };

                _context.CuotasCredito.Add(cuota);
                fechaVencimiento = fechaVencimiento.AddMonths(1);
            }

            _context.SaveChanges();
        }
    }
}
