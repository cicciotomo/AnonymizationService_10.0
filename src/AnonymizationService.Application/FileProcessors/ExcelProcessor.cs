using AnonymizationService.Localization;
using Ganss.Excel;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Validation;

namespace AnonymizationService.FileProcessors
{
    public class ExcelProcessor : IExcelProcessor, ITransientDependency
    {
        private readonly ExcelMapper _excelMapper;
        private readonly IStringLocalizer<AnonymizationServiceResource> _stringLocalizer;
        private readonly IObjectValidator _objectValidator;

        public ExcelProcessor(IStringLocalizer<AnonymizationServiceResource> stringLocalizer, IObjectValidator objectValidator)
        {
            _excelMapper = new ExcelMapper();
            _stringLocalizer = stringLocalizer;
            _objectValidator = objectValidator;
        }

        public async Task<IEnumerable<T>> MapFromExcelFileAsync<T>(Stream fileStream)
        {
            IEnumerable<T> items = new List<T>();

            try
            {
                items = await _excelMapper.FetchAsync<T>(fileStream);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }

            if (items is null || !items.Any())
            {
                throw new UserFriendlyException(_stringLocalizer[AnonymizationServiceResource.UnparsableFileExceptionMessage]);
            }

            var errorsDictionary = new Dictionary<int, string>();

            int i = 0;

            foreach (var patient in items)
            {
                var validationError = await _objectValidator.GetErrorsAsync(patient);

                if (validationError?.Count > 0)
                {
                    errorsDictionary.Add(i, string.Join(';', validationError.Select(v => v.ErrorMessage)));
                }

                i++;
            }

            if (errorsDictionary?.Any() == true)
            {
                throw new UserFriendlyException(
                    string.Join(" | "
                    , errorsDictionary.Select(entry => _stringLocalizer[AnonymizationServiceApplicationContractsResource.CreatePatientBatchRowErrorValidationMessage, entry.Key + 1, entry.Value])
                    ));
            }

            return items;
        }
    }
}
