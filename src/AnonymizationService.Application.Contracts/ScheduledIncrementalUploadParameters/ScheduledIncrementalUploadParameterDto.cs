using AnonymizationService.Enums;
using System;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ScheduledIncrementalUploadParameters
{
    public class ScheduledIncrementalUploadParameterDto : EntityDto<Guid>
    {
        public ScheduledIncrementalUploadParameterEnum ParameterKey { get; set; }
        public string Value { get; set; }
        public DataTypeEnum DataType { get; set; }
    }
}
