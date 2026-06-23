using AnonymizationService.ContainerImages;
using AnonymizationService.Containers.ContainerHosts;
using AnonymizationService.Localization;
using AnonymizationService.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.Containers
{
    [Authorize(AnonymizationServicePermissions.ContainerImageManagementPermission)]
    public class ContainerImageAppService : AnonymizationServiceAppService
    {
        private readonly IContainerImageRepository _containerImageRepository;
        private readonly IContainerStore _containerStore;
        private readonly ContainerHost _containerHost;

		public ContainerImageAppService(IContainerImageRepository containerImageRepository, IContainerStore containerStore, IServiceProvider serviceProvider)
        {
            _containerImageRepository = containerImageRepository;
            _containerStore = containerStore;
			_containerHost = serviceProvider.GetService<ContainerHost>();
		}

        public async Task<PagedResultDto<ContainerImageDto>> GetListAsync(PagedAndSortedResultRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(ContainerImage.ShortName);
            }

            var containerImages = await _containerImageRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting
            );

            var totalCount = await _containerImageRepository.GetCountAsync();

            return new PagedResultDto<ContainerImageDto>(
                totalCount,
                ObjectMapper.Map<List<ContainerImage>, List<ContainerImageDto>>(containerImages)
            );
        }

        public async Task<ContainerImageDto> CreateAsync(UpsertContainerImageDto createContainerDto)
        {
            var containerEntity = ObjectMapper.Map<UpsertContainerImageDto, ContainerImage>(createContainerDto);

            if (await _containerImageRepository.FindAsync(x => x.ShortName == containerEntity.ShortName) is not null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.CreateContainerImageAlreadyExistingShortNameValidationMessage]);
            }

            var createdEntity = await _containerImageRepository.InsertAsync(containerEntity);
            return ObjectMapper.Map<ContainerImage, ContainerImageDto>(createdEntity);
        }

        public async Task<ContainerImageDto> UpdateAsync(Guid id, UpsertContainerImageDto containerDto)
        {
            var containerEntity = await _containerImageRepository.FindAsync(id, includeDetails: true);

            if (containerEntity is null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.ContainerNotExistingValidationMessage]);
            }

            ObjectMapper.Map<UpsertContainerImageDto, ContainerImage>(containerDto, containerEntity);

            if (await _containerImageRepository.FindAsync(x => x.ShortName == containerEntity.ShortName && x.Id != id) is not null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.CreateContainerImageAlreadyExistingShortNameValidationMessage]);
            }

            var updatedEntity = await _containerImageRepository.UpdateAsync(containerEntity, autoSave: true);

            var runningContainers = _containerStore.GetRunningContainersByImageName(containerEntity.Name);

            foreach (var runningContainer in runningContainers)
            {
				await _containerHost.StopContainerAsync(runningContainer.ContainerId);
                _containerStore.RemoveById(runningContainer.ContainerId);
			}

			return ObjectMapper.Map<ContainerImage, ContainerImageDto>(updatedEntity);
        }

        public async Task DeleteAsync(Guid containerImageId)
        {
            var containerImage = await _containerImageRepository.FindAsync(containerImageId);
            if (containerImage is null)
            {
                throw new UserFriendlyException(L[AnonymizationServiceResource.ContainerNotExistingValidationMessage]);
            }

            await _containerImageRepository.DeleteAsync(containerImageId);
        }
    }
}
