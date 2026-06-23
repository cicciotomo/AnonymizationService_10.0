using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Data;

/* This is used if database provider does't define
 * IAnonymizationServiceDbSchemaMigrator implementation.
 */
public class NullAnonymizationServiceDbSchemaMigrator : IAnonymizationServiceDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
