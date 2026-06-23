using AnonymizationService.Enums;
using System;

namespace AnonymizationService.DefaultImportParameters
{
    public class DefaultImportParameterDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DefaultImportParameterDataTypeAssociationEnum DataTypeAssociation { get; set; }
        public DataTypeEnum DataType { get; set; }
        public string Value { get; set; }
        public string AppConfigurationKey { get; set; }
    }
}
