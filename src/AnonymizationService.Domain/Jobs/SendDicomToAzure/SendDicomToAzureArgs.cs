using System;
using AnonymizationService.StateMachines.Dicom.States;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.SendDicomToAzure
{
    internal class SendDicomToAzureArgs : JobArgs
    {
        public Guid PatientId { get; set; }
        public List<SerieToSendParameter> SerieToSendParameters { get; set; }
    }
}
