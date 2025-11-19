using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace ComercializadoraAPI.SoapClient
{
    public class BancoServiceClient : IDisposable
    {
        private readonly ChannelFactory<IBancoServiceClient> _channelFactory;
        private IBancoServiceClient? _client;

        public BancoServiceClient(string serviceUrl)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(5),
                ReceiveTimeout = TimeSpan.FromMinutes(5)
            };

            // Muy importante para SOAPCore
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;

            var endpoint = new EndpointAddress(serviceUrl);

            _channelFactory = new ChannelFactory<IBancoServiceClient>(binding, endpoint);
        }

        /// <summary>
        /// Obtiene el cliente SOAP, lo crea si no existe
        /// </summary>
        private IBancoServiceClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = _channelFactory.CreateChannel();
                }
                return _client;
            }
        }

        // ===========================
        //     MÉTODOS DEL BANCO
        // ===========================

        public async Task<EvaluateCreditResponse> EvaluateCreditAsync(EvaluateCreditRequest request)
        {
            try
            {
                Console.WriteLine(
                    $"[SOAP CLIENT] EvaluateCredit → Enviando Cedula='{request.Cedula}', " +
                    $"Precio={request.PrecioElectrodomestico}, Plazo={request.PlazoMeses}");

                // IMPORTANTE: las operaciones SOAP son síncronas internamente
                return await Task.Run(() => Client.EvaluateCredit(request));
            }
            catch
            {
                AbortClient();
                throw;
            }
        }

        public async Task<GetAmortizationScheduleResponse> GetAmortizationScheduleAsync(GetAmortizationScheduleRequest request)
        {
            try
            {
                return await Task.Run(() => Client.GetAmortizationSchedule(request));
            }
            catch
            {
                AbortClient();
                throw;
            }
        }

        public async Task<GetClientInfoResponse> GetClientInfoAsync(GetClientInfoRequest request)
        {
            try
            {
                return await Task.Run(() => Client.GetClientInfo(request));
            }
            catch
            {
                AbortClient();
                throw;
            }
        }

        // ===========================
        //      CONTROL DEL CANAL
        // ===========================

        private void AbortClient()
        {
            if (_client is IClientChannel channel)
            {
                channel.Abort();
            }
        }

        public void Dispose()
        {
            if (_client is IClientChannel channel)
            {
                try
                {
                    if (channel.State == CommunicationState.Faulted)
                        channel.Abort();
                    else
                        channel.Close();
                }
                catch
                {
                    channel.Abort();
                }
            }

            try
            {
                _channelFactory.Close();
            }
            catch
            {
                _channelFactory.Abort();
            }
        }
    }
}
