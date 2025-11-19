using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BancoSoapService.Services.Contracts
{
    [DataContract(Namespace = "http://tempuri.org/")]
    public class EvaluateCreditRequest
    {
        [DataMember(Order = 1)]
        public string Cedula { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public decimal PrecioElectrodomestico { get; set; }

        [DataMember(Order = 3)]
        public int PlazoMeses { get; set; }
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class EvaluateCreditResponse
    {
        [DataMember(Order = 1)]
        public bool SujetoCredito { get; set; }

        [DataMember(Order = 2)]
        public decimal MontoMaximo { get; set; }

        [DataMember(Order = 3)]
        public bool Aprobado { get; set; }

        [DataMember(Order = 4)]
        public int IdCredito { get; set; }

        [DataMember(Order = 5)]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class GetAmortizationScheduleRequest
    {
        [DataMember(Order = 1)]
        public int IdCredito { get; set; }
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class AmortizationCuota
    {
        [DataMember(Order = 1)]
        public int NumeroCuota { get; set; }

        [DataMember(Order = 2)]
        public DateTime FechaVencimiento { get; set; }

        [DataMember(Order = 3)]
        public decimal ValorCuota { get; set; }

        [DataMember(Order = 4)]
        public decimal InteresPagado { get; set; }

        [DataMember(Order = 5)]
        public decimal CapitalPagado { get; set; }

        [DataMember(Order = 6)]
        public decimal SaldoRestante { get; set; }
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class GetAmortizationScheduleResponse
    {
        [DataMember(Order = 1)]
        public List<AmortizationCuota> Cuotas { get; set; } = new List<AmortizationCuota>();

        [DataMember(Order = 2)]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class GetClientInfoRequest
    {
        [DataMember(Order = 1)]
        public string Cedula { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class ClientInfo
    {
        [DataMember(Order = 1)]
        public string Cedula { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string NombreCompleto { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public DateTime FechaNacimiento { get; set; }

        [DataMember(Order = 4)]
        public string EstadoCivil { get; set; } = string.Empty;

        [DataMember(Order = 5)]
        public List<CuentaInfo> Cuentas { get; set; } = new List<CuentaInfo>();
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class CuentaInfo
    {
        [DataMember(Order = 1)]
        public string NumeroCuenta { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string TipoCuenta { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public decimal Saldo { get; set; }
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class GetClientInfoResponse
    {
        [DataMember(Order = 1)]
        public ClientInfo? Cliente { get; set; }

        [DataMember(Order = 2)]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class CheckCreditEligibilityRequest
    {
        [DataMember(Order = 1)]
        public string Cedula { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public decimal PrecioElectrodomestico { get; set; }
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class CheckCreditEligibilityResponse
    {
        [DataMember(Order = 1)]
        public bool SujetoCredito { get; set; }

        [DataMember(Order = 2)]
        public decimal MontoMaximo { get; set; }

        [DataMember(Order = 3)]
        public bool Aprobado { get; set; }

        [DataMember(Order = 4)]
        public string Mensaje { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class CreateCreditRequest
    {
        [DataMember(Order = 1)]
        public string Cedula { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public decimal MontoCredito { get; set; }

        [DataMember(Order = 3)]
        public int PlazoMeses { get; set; }
    }

    [DataContract(Namespace = "http://tempuri.org/")]
    public class CreateCreditResponse
    {
        [DataMember(Order = 1)]
        public bool Aprobado { get; set; }

        [DataMember(Order = 2)]
        public int IdCredito { get; set; }

        [DataMember(Order = 3)]
        public string Mensaje { get; set; } = string.Empty;
    }
}
