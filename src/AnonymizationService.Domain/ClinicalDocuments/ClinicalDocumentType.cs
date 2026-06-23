using System;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.ClinicalDocuments
{
    public class ClinicalDocumentType : Entity<int> 
    {
        public string GalileoCode { get; protected set; }
        public string LoincDescription { get; protected set; }
        public string LoincCode { get; protected set; }
        public int Enabled { get; protected set; }

        public ClinicalDocumentType(string galileoCode, string loincDescription, string loincCode)
        {
            GalileoCode = galileoCode;
            LoincDescription = loincDescription;
            LoincCode = loincCode;
            Enabled = 0;
        }
    }
}
