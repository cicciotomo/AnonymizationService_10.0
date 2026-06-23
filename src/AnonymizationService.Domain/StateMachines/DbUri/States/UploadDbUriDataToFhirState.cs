using AnonymizationService.DbUriData;
using AnonymizationService.Jobs.UpsertFhirDbUriData;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Jobs;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.DbUri.States
{
    internal class UploadDbUriDataToFhirState : State
    {
        public UploadDbUriDataToFhirState() { }

        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            var jobScheduler = serviceProvider.GetService<IJobScheduler>();

            if (stateMachine is not DbUriStateMachine dbUriStateMachine)
            {
                throw new Exception("Invalid state machine for the given state");
            }

            var dbUriFUpItemsRepository = serviceProvider.GetService<IRepository<DbUriFUpItem>>();

            var retrivedDbUriFUpItemIds = new List<long>();
            if (dbUriStateMachine.ContextData.DbUriPatientData.fup != null)
            {
                retrivedDbUriFUpItemIds = dbUriStateMachine.ContextData.DbUriPatientData.fup.items.Select(i => i.id).ToList();
            }


            var dbUriFUpItemsToUpload = await dbUriFUpItemsRepository.GetListAsync(e => retrivedDbUriFUpItemIds.Contains(e.Id) && e.CloudUploadDate == null);
            var dbUriFUpItemToUploadIds = dbUriFUpItemsToUpload.Select(i => i.Id).ToList();


            var dbUriEventsRepository = serviceProvider.GetService<IRepository<DbUriEvent>>();

            var retrivedDbUriEventIds = dbUriStateMachine.ContextData.DbUriPatientData.events.Select(i => i.generalInfo.id).ToList();

            var dbUriEventsToUpload = await dbUriEventsRepository.GetListAsync(e => retrivedDbUriEventIds.Contains(e.Id) && e.CloudUploadDate == null);
            var dbUriEventToUploadIds = dbUriEventsToUpload.Select(i => i.Id).ToList();

            var eventsToUpload = dbUriStateMachine.ContextData.DbUriPatientData.events?.Where(e => dbUriEventToUploadIds.Contains(e.generalInfo.id)).ToList();
            var fupsToUpload = dbUriStateMachine.ContextData.DbUriPatientData.fup?.items?.Where(e => dbUriFUpItemToUploadIds.Contains(e.id)).ToList();

            FupData fupData = null;
            if (dbUriStateMachine.ContextData.DbUriPatientData.fup != null)
            {
                 fupData = new FupData()
                {
                    id = dbUriStateMachine.ContextData.DbUriPatientData.fup.id,
                    idStructure = dbUriStateMachine.ContextData.DbUriPatientData.fup.idStructure,
                    examEmCreatininemia06 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia06,
                    examEmCreatininemia06Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia06Um,
                    examEmCreatininemia06Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia06Range,
                    examEmCreatininemia12 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia12,
                    examEmCreatininemia12Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia12Um,
                    examEmCreatininemia12Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia12Range,
                    examEmCreatininemia24 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia24,
                    examEmCreatininemia24Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia24Um,
                    examEmCreatininemia24Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia24Range,
                    examEmCreatininemia36 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia36,
                    examEmCreatininemia36Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia36Um,
                    examEmCreatininemia36Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia36Range,
                    examEmCreatininemia48 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia48,
                    examEmCreatininemia48Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia48Um,
                    examEmCreatininemia48Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia48Range,
                    examEmCreatininemia60 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia60,
                    examEmCreatininemia60Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia60Um,
                    examEmCreatininemia60Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia60Range,
                    examEmCreatininemia72 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia72,
                    examEmCreatininemia72Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia72Um,
                    examEmCreatininemia72Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia72Range,
                    examEmCreatininemia84 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia84,
                    examEmCreatininemia84Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia84Um,
                    examEmCreatininemia84Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia84Range,
                    examEmCreatininemia96 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia96,
                    examEmCreatininemia96Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia96Um,
                    examEmCreatininemia96Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia96Range,
                    examEmCreatininemia108 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia108,
                    examEmCreatininemia108Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia108Um,
                    examEmCreatininemia108Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia108Range,
                    examEmCreatininemia120 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia120,
                    examEmCreatininemia120Um = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia120Um,
                    examEmCreatininemia120Range = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmCreatininemia120Range,
                    examEmGFR12 = dbUriStateMachine.ContextData.DbUriPatientData.fup.examEmGFR12,
                    patientEnrolledProtocol = dbUriStateMachine.ContextData.DbUriPatientData.fup.patientEnrolledProtocol,
                    hasCardiovascularEvent = dbUriStateMachine.ContextData.DbUriPatientData.fup.hasCardiovascularEvent,
                    cardiovascularEventSurvival = dbUriStateMachine.ContextData.DbUriPatientData.fup.cardiovascularEventSurvival,
                    hasPrimaryTumor = dbUriStateMachine.ContextData.DbUriPatientData.fup.hasPrimaryTumor,
                    primaryTumorTimeTo = dbUriStateMachine.ContextData.DbUriPatientData.fup.primaryTumorTimeTo,
                    hasKidneyPrimaryTumor = dbUriStateMachine.ContextData.DbUriPatientData.fup.hasKidneyPrimaryTumor,
                    kidneyPrimaryTumorTimeTo = dbUriStateMachine.ContextData.DbUriPatientData.fup.kidneyPrimaryTumorTimeTo,
                    hasRelapse = dbUriStateMachine.ContextData.DbUriPatientData.fup.hasRelapse,
                    relapseSurvival = dbUriStateMachine.ContextData.DbUriPatientData.fup.relapseSurvival,
                    hasTherapyAdjuvant = dbUriStateMachine.ContextData.DbUriPatientData.fup.hasTherapyAdjuvant,
                    hasDiabetes = dbUriStateMachine.ContextData.DbUriPatientData.fup.hasDiabetes,
                    diabetesTimeTo = dbUriStateMachine.ContextData.DbUriPatientData.fup.diabetesTimeTo
                };
            }

            await jobScheduler.EnqueueJob<UpsertFhirDbUriDataJob, UpsertFhirDbUriDataArgs, JobResult>(new UpsertFhirDbUriDataArgs()
            {
                CloudPatientId = dbUriStateMachine.ContextData.CloudPatientId,
                StateMachineId = stateMachine.Id,
                FhirPatientId = dbUriStateMachine.ContextData.FhirPatientId,
                DbUriEventToUpload = eventsToUpload,
                DbUriFUpItemToUpload = fupsToUpload,
                FupData = fupData,
                Patient = dbUriStateMachine.ContextData.DbUriPatientData.patient
            });
        }
    }
}
