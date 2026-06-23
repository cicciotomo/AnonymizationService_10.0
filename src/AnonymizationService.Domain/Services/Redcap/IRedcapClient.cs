using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AnonymizationService.Services.Redcap.RedcapClient;

namespace AnonymizationService.Services.Redcap
{
    public interface IRedcapClient
    {
        public  Task<RedcapReturnData> GetPatientDataByMPIAsync(string masterPatientIndex,
                                                           string mpiColumnName,
                                                           string redcapToken,
                                                           string endpoint,
                                                           string studyFields,
                                                           string cloudPatientIndex);
        public Task<RedcapReturnData> GetPatientDataByRecordIdAsync(string redcapToken,
                                                           string mpiColumnName, 
                                                           string endpoint,
                                                           string studyFields,
                                                           string cloudPatientIndex,
                                                           string record_id);
        public Task<string> GetStudyMetadataAsync(string redcapToken,
                                                  string endpoint);
        public Task<string> GetStudyDataAsync(string redcapToken,
                                              string endpoint,
                                              string studyFields,
                                              string requestFilter);

    }
}
