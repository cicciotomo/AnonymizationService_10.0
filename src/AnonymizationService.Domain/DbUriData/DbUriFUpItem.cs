using DemographicWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.DbUriData
{
	public class DbUriFUpItem : Entity<long>
	{
		public DbUriFUpItem(long id,Guid cloudPatientId) : base(id)
		{
			CloudPatientId = cloudPatientId;
		}
		public Guid CloudPatientId { get; protected set; }
		public DateTime? CloudUploadDate { get; protected set; }
		public void SetCloudUploadDate(DateTime uploadDate)
		{
			CloudUploadDate = uploadDate;
		}
	}
}
