using AnonymizationService.Services.DbUri;
using AnonymizationService.Services.FhirService.Adapters.DbUri;
using AnonymizationService.Services.Pathox;
using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace AnonymizationService.Services.FhirService.Adapters.Pathox
{
	public interface IPathoxFhirDataAdapter
	{
		List<Resource> Transform(PathoxExamResult examResult, PathoxTransformationAdditionalInfo pathoxTransformationAdditionalInfo);
	}
}
