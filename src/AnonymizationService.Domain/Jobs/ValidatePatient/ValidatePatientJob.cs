using AnonymizationService.Services.Galileo;
using AnonymizationService.StateMachines.LoadPatientData;
using Porini.Abp.StateMachineEngine.Jobs;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AnonymizationService.Jobs.ValidatePatient
{
    public class ValidatePatientJob : Job<ValidatePatientJobArgs, JobResult>
    {
        private readonly IHospitalMasterPatientDataService _hospitalMasterPatientDataService;
        private readonly ILogger<ValidatePatientJob> _logger;

        public ValidatePatientJob(IHospitalMasterPatientDataService hospitalMasterPatientDataService, ILogger<ValidatePatientJob> logger)
        {
            _hospitalMasterPatientDataService = hospitalMasterPatientDataService;
            _logger = logger;
        }

        public override async Task<JobResult> ExecuteJobAsync(ValidatePatientJobArgs args)
        {
            _logger.LogInformation("Validating MPI {id}", args.MasterPatientIndex);

            var patientValidationSucceeded = await _hospitalMasterPatientDataService.ValidateMasterPatientIndexGenderAndBirthDateAsync(args.MasterPatientIndex);

            if (patientValidationSucceeded.Validated)
            {
                _logger.LogInformation("MPI {id} valid", args.MasterPatientIndex);

                var patientTaxCode = await _hospitalMasterPatientDataService.GetTaxCodeByMasterPatientIndexAsync(args.MasterPatientIndex);

				return new JobResult()

                {
                    StateMachineId = args.StateMachineId,
                    JobResultEvent = new PatientValidationSucceededEvent(patientTaxCode, patientValidationSucceeded.gender, patientValidationSucceeded.BirthDate)
                };
            }
            
            _logger.LogInformation("MPI {id} invalid", args.MasterPatientIndex);
            return new JobResult()
            {
                StateMachineId = args.StateMachineId,
                JobResultEvent = new PatientValidationFailedEvent()
            };
        }
    }
}
