using AnonymizationService.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace AnonymizationService;

[DependsOn(
    typeof(AnonymizationServiceEntityFrameworkCoreTestModule)
    )]
public class AnonymizationServiceDomainTestModule : AbpModule
{

}
