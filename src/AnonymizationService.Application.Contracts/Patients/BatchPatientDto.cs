using AnonymizationService.ClinicalDocumentTypes;
using AnonymizationService.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AnonymizationService.Patients
{
    public class BatchPatientDto : IValidatableObject
    {
        public string MasterPatientIndex { get; set; }
        public string StudyId { get; set; }

        public bool ClinicalStateMachineShouldStart { get; set; }
        public DateTime? ClinicalStateMachineStartTime { get; set; }


        public string ClinicalStateMachineDocumentType { get; set; }
        //public List<ClinicalDocumentTypeFilterDto> ClinicalStateMachineDocumentType { get; set; }

        public bool DicomStateMachineShouldStart { get; set; }
        public DateTime? DicomStateMachineStartTime { get; set; }
        public DateTime? DicomStateMachineEndTime { get; set; }
        public string DicomStateMachineModalities { get; set; }
        public string DicomPacsSource { get; set; }
        public string DicomParametersJson { get; set; }

        public bool LaboratoryStateMachineShouldStart { get; set; }
        public DateTime? LaboratoryStateMachineStartTime { get; set; }

		public bool DbUriStateMachineShouldStart { get; set; }
		public DateTime? DbUriStateMachineStartTime { get; set; }

		public bool PathoxStateMachineShouldStart { get; set; }
		public DateTime? PathoxStateMachineStartTime { get; set; }

        public bool IntensiveCareStateMachineShouldStart { get; set; }
        public DateTime? IntensiveCareStateMachineStartTime { get; set; }
        public string IntensiveCareStateMachineNosologicalCode { get; set; }

        public bool RedcapStateMachineShouldStart { get; set; }
        public DateTime? RedcapStateMachineStartTime { get; set; }
        public string RedcapStateMachineRedcapStudyConfigurationId { get; set; }
        public string RedcapStateMachineRecordId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var stringLocalizer = validationContext.GetService<IStringLocalizer<AnonymizationServiceApplicationContractsResource>>();

            if (!ClinicalStateMachineShouldStart 
                && !DicomStateMachineShouldStart 
                && !LaboratoryStateMachineShouldStart 
                && !DbUriStateMachineShouldStart
				&& !PathoxStateMachineShouldStart
                && !IntensiveCareStateMachineShouldStart
                && !RedcapStateMachineShouldStart)
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.CreatePatientInputParametersValidationMessage]);
            }
        }
    }
}
