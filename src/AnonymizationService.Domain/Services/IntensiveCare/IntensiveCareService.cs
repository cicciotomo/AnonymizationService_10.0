using AnonymizationService.IntensiveCareData;
using AnonymizationService.Services.Clinical;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.IntensiveCare
{
    [ExposeServices(typeof(IIntensiveCareService))]
    public class IntensiveCareService : IIntensiveCareService, ITransientDependency
    {
        private readonly IIntensiveCareRepository _intensiveCareRepository;

        public IntensiveCareService(IIntensiveCareRepository intensiveCareRepository)
        {
            _intensiveCareRepository = intensiveCareRepository;
        }
        
        public Task<List<IntensiveCareEncounter>> GetEncountersAsync(string nosologicalCode)
        {
            return _intensiveCareRepository.GetTerapiaIntensivaEncounters(nosologicalCode);
        }

        public Task<List<IntensiveCarePatientStringAttribute>> GetPatientsFromNosologici(IEnumerable<string> nosologicalCodes)
        {
            return _intensiveCareRepository.GetPatientsFromNosologici(nosologicalCodes);
        }
    }
}
