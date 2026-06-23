using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AnonymizationService;

[Dependency(ReplaceServices = true)]
public class AnonymizationServiceBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "AnonymizationService";
}
