using Porini.Abp.StateMachineEngine.Jobs;

namespace AnonymizationService.Jobs.ValidatePatient
{
    public class ValidatePatientJobArgs : JobArgs
    {
        public string MasterPatientIndex { get; set; }
    }
}
