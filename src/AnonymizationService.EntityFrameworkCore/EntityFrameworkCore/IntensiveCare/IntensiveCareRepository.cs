using AnonymizationService.IntensiveCareData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace AnonymizationService.EntityFrameworkCore.IntensiveCare
{
    public class IntensiveCareRepository : EfCoreRepository<IntensiveCareDbContext, IntensiveCareEncounter>, IIntensiveCareRepository
    {
        private readonly IntensiveCareDbContext _intensiveCareDbcontext;
        private readonly AnonymizationServiceDbContext _anonymizationServiceDbcontext;

        public IntensiveCareRepository(IDbContextProvider<IntensiveCareDbContext> dbContextProvider
            , IntensiveCareDbContext intensiveCareDbcontext
            , AnonymizationServiceDbContext anonymizationServiceDbcontext) : base(dbContextProvider)
        {
            _intensiveCareDbcontext = intensiveCareDbcontext;
            _anonymizationServiceDbcontext = anonymizationServiceDbcontext;
        }

        public async Task<List<IntensiveCarePatientStringAttribute>> GetPatientsFromNosologici(IEnumerable<string> nosologici)
        {
            var sql = "SELECT DISTINCT [PatientId],[Timestamp],'EncounterId' as [Name],[Value] FROM [_Export].[PatientStringAttribute_]  where name in ( 'EncounterId', 'LifetimeId') and Value = @p0";
            var results = new List<IntensiveCarePatientStringAttribute>();
            foreach (var item in nosologici)
            {
                _intensiveCareDbcontext.PatientStringAttributes.FromSqlRaw(sql, item);
                results.AddRange(_intensiveCareDbcontext.PatientStringAttributes.FromSqlRaw(sql, item));
            }
            return results;
        }

        public async Task<List<IntensiveCarePatientAdminState>> GetPatientAdminStates(string PatientId)
        {
            var sql = "SELECT [Id], [Timestamp], [AdmitState] FROM [Philips.PatientData].[_Export].[Patient_] WHERE [AdmitState] IN (0, 1, 2) and Id= @p0";
            var adminStates = _intensiveCareDbcontext.PatientAdminStates.FromSqlRaw(sql, PatientId).ToList();

            foreach (var state in adminStates)
            {
                Console.WriteLine($"Id: {state.Id}, Timestamp: {state.Timestamp}, AdmitState: {state.AdmitState}");
            }
            return adminStates;
        }

        public async Task<List<IntensiveCareEncounter>> GetTerapiaIntensivaEncounters(string terapiaIntensivaPatientID)
        {
            // Ottieni gli stati di ammissione del paziente
            List<IntensiveCarePatientAdminState> adminStates = await GetPatientAdminStates(terapiaIntensivaPatientID);
            // Converti lo stato di ammissione
            adminStates.ForEach(p => {
                if (p.AdmitState == 0)
                {
                    p.AdmitState = 1;
                }
            });

            var changePoints = adminStates
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Timestamp)
                .Select((p, index) => new TIPatientAdminStateWithFlag
                {
                    Id = p.Id,
                    Timestamp = p.Timestamp,
                    AdmitState = p.AdmitState,
                    //ChangeFlag = (index == 0 || adminStates[index - 1].AdmitState != p.AdmitState) ? 1 : 0
                    ChangeFlag = (index == 0 || adminStates[index - 1].AdmitState != adminStates[index].AdmitState) ? 1 : 0
                })
                .ToList();

            var changes = changePoints
               .Where(p => p.ChangeFlag == 1)
               .GroupBy(p => new { p.Id, p.AdmitState })
               .SelectMany(g => g.OrderBy(p => p.Timestamp)
                                 .Select((p, index) => new TIPatientAdminStateWithRanking
                                 {
                                     Id = p.Id,
                                     Timestamp = p.Timestamp,
                                     AdmitState = p.AdmitState,
                                     ChangeFlag = p.ChangeFlag,
                                     Ranking = index + 1
                                 }))
               .ToList();
            var startDates = changes
                .Where(p => p.AdmitState == 1)
                .Select(p => new
                {
                    p.Id,
                    p.Timestamp,
                    p.Ranking
                })
                .ToList();

            var endDates = changes
                .Where(p => p.AdmitState == 2)
                .Select(p => new
                {
                    p.Id,
                    p.Timestamp,
                    p.Ranking
                })
                .ToList();

            // Crea una lista per memorizzare gli incontri
            var result = (from start in startDates
                          join end in endDates
                          on new { start.Id, start.Ranking } equals new { end.Id, end.Ranking } into joined
                          from end in joined.DefaultIfEmpty()
                          orderby start.Id, start.Timestamp
                          select new IntensiveCareEncounter(start.Id, start.Timestamp.DateTime, end?.Timestamp.DateTime)
                          ).ToList();
            return result;
        }

        public async Task<List<IntensiveCarePatientData>> GetPatientDatasFromCPI(string cpi)
        {
            var x = _anonymizationServiceDbcontext.IntensiveCarePatientDatas;
            
            return await x.ToListAsync();
        }

        private class TIPatientAdminStateWithFlag : IntensiveCarePatientAdminState
        {
            public int ChangeFlag { get; set; }
        }
        private class TIPatientAdminStateWithRanking : TIPatientAdminStateWithFlag
        {
            public int Ranking { get; set; }
        }
    }
}
