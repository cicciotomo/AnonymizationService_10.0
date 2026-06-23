using AnonymizationService.ClinicalDocuments;
using AnonymizationService.ContainerImages;
using AnonymizationService.Localization;
using AnonymizationService.Permissions;
using AnonymizationService.PodDefinitions;
using AnonymizationService.Services.Galileo;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace AnonymizationService.ClinicalDocumentTypes
{
    [Authorize(AnonymizationServicePermissions.PodDefinitionManagementPermission)]

    public class ClinicalDocumentTypeAppService : AnonymizationServiceAppService, IClinicalDocumentTypeAppService
    {
        private IClinicalDocumentTypeRepository _clinicalDocumentTypeRepository;
        
        public ClinicalDocumentTypeAppService(IClinicalDocumentTypeRepository clinicalDocumentTypeRepository)
        {
            _clinicalDocumentTypeRepository = clinicalDocumentTypeRepository;
        }

        public async Task<List<ClinicalDocumentTypeDto>> GetAvailableClinicalDocumentTypeAsync()
        {

            var clinicalDocuments = await _clinicalDocumentTypeRepository.GetListAsync();

            var loincDescriptions = clinicalDocuments.Select(x => x.LoincDescription).Distinct();

            clinicalDocuments = clinicalDocuments.Where(x => loincDescriptions.Contains(x.LoincDescription)).Where(x => x.Enabled==1).ToList();

            return ObjectMapper.Map<List<ClinicalDocumentType>, List<ClinicalDocumentTypeDto>>(clinicalDocuments);
        }

        public async Task<List<ClinicalDocumentTypeDto>> GetAvailableDistinctClinicalDocumentTypeAsync()
        {
            var clinicalDocuments = await _clinicalDocumentTypeRepository.GetListAsync();

            var loincDescriptions = clinicalDocuments.Where(x => x.Enabled == 1).Select(x => x.LoincDescription).Distinct();

            List<ClinicalDocumentType> retval = new List<ClinicalDocumentType>();

            foreach (var loincDescription in loincDescriptions) {
                retval.Add(clinicalDocuments.Where(x => x.LoincDescription == loincDescription).FirstOrDefault());
            }

            return ObjectMapper.Map<List<ClinicalDocumentType>, List<ClinicalDocumentTypeDto>>(retval);
        }
    }
}
