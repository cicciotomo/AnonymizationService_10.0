using AnonymizationService.IntensiveCareData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.IntensiveCare
{
    public interface IIntensiveCareService
    {
        public Task<List<IntensiveCareEncounter>> GetEncountersAsync(string terapiaIntensivaPatientID);

        public Task<List<IntensiveCarePatientStringAttribute>> GetPatientsFromNosologici(IEnumerable<string> nosologicalCodes);

    }
}
