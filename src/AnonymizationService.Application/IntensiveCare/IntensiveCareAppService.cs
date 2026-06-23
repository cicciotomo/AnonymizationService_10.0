using AnonymizationService.Permissions;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AnonymizationService.IntensiveCareData;


namespace AnonymizationService.IntensiveCare
{
    [Authorize(AnonymizationServicePermissions.PatientManagementPermission)]
    public class IntensiveCareAppService : AnonymizationServiceAppService
    {
        private IIntensiveCareRepository _intensiveCareRepository;

        public IntensiveCareAppService(IIntensiveCareRepository intensiveCareRepository )
        {
            _intensiveCareRepository = intensiveCareRepository;
        }

        [AllowAnonymous]
        public async Task<List<IntensiveCarePatientStringAttributeDto>> GetPatientsFromNosologici(List<string> nosologici)
        {
            var listResult = await _intensiveCareRepository.GetPatientsFromNosologici(nosologici);
            return ObjectMapper.Map<List<IntensiveCarePatientStringAttribute>, List<IntensiveCarePatientStringAttributeDto>>(listResult);
        }

        [AllowAnonymous]
        public async Task<List<IntensiveCareEncounterDto>> GetTerapiaIntensivaEncounters(string terapiaIntensivaPatientID)
        {
            var listResult = await _intensiveCareRepository.GetTerapiaIntensivaEncounters(terapiaIntensivaPatientID);
            return ObjectMapper.Map<List<IntensiveCareEncounter>, List<IntensiveCareEncounterDto>>(listResult);
        }
    }
}
