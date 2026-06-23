using AnonymizationService.IntensiveCareData;
using FellowOakDicom.Log;
using Microsoft.Extensions.DependencyInjection;
using Porini.Abp.StateMachineEngine;
using Porini.Abp.StateMachineEngine.Events;
using Porini.Abp.StateMachineEngine.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.StateMachines.IntensiveCare.States
{
    public class UpdateIntensiveCareDataState : State
    {
        private readonly List<IntensiveCarePatientData> _patientData;

        public UpdateIntensiveCareDataState(List<IntensiveCarePatientData> patientData) { 
            _patientData = patientData;
        }
        protected override async Task RunAsync(IServiceProvider serviceProvider, StateMachine stateMachine)
        {
            if (stateMachine is not IntensiveCareStateMachine intensiveCareStateMachine)
            {
                throw new Exception(string.Format("[{0}] Invalid state machine for UpdateIntensiveCareDataState", stateMachine.Id));
            }

            var intensiveCareRepository = serviceProvider.GetService<IRepository<IntensiveCarePatientData>>();
            var patientDataList = await intensiveCareRepository.GetListAsync();

            foreach (var patientData in _patientData)
            {
                if (patientData.CloudUploadDate.HasValue)
                {
                    var itemToUpdate = patientDataList.Where(p => 
                        p.CloudPatientId == intensiveCareStateMachine.ContextData.CloudPatientId
                        && intensiveCareStateMachine.ContextData.NosologicalCodes.Contains(p.NosologicalCode)
                        && p.ExamStartDate == patientData.ExamStartDate
                        && p.ExamEndDate == patientData.ExamEndDate
                    ).ToList();
                    if (itemToUpdate.Count == 1)
                    {
                        var item = itemToUpdate.FirstOrDefault();
                        item.CloudUploadDate = patientData.CloudUploadDate.Value;
                        await intensiveCareRepository.UpdateAsync(item);
                    }
                    else if (itemToUpdate.Count > 1)
                    {
                        foreach (var item in itemToUpdate) {
                            item.CloudUploadDate = patientData.CloudUploadDate.Value;
                            await intensiveCareRepository.UpdateAsync(item);
                        }
                    }
                }
            }
            
            await PublishEventOnBusAsync(new ExecutedTaskEvent(stateMachine.Id, new IntensiveCareDataCheckUpdateCompletedEvent()));
        }
    }
}
