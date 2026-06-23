
using AnonymizationService.IntensiveCareData;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonymizationService.Jobs.UpsertFhirIntensiveCarePatientData
{
    internal class UpsertFhirIntensiveCareDataJobArgs : JobArgs
    {
        public Guid CloudPatientId { get; set; }
        public string FhirPatientId { get; set; }
        public List<IntensiveCarePatientData> PatientData { get; set; }
    }
}
