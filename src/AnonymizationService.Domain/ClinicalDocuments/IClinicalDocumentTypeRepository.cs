using AnonymizationService.PodDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.ClinicalDocuments
{
    public interface IClinicalDocumentTypeRepository: IRepository<ClinicalDocumentType>
    {
        Task<List<ClinicalDocumentType>> GetListAsync();

    }
}

