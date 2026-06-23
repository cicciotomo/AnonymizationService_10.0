using AnonymizationService.Enums;
using System.Threading.Tasks;

namespace AnonymizationService.Services.ThrottlingManager
{
    public interface IThrottlingManager
    {
        Task RetrieveExternalServiceRequestLimits();
        Task SleepThreadUntilNeededAsync(ExternalServiceRequestTypeEnum externalServiceRequestLimitEnum);
    }
}
