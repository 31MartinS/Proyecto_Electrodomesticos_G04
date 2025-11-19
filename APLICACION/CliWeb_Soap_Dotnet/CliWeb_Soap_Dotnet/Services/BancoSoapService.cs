using System.Threading.Tasks;
using BancoServiceRef;                 // namespace del service reference
using Microsoft.Extensions.Configuration;

namespace ClienteWeb.Services
{
    public class BancoSoapService
    {
        private readonly BancoServiceClient _client;

        public BancoSoapService(IConfiguration config)
        {
            // URL configurable, o usa la del WSDL
            var url = config["BancoSettings:ServiceUrl"]
                      ?? "http://localhost:5000/BancoService.asmx";

            _client = new BancoServiceClient(
                BancoServiceClient.EndpointConfiguration.BasicHttpBinding_IBancoService_soap,
                url
            );
        }

        public Task<EvaluateCreditResponse> EvaluateCreditAsync(
            string cedula,
            decimal precioElectrodomestico,
            int plazoMeses)
        {
            var request = new EvaluateCreditRequest
            {
                Cedula = cedula,
                PrecioElectrodomestico = precioElectrodomestico,
                PlazoMeses = plazoMeses
            };

            return _client.EvaluateCreditAsync(request);
        }

        public Task<GetAmortizationScheduleResponse> GetAmortizationScheduleAsync(int idCredito)
        {
            var request = new GetAmortizationScheduleRequest
            {
                IdCredito = idCredito
            };

            return _client.GetAmortizationScheduleAsync(request);
        }

        public Task<GetClientInfoResponse> GetClientInfoAsync(string cedula)
        {
            var request = new GetClientInfoRequest
            {
                Cedula = cedula
            };

            return _client.GetClientInfoAsync(request);
        }
    }
}
