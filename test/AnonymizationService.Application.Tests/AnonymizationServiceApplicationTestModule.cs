using Volo.Abp.Modularity;

namespace AnonymizationService;

[DependsOn(
    typeof(AnonymizationServiceApplicationModule),
    typeof(AnonymizationServiceDomainTestModule)
    )]
public class AnonymizationServiceApplicationTestModule : AbpModule
{

}
