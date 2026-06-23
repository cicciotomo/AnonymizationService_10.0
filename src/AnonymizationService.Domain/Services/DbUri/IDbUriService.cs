using System.Threading.Tasks;

namespace AnonymizationService.Services.DbUri
{
	public interface IDbUriService
	{
		Task<DbUriPatientData> GetPatientDataByMasterPatientIndexAsync(string masterPatientIndex);
		Task<DbUriPatientData> GetPatientDataByTaxCodeAsync(string taxCode);
	}
}
