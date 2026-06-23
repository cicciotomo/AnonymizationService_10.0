using System;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Pathox
{
	public interface IPathoxService
	{
		Task<PathoxExamResult> GetExamDetailByExamIdAsync(string examId);
		Task<PathoxListResponse> GetPatientDataByMasterPatientIndexAsync(string masterPatientIndex, DateTime startDate);
	}
}
