using AnonymizationService.PatientUploadRequests;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnonymizationService.Services.PatientLoadManager
{
    public interface IPatientLoadManager
    {
        Task HandleStateMachineCompletionAsync(Guid stateMachineId, string errorMessage = null);
        Task LoadBatchPatientsAsync(IList<PatientUploadInput> patientUploadInputList, Guid userId);
        Task LoadNewPatientAsync(PatientUploadInput patientUploadInput, Guid userId);
        Task RetrieveRunningLoadPatientDataStateMachines();
        Task RetrievePendingLoadPatientDataStateMachines();
        Task ReloadPatientUploadRequestAsync(PatientUploadRequest patientUploadRequest);
    }
}