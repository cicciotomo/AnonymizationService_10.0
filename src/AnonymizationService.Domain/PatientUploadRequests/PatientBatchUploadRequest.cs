using AnonymizationService.Enums;
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace AnonymizationService.PatientUploadRequests
{
    public class PatientBatchUploadRequest : CreationAuditedAggregateRoot<Guid>
    {
        private PatientBatchUploadRequest() { } // ORM purposes

        public PatientBatchUploadRequest(Guid id, Guid creatorId)
        {
            Id = id;
            Status = PatientBatchUploadStatus.Pending;
            CreatorId = creatorId;
        }

        public PatientBatchUploadStatus Status { get; protected set; }

        public bool TrySetRunning()
        {
            if (Status == PatientBatchUploadStatus.Pending)
            {
                Status = PatientBatchUploadStatus.Running;
                return true;
            }

            return false;
        }

        public bool TrySetCompleted()
        {
            if (Status == PatientBatchUploadStatus.Running)
            {
                Status = PatientBatchUploadStatus.Completed;
                return true;
            }

            return false;
        }
    }
}