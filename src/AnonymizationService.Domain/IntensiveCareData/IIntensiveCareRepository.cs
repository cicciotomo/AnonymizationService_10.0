using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.IntensiveCareData
{
    public interface IIntensiveCareRepository : IRepository<IntensiveCareEncounter>
    {
        Task<List<IntensiveCareEncounter>> GetTerapiaIntensivaEncounters(string terapiaIntensivaPatientID);
        Task<List<IntensiveCarePatientAdminState>> GetPatientAdminStates(string PatientId);
        Task<List<IntensiveCarePatientStringAttribute>> GetPatientsFromNosologici(IEnumerable<string> nosologici);
        Task<List<IntensiveCarePatientData>> GetPatientDatasFromCPI(string cpi);
    }
}
