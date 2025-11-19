using System.ServiceModel;
using BancoSoapService.Services.Contracts;

namespace BancoSoapService.Services
{
    // IMPORTANTE: que coincida con los clientes
    [ServiceContract(
        Namespace = "http://tempuri.org/",
        Name = "IBancoService"
    )]
    public interface IBancoService
    {
        [OperationContract]
        EvaluateCreditResponse EvaluateCredit(EvaluateCreditRequest request);

        [OperationContract]
        GetAmortizationScheduleResponse GetAmortizationSchedule(GetAmortizationScheduleRequest request);

        [OperationContract]
        GetClientInfoResponse GetClientInfo(GetClientInfoRequest request);

        [OperationContract]
        CheckCreditEligibilityResponse CheckCreditEligibility(CheckCreditEligibilityRequest request);

        [OperationContract]
        CreateCreditResponse CreateCredit(CreateCreditRequest request);
    }
}
