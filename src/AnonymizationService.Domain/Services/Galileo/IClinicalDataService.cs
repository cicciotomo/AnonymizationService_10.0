using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Galileo
{
    public interface IClinicalDataService
    {
        public Task<List<PatientDocument>> GetPatientClinicalDataResultsAsync(string masterPatientIndex, DateTime startDate);
        public Task<PatientDocumentFile> GetDocumentAsync(int documentId);
    }
}