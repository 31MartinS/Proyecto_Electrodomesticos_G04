using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace ClienteEscritorio.Services
{
    public class BancoSoapService
    {
        public static string ServiceUrl { get; set; } = "http://localhost:5000/BancoService.asmx";

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
                Console.WriteLine($"[ClienteEscritorio] Enviando cédula: '{request.Cedula}'");

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

                var request = new GetAmortizationScheduleRequest
                {
                    IdCredito = idCredito
                };

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

        public static async Task<CheckCreditEligibilityResponse> CheckCreditEligibilityAsync(CheckCreditEligibilityRequest request)
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
                var response = await Task.Run(() => client.CheckCreditEligibility(request));

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

        public static async Task<CreateCreditResponse> CreateCreditAsync(CreateCreditRequest request)
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
                var response = await Task.Run(() => client.CreateCredit(request));

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
