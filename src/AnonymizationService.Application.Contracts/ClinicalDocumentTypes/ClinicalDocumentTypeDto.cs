using System;
using Volo.Abp.Application.Dtos;

namespace AnonymizationService.ClinicalDocumentTypes
{
    public class ClinicalDocumentTypeDto : EntityDto<int>
    {
        public string GalileoCode { get; protected set; }
        public string LoincDescription { get; protected set; }
        public string LoincCode { get; protected set; }
        public int Enabled { get; protected set; }
    }
}
