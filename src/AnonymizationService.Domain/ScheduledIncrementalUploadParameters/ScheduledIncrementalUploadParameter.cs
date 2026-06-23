using AnonymizationService.Enums;
using System;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.ScheduledUploadParameters
{
    public class ScheduledIncrementalUploadParameter : Entity<Guid>
    {
        public ScheduledIncrementalUploadParameter(ScheduledIncrementalUploadParameterEnum parameterKey, string value, DataTypeEnum dataType)
        {
            ParameterKey = parameterKey;
            Value = value;
            DataType = dataType;
        }

        public ScheduledIncrementalUploadParameterEnum ParameterKey { get; set; }
        public string Value { get; set; }
        public DataTypeEnum DataType { get; set; }

    }
}
