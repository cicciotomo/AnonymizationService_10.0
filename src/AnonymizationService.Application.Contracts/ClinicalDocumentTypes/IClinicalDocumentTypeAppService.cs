using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.ClinicalDocumentTypes
{
    public interface IClinicalDocumentTypeAppService
    {
        Task<List<ClinicalDocumentTypeDto>> GetAvailableClinicalDocumentTypeAsync();

        Task<List<ClinicalDocumentTypeDto>> GetAvailableDistinctClinicalDocumentTypeAsync();
    }
}
