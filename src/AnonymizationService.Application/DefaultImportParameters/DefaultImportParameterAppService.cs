using AnonymizationService.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.DefaultImportParameters
{
    [Authorize(AnonymizationServicePermissions.DefaultImportParameterManagementPermission)]
    public class DefaultImportParameterAppService : AnonymizationServiceAppService, IDefaultImportParameterAppService
    {
        private readonly IDefaultImportParameterRepository _defaultImportParameterRepository;
        private readonly IOptionsSnapshot<DefaultImportParameterKeys> _defaultImportParameterKeysOptions;
        
        public DefaultImportParameterAppService(IDefaultImportParameterRepository defaultImportParameterRepository, IOptionsSnapshot<DefaultImportParameterKeys> defaultImportParameterKeysOptions)
        {
            _defaultImportParameterRepository = defaultImportParameterRepository;
            _defaultImportParameterKeysOptions = defaultImportParameterKeysOptions;
        }

        public async Task<DefaultImportParameterDto> GetAsync(Guid id)
        {
            var existingParameter = await _defaultImportParameterRepository.GetAsync(id);
            return ObjectMapper.Map<DefaultImportParameter, DefaultImportParameterDto>(existingParameter);
        }

        public async Task<PagedResultDto<DefaultImportParameterDto>> GetListAsync(DefaultImportParameterListRequestDto requestDto)
        {
            if (requestDto.Sorting.IsNullOrWhiteSpace())
            {
                requestDto.Sorting = nameof(DefaultImportParameter.Name);
            }

            var availableDefaultImportParameterList = await _defaultImportParameterRepository.GetListAsync(
                requestDto.SkipCount,
                requestDto.MaxResultCount,
                requestDto.Sorting,
                requestDto.Filter
            );

            var totalCount = requestDto.Filter == null
                ? await _defaultImportParameterRepository.CountAsync()
                : await _defaultImportParameterRepository.CountAsync(
                    defaultImportParameter => defaultImportParameter.Name.Contains(requestDto.Filter));

            var defaultImportParameterListDto = ObjectMapper.Map<List<DefaultImportParameter>, List<DefaultImportParameterDto>>(availableDefaultImportParameterList);

            return new PagedResultDto<DefaultImportParameterDto>(totalCount, defaultImportParameterListDto);
        }

        public async Task<DefaultImportParameterDto> UpdateAsync(UpdateDefaultImportParameterDto updateDefaultImportParameterDto)
        {
            var parameter = await _defaultImportParameterRepository.GetAsync(parameter => parameter.Id == updateDefaultImportParameterDto.Id);
            if (CheckParameterValue(parameter, updateDefaultImportParameterDto.Value))
            {
                parameter.Value = updateDefaultImportParameterDto.Value;
                var updatedDefaultImportParameter = await _defaultImportParameterRepository.UpdateAsync(parameter);
                return ObjectMapper.Map<DefaultImportParameter, DefaultImportParameterDto>(updatedDefaultImportParameter);
            }
            else
            {
                throw new Exception();
            }
        }

        [AllowAnonymous]
        public async Task<List<string>> GetAvailableValuesAsync(Guid id)
        {
            var parameter = await _defaultImportParameterRepository.GetAsync(id);
            return GetAvailableValues(parameter.AppConfigurationKey);
        }

        #region private methods
        private bool CheckParameterValue(DefaultImportParameter parameter, string newValue)
        {
            CultureInfo enUS = new CultureInfo("en-US");

            switch (parameter.DataType)
            {
                case Enums.DataTypeEnum.DateTime:
                    if (!DateTime.TryParseExact(newValue, "yyyy-MM-dd", enUS, System.Globalization.DateTimeStyles.None, out var dateTime))
                    {
                        return false;
                    }

                    break;
                case Enums.DataTypeEnum.Int:
                    if (!Int32.TryParse(newValue, out var intValue))
                    {
                        return false;
                    }

                    break;
                case Enums.DataTypeEnum.List:
                    return CheckConfigurationListsConsistency(parameter, newValue);
            }
            return true;
        }

        private bool CheckConfigurationListsConsistency(DefaultImportParameter parameter, string newValue)
        {
            var availableValues = GetAvailableValues(parameter.AppConfigurationKey);

            var newValues = newValue.Split(',').ToList();
            foreach (var value in newValues)
            {
                if (!availableValues.Contains(value))
                {
                    return false;
                }
            }
            return true;
        }

        private List<string> GetAvailableValues(string appConfigurationKey)
        {
            switch (appConfigurationKey)
            {
                case "DefaultImportParameterKeys:Modalities":
                    return _defaultImportParameterKeysOptions.Value.Modalities.Split(',').ToList();
                case "DefaultImportParameterKeys:DocumentTypes":
                    return _defaultImportParameterKeysOptions.Value.DocumentTypes.Split(',').ToList();
                default: return new List<string>();
            }
        }

        #endregion private methods

    }
}
