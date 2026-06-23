using AnonymizationService.Enums;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.DefaultImportParameters
{
    public class DefaultImportParameterDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<DefaultImportParameter, Guid> _defaultImportParameterRepository;

        public DefaultImportParameterDataSeederContributor(IRepository<DefaultImportParameter, Guid> defaultImportParameterRepository)
        {
            _defaultImportParameterRepository = defaultImportParameterRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _defaultImportParameterRepository.GetCountAsync() <= 0)
            {
                await _defaultImportParameterRepository.InsertAsync(
                    new DefaultImportParameter(
                    name: "ImportStartDate",
                    dataTypeAssociation: DefaultImportParameterDataTypeAssociationEnum.All,
                    dataType: DataTypeEnum.DateTime,
                    value: "2022-01-26"
                    ),
                    autoSave: true
                );
                await _defaultImportParameterRepository.InsertAsync(
                    new DefaultImportParameter(
                    name: "Modalities",
                    dataTypeAssociation: DefaultImportParameterDataTypeAssociationEnum.Dicom,
                    dataType: DataTypeEnum.List,
                    value: "CT,PT",
                    appConfigurationKey: "DefaultImportParameterKeys:Modalities"
                    ),
                    autoSave: true
                );
                await _defaultImportParameterRepository.InsertAsync(
                   new DefaultImportParameter(
                   name: "DocumentType",
                   dataTypeAssociation: DefaultImportParameterDataTypeAssociationEnum.Clinical,
                   dataType: DataTypeEnum.List,
                   value: "Dimissioni",
                   appConfigurationKey: "DefaultImportParameterKeys:DocumentTypes"
                   ),
                   autoSave: true
               );
                await _defaultImportParameterRepository.InsertAsync(
                   new DefaultImportParameter(
                   name: "Generate Log",
                   dataTypeAssociation: DefaultImportParameterDataTypeAssociationEnum.Clinical,
                   dataType: DataTypeEnum.Boolean,
                   value: "true"
                   ),
                   autoSave: true
               );
            }
        }
    }
}

