using AnonymizationService.Services.Dicom;
using AnonymizationService.StateMachines.LoadPatientData;
using Porini.Abp.StateMachineEngine.Jobs;
using System;

namespace AnonymizationService.Jobs.RetrieveDicomData
{
    internal class RetrieveDicomDataArgs : JobArgs
    {
        public Guid CloudPatientId { get; set; }
        public string MasterPatientIndex { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string[] Modalities { get; set; }
        public string PacsSource { get; set; }
        public DicomParametersJson DicomParametersJson { get; set; }
    }
}
