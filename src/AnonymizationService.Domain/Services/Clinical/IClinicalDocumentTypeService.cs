using AnonymizationService.ClinicalDocuments;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.Clinical
{
    public interface IClinicalDocumentTypeService
    {
        public Task<List<ClinicalDocumentType>> GetDocumentTypesAsync();

    }
}
