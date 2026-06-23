using AnonymizationService.Enums;
using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.DefaultImportParameters
{
    public class DefaultImportParameter : Entity<Guid>
    {
        protected DefaultImportParameter() { }

        public DefaultImportParameter(
            string name,
            DefaultImportParameterDataTypeAssociationEnum dataTypeAssociation,
            DataTypeEnum dataType,
            string value
            )
        {
            Name = name;
            DataTypeAssociation = dataTypeAssociation;
            DataType = dataType;
            Value = value;
        }

        public DefaultImportParameter(
            string name,
            DefaultImportParameterDataTypeAssociationEnum dataTypeAssociation,
            DataTypeEnum dataType,
            string value,
            string appConfigurationKey
            )
        {
            Name = name;
            DataTypeAssociation = dataTypeAssociation;
            DataType = dataType;
            Value = value;
            AppConfigurationKey = appConfigurationKey;
        }

        public string Name { get; set; }
        public DefaultImportParameterDataTypeAssociationEnum DataTypeAssociation { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string Value { get; set; }
        public string AppConfigurationKey { get; set; }
    }
}
