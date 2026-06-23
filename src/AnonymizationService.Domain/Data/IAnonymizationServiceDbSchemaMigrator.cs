using System.Threading.Tasks;

namespace AnonymizationService.Data;

public interface IAnonymizationServiceDbSchemaMigrator
{
    Task MigrateAsync();
}
