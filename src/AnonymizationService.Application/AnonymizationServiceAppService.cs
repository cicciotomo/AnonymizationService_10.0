using AnonymizationService.Localization;
using Volo.Abp.Application.Services;

namespace AnonymizationService;

/* Inherit your application services from this class.
 */
public abstract class AnonymizationServiceAppService : ApplicationService
{
    protected AnonymizationServiceAppService()
    {
        LocalizationResource = typeof(AnonymizationServiceResource);
    }
}
