using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.LaboratoryExams
{
    public class LaboratoryCodeMappingService : ILaboratoryCodeMappingService, ITransientDependency
    {
        private Dictionary<(string SourceCode, string DestinationSystem), string> _codeMapping { get; set; }
        private IRepository<LaboratoryCodeMap> _mapRepository { get; }

        private const string DEFAULT_SOURCE_SYSTEM = "https://www.hsr.it";
        private const string DEFAULT_DESTINATION_SYSTEM = "";

        public LaboratoryCodeMappingService(IRepository<LaboratoryCodeMap> mapRepository)
        {
            _mapRepository = mapRepository;
        }

        public (string System, string Code) MapLaboratoryCode(string source, string destinationSystem = "")
        {
            destinationSystem ??= DEFAULT_DESTINATION_SYSTEM;

            if (_codeMapping.ContainsKey((source, destinationSystem)))
            {
                return (destinationSystem, _codeMapping[(source, destinationSystem)]);
            }

            return (DEFAULT_SOURCE_SYSTEM, source);
        }

        public async Task InitializeMapAsync()
        {
            if (_codeMapping is null)
            {
                var maps = await _mapRepository.GetListAsync();
                _codeMapping = maps.ToDictionary(
                    m => (m.SourceCode, m.DestinationSystem)
                    , m => m.DestinationCode);
            }
        }
    }
}
