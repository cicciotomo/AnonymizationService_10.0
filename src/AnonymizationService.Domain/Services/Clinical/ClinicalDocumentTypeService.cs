using AnonymizationService.ClinicalDocuments;
using Porini.Abp.StateMachineEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.Services.Clinical
{
    [ExposeServices(typeof(IClinicalDocumentTypeService))]
    public class ClinicalDocumentTypeService : IClinicalDocumentTypeService, ITransientDependency
    {
        private readonly IClinicalDocumentTypeRepository _clinicalDocumentTypeRepository;

        public ClinicalDocumentTypeService(
            IClinicalDocumentTypeRepository clinicalDocumentTypeRepository
            )
        {
            this._clinicalDocumentTypeRepository = clinicalDocumentTypeRepository;
        }

        public Task<List<ClinicalDocumentType>> GetDocumentTypesAsync()
        {
            return _clinicalDocumentTypeRepository.GetListAsync();
        }



    }
}
