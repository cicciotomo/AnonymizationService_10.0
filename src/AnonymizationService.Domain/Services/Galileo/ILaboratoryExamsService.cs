using AnonymizationService.Services.LaboratoryExamService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Galileo
{
    public interface ILaboratoryExamsService
    {
        public Task<List<LabResult>> GetPatientLaboratoryExamResultsAsync(string masterPatientIndex, DateTime startDate);

        public Task<List<LabResult>> GetPatientLaboratoryExamResultsFromDateToDateAsync(string masterPatientIndex, DateTime startDate, DateTime? endDate);
    }
}
