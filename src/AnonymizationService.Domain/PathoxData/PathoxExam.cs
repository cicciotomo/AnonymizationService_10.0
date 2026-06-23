using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.PathoxData
{
	public class PathoxExam: Entity<string>
	{
		public PathoxExam(string id, Guid cloudPatientId) : base(id)
		{
			CloudPatientId = cloudPatientId;
		}
		public Guid CloudPatientId { get; set; }
		public DateTime? CloudUploadDate { get; protected set; }
		public void SetCloudUploadDate(DateTime uploadDate)
		{
			CloudUploadDate = uploadDate;
		}
	}
}
