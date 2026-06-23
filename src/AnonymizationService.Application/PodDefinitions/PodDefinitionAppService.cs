using AnonymizationService.ContainerImages;
using AnonymizationService.Localization;
using AnonymizationService.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.PodDefinitions
{
    [Authorize(AnonymizationServicePermissions.PodDefinitionManagementPermission)]

    public class PodDefinitionAppService : AnonymizationServiceAppService
    {
        private IPodDefinitionRepository _podDefinitionRepository;
        private IContainerImageRepository _containerImageRepository;

        public PodDefinitionAppService(IPodDefinitionRepository podDefinitionRepository, IContainerImageRepository containerImageRepository)
        {
            _podDefinitionRepository = podDefinitionRepository;
            _containerImageRepository = containerImageRepository;
        }

        public async Task<PagedResultDto<PodDefinitionDto>> GetListAsync(PagedAndSortedResultRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(PodDefinition.Name);
            }

            var podDefinitions = await _podDefinitionRepository.GetListAsync(
            requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting
            );

            var totalCount = await _podDefinitionRepository.GetCountAsync();

            return new PagedResultDto<PodDefinitionDto>(
                totalCount,
                ObjectMapper.Map<List<PodDefinition>, List<PodDefinitionDto>>(podDefinitions)
            );
        }

        public async Task<PodDefinitionDto> CreateAsync(UpsertPodDefinitionDto createPodDefinitionDto)
        {
            var podDefinitionEntity = ObjectMapper.Map<UpsertPodDefinitionDto, PodDefinition>(createPodDefinitionDto);

            if (await _podDefinitionRepository.FindAsync(x => x.Name == podDefinitionEntity.Name) is not null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.CreatePodDefinitionAlreadyExistingNameValidationMessage]);
            }

            foreach (var container in podDefinitionEntity.ContainerList)
            {
                if (await _containerImageRepository.SingleOrDefaultAsync(c => c.ShortName == container.ShortName) == null)
                {
                    throw new UserFriendlyException(
                        L[AnonymizationServiceResource.InvalidContainerForPodDefinitionValidationMessage,
                            container.ShortName]
                        );
                }
            }

            var createdEntity = await _podDefinitionRepository.InsertAsync(podDefinitionEntity);
            return ObjectMapper.Map<PodDefinition, PodDefinitionDto>(createdEntity);
        }

        public async Task<PodDefinitionDto> UpdateAsync(Guid id, UpsertPodDefinitionDto podDefinitionDto)
        {
            var podDefinitionEntity = await _podDefinitionRepository.FindAsync(id, includeDetails: true);

            if (podDefinitionEntity is null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.PodDefinitionNotExistingValidationMessage]);
            }

            ObjectMapper.Map<UpsertPodDefinitionDto, PodDefinition>(podDefinitionDto, podDefinitionEntity);

            if (await _podDefinitionRepository.FindAsync(x => x.Name == podDefinitionEntity.Name && x.Id != id) is not null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.CreatePodDefinitionAlreadyExistingNameValidationMessage]);
            }

            var updatedEntity = await _podDefinitionRepository.UpdateAsync(podDefinitionEntity, autoSave: true);
            return ObjectMapper.Map<PodDefinition, PodDefinitionDto>(updatedEntity);
        }

        public async Task DeleteAsync(Guid podDefinitionId)
        {
            var containerImage = await _podDefinitionRepository.FindAsync(podDefinitionId);
            if (containerImage is null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.PodDefinitionNotExistingValidationMessage]);
            }

            await _podDefinitionRepository.DeleteAsync(podDefinitionId);
        }
    }
}
