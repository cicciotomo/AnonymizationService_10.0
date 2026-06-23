using AnonymizationService.Enums;
using AnonymizationService.PatientUploadRequests;
using AnonymizationService.ScheduledIncrementalUploadParameters;
using AnonymizationService.Services.PatientLoadManager;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.UploadIncrementalDataManager
{
    public class UploadIncrementalDataManager : IUploadIncrementalDataManager, ISingletonDependency
    {
        private readonly IScheduledIncrementalUploadParameterRepository _scheduledIncrementalUploadParameterRepository;

        private readonly IPatientLoadManager _patientLoadManager;
        private readonly IPatientUploadRequestRepository _patientUploadRequestRepository;

        private TimeSpan ExecutionTime { get; set; }
        private int UploadIntervalInDays { get; set; }
        private int PatientUploadRequestNumber { get; set; }
        private DateTime StartDate { get; set; }
        private Boolean UpdateEnabled { get; set; }


        public UploadIncrementalDataManager(
            IScheduledIncrementalUploadParameterRepository scheduledIncrementalUploadParameterRepository,
            IPatientLoadManager patientLoadManager,
            IPatientUploadRequestRepository patientUploadRequestRepository)
        {
            _scheduledIncrementalUploadParameterRepository = scheduledIncrementalUploadParameterRepository;
            _patientLoadManager = patientLoadManager;
            _patientUploadRequestRepository = patientUploadRequestRepository;
        }

        public async Task Initialize()
        {
            var scheduledIncrementalUploadParameters = await _scheduledIncrementalUploadParameterRepository.GetListAsync();

            ExecutionTime = TimeSpan.Parse(scheduledIncrementalUploadParameters.Where(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.ExecutionTime).FirstOrDefault().Value);
            UploadIntervalInDays = int.Parse(scheduledIncrementalUploadParameters.Where(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.UploadIntervalInDays).FirstOrDefault().Value);
            PatientUploadRequestNumber = int.Parse(scheduledIncrementalUploadParameters.Where(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.PatientUploadRequestNumber).FirstOrDefault().Value);
            UpdateEnabled = bool.Parse(scheduledIncrementalUploadParameters.Where(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.UploadEnabled).FirstOrDefault().Value);
        }

        public async Task UploadIncrementalDataAsync()
        {
            if (!CanStart())
            {
                return;
            }

            StartDate = DateTime.Now;

            var lastUploadRequestForEachParientList = await _patientUploadRequestRepository.GetLastRequestForEachPatientListAsync();
            var patientUploadRequestsToUpload = lastUploadRequestForEachParientList.Where(r => r.CreationTime < DateTime.Now.AddDays(-UploadIntervalInDays)).OrderBy(r => r.CreationTime).Take(PatientUploadRequestNumber);

            foreach (var patientUploadRequest in patientUploadRequestsToUpload)
            {
                await _patientLoadManager.ReloadPatientUploadRequestAsync(patientUploadRequest);
            }
        }

        private bool CanStart()
        {
            if (UpdateEnabled && DateTime.Now.TimeOfDay >= ExecutionTime && StartDate.Date != DateTime.Today)
            { return true; }
            else
            { return false; }
        }
    }
}
