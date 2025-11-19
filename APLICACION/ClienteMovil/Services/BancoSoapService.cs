using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace ClienteMovil.Services
{
    public class BancoSoapService
    {
        // CAMBIAR ESTA IP A LA IP DEL SERVIDOR
        public static string ServiceUrl { get; set; } = "http://10.40.20.89:5000/BancoService.asmx";

        public static async Task<EvaluateCreditResponse> EvaluateCreditAsync(EvaluateCreditRequest request)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                MaxBufferSize = 2147483647,
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10)
            };

            var endpoint = new EndpointAddress(ServiceUrl);
            var channelFactory = new ChannelFactory<IBancoService>(binding, endpoint);

            try
            {
                var client = channelFactory.CreateChannel();
                var response = await Task.Run(() => client.EvaluateCredit(request));

                if (client is IClientChannel channel)
                {
                    if (channel.State != CommunicationState.Faulted)
                        channel.Close();
                    else
                        channel.Abort();
                }

                channelFactory.Close();
                return response;
            }
            catch
            {
                channelFactory.Abort();
                throw;
            }
        }

        public static async Task<GetAmortizationScheduleResponse> GetAmortizationScheduleAsync(int idCredito)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                MaxBufferSize = 2147483647
            };

            var endpoint = new EndpointAddress(ServiceUrl);
            var channelFactory = new ChannelFactory<IBancoService>(binding, endpoint);

            try
            {
                var client = channelFactory.CreateChannel();
                var request = new GetAmortizationScheduleRequest { IdCredito = idCredito };
                var response = await Task.Run(() => client.GetAmortizationSchedule(request));

                if (client is IClientChannel channel)
                {
                    if (channel.State != CommunicationState.Faulted)
                        channel.Close();
                    else
                        channel.Abort();
                }

                channelFactory.Close();
                return response;
            }
            catch
            {
                channelFactory.Abort();
                throw;
            }
        }

        public static async Task<GetClientInfoResponse> GetClientInfoAsync(GetClientInfoRequest request)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 2147483647,
                MaxBufferSize = 2147483647,
                OpenTimeout = TimeSpan.FromMinutes(1),
                CloseTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(10),
                ReceiveTimeout = TimeSpan.FromMinutes(10)
            };

            var endpoint = new EndpointAddress(ServiceUrl);
            var channelFactory = new ChannelFactory<IBancoService>(binding, endpoint);

            try
            {
                var client = channelFactory.CreateChannel();
                var response = await Task.Run(() => client.GetClientInfo(request));

                if (client is IClientChannel channel)
                {
                    if (channel.State != CommunicationState.Faulted)
                        channel.Close();
                    else
                        channel.Abort();
                }

                channelFactory.Close();
                return response;
            }
            catch
            {
                channelFactory.Abort();
                throw;
            }
        }
    }
}
