using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AnonymizationService.Localization;
using AnonymizationService.StateMachines.Clinical;
using AnonymizationService.StateMachines.Dicom;
using AnonymizationService.StateMachines.Laboratory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace AnonymizationService.Patients
{
    public class CreatePatientDto : IValidatableObject
    {
        public string MasterPatientIndex { get; set; }
        public List<string> StudyIds { get; set; }
        public ClinicalParametersDto ClinicalParameters { get; set; }
        public DicomParametersDto DicomParameters { get; set; }
        public LaboratoryParametersDto LaboratoryParameters { get; set; }
        public DbUriParametersDto DbUriParameters { get; set; }
		public PathoxParametersDto PathoxParameters { get; set; }
        public IntensiveCareParametersDto IntensiveCareParameters { get; set; }
        public RedcapParametersDto RedcapParameters { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var stringLocalizer = validationContext.GetService<IStringLocalizer<AnonymizationServiceApplicationContractsResource>>();

            if (ClinicalParameters?.ShouldStart != true 
                && LaboratoryParameters?.ShouldStart != true 
                && DicomParameters?.ShouldStart != true 
                && DbUriParameters?.ShouldStart != true
				&& PathoxParameters?.ShouldStart != true
                && IntensiveCareParameters?.ShouldStart != true
                && RedcapParameters?.ShouldStart != true)
            {
                yield return new ValidationResult(stringLocalizer[AnonymizationServiceApplicationContractsResource.CreatePatientInputParametersValidationMessage]);
            }
        }
    }
}
