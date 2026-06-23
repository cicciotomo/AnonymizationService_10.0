using AnonymizationService.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AnonymizationService.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AnonymizationServiceController : AbpControllerBase
{
    protected AnonymizationServiceController()
    {
        LocalizationResource = typeof(AnonymizationServiceResource);
    }
}
