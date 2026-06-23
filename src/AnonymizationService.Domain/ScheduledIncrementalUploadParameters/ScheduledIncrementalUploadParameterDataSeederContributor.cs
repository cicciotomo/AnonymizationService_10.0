using AnonymizationService.Enums;
using AnonymizationService.ScheduledUploadParameters;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.ScheduledIncrementalUploadParameters
{
    public class ScheduledIncrementalUploadParameterDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IScheduledIncrementalUploadParameterRepository _scheduledIncrementalUploadParameterRepository;
        public ScheduledIncrementalUploadParameterDataSeederContributor(IScheduledIncrementalUploadParameterRepository scheduledIncrementalUploadParameterRepository)
        {
            _scheduledIncrementalUploadParameterRepository = scheduledIncrementalUploadParameterRepository;
        }
        public async Task SeedAsync(DataSeedContext context)
        {
            var dbScheduledIncrementalUploadParameter = await _scheduledIncrementalUploadParameterRepository.GetListAsync();

            if (!dbScheduledIncrementalUploadParameter.Exists(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.UploadEnabled))
            {
                await _scheduledIncrementalUploadParameterRepository.InsertAsync(
                   new ScheduledIncrementalUploadParameter(
                       parameterKey: ScheduledIncrementalUploadParameterEnum.UploadEnabled,
                       value: "true",
                       dataType: DataTypeEnum.Boolean
                   ),
                   autoSave: true
                );
            }

            if (!dbScheduledIncrementalUploadParameter.Exists(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.UploadIntervalInDays))
            {
                await _scheduledIncrementalUploadParameterRepository.InsertAsync(
                   new ScheduledIncrementalUploadParameter(
                       parameterKey: ScheduledIncrementalUploadParameterEnum.UploadIntervalInDays,
                       value: "30",
                       dataType: DataTypeEnum.Int
                   ),
                   autoSave: true
                );
            }

            if (!dbScheduledIncrementalUploadParameter.Exists(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.PatientUploadRequestNumber))
            {
                await _scheduledIncrementalUploadParameterRepository.InsertAsync(
                   new ScheduledIncrementalUploadParameter(
                       parameterKey: ScheduledIncrementalUploadParameterEnum.PatientUploadRequestNumber,
                       value: "50",
                       dataType: DataTypeEnum.Int
                   ),
                   autoSave: true
                );
            }

            if (!dbScheduledIncrementalUploadParameter.Exists(s => s.ParameterKey == ScheduledIncrementalUploadParameterEnum.ExecutionTime))
            {
                await _scheduledIncrementalUploadParameterRepository.InsertAsync(
                   new ScheduledIncrementalUploadParameter(
                       parameterKey: ScheduledIncrementalUploadParameterEnum.ExecutionTime,
                       value: "23:00",
                       dataType: DataTypeEnum.Time
                   ),
                   autoSave: true
                );
            }
        }
    }
}
