using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.RetrievePathoxData
{
	internal class RetrievePathoxDataArgs : JobArgs
	{
		public Guid CloudPatientId { get; set; }
		public string MasterPatientIndex { get; set; }
		public DateTime StartDate { get; set; }
	}
}
