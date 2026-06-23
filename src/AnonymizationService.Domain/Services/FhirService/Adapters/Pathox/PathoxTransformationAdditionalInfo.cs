using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Services.FhirService.Adapters.Pathox
{
	public class PathoxTransformationAdditionalInfo
	{
		public Guid CloudPatientId { get; set; }
		public string FhirPatientId { get; set; }
		public string PathoxOrganizationId { get; set; }
	}
}
