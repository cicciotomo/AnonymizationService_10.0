using System.Threading.Tasks;
using static AnonymizationService.Services.Galileo.GalileoService;

namespace AnonymizationService.Services.Galileo
{
    public interface IHospitalMasterPatientDataService
    {
		Task<string> GetTaxCodeByMasterPatientIndexAsync(string masterPatientIndex);
		public Task<bool> ValidateMasterPatientIndexAsync(string masterPatientIndex);
        public Task<ValidatedPatient> ValidateMasterPatientIndexGenderAndBirthDateAsync(string masterPatientIndex);

    }
}
