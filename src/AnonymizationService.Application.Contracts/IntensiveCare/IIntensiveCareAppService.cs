using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.IntensiveCare.Encounter
{
    public interface IIntensiveCareAppService
    {
        Task<List<IntensiveCarePatientStringAttributeDto>> GetPatientsFromNosologici(List<string> nosologici);
        Task<List<IntensiveCareEncounterDto>> GetTerapiaIntensivaEncounters(string terapiaIntensivaPatientID);
    }
}
