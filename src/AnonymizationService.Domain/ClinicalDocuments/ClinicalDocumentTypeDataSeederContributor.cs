using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.ClinicalDocuments
{
    public class ClinicalDocumentTypeDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IClinicalDocumentTypeRepository _clinicalDocumentTypeRepository;

        public ClinicalDocumentTypeDataSeederContributor(IClinicalDocumentTypeRepository clinicalDocumentTypeRepository)
        {
            this._clinicalDocumentTypeRepository = clinicalDocumentTypeRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var list = new List<ClinicalDocumentType>
            {
                new ClinicalDocumentType("Referto di Radiologia", "Referto di Radiologia", "18782-3"),
                new ClinicalDocumentType("refertoAmb.RefertoAmb_versII", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("Ref. Ambulatoriale", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmbPediatria.RefertoAmb_Pediatria", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmbVulnologia.RefAmbVulnologia", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmbPancreas.RefAmbPancreas", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmb_endo.RefertoAmb_endo_versII", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("Controllo_Urogine.Visita_controllo_uroginecologia", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("Urodinamica.RefertoUrodinamico", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmbTerapiaDolore.RefertoAmb_versII", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmbGinecologia.RefAmbGinecologia", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmb.RefertoAmb", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("VISITA ORTOPEDICA AMBULATORIALE", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("CHIRURGIA PROTESICA ROBOTICA", "", ""),
                new ClinicalDocumentType("refertoAmbVisDolPelvico.VisitaDolorePelvico", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmbVisDolPelvico.VisitaDoloreVulvare", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("refertoAmb_endo.RefertoAmb_Endo", "Referto Ambulatoriale", "34108-1"),
                new ClinicalDocumentType("Ref. Anatomia", "Referto di Anatomia Patologica", "60570-9"),
                new ClinicalDocumentType("AP", "Referto di Anatomia Patologica", "60570-9"),
                new ClinicalDocumentType("DischargeLetter_HSR.DNSAN035", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("DischargeLetter_HSR.DNSAN804", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("DischargeLetter_HSR.DNSAN820", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("DischargeLetter_HSR.DNSAN078", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("DischargeLetter_HSR.LDRIAB", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("DischargeLetter_HSR.DNSAN743", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("DischargeLetter_HSR.DNSAN819", "Lettera di Dimissione", "11490-0"),
                new ClinicalDocumentType("Verbale di PS", "Verbale di Pronto Soccorso", "28568-4"),
                new ClinicalDocumentType("CARELINE_SO.V_CHIR", "Visita Chirurgica", "34847-4"),
                new ClinicalDocumentType("Verbale Operatorio", "Verbale Operatorio", "11504-8"),
                new ClinicalDocumentType("Cartellino Anestesiologico", "Cartellino Anestesiologico", "11485-0"),
                new ClinicalDocumentType("Ref. di Endoscopia", "Referto di Endoscopia", "18751-8"),
                new ClinicalDocumentType("CARELINE_SO.V_ANEST", "Visita Anestesiologica", "24749-2"),
                new ClinicalDocumentType("Must.Must", "Referto Malnutrition Universal Screening Tool", "101789-6"),
                new ClinicalDocumentType("Referto", "Referto Elettromiagrafia", "18749-2"),
                new ClinicalDocumentType("Admission Summary", "Referto Radioterapia", "34832-6"),
                new ClinicalDocumentType("ProcAritmologia.ARITMO", "Referto Aritmologia", "11524-6"),
                new ClinicalDocumentType("storico cartelle AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storicom cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storic cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("cartella storico amb", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storico c artella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storico cartellan AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("swtorico cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("stoico cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("STORICO CARTELLA AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storico crtella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storico carella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storco cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storioc cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("torico cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storio cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("cartella cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("STORICO CATRTELLA AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("AMB CARTELLA STORICO", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("tstorico cartella AMB", "Referto Ambulatoriale Storico", ""),
                new ClinicalDocumentType("storico cartelle", "Referto Ambulatoriale Storico", "")
            };

            foreach (var clinicalDocumentType in list)
            {
                if (!await _clinicalDocumentTypeRepository
                        .AnyAsync(x => x.GalileoCode == clinicalDocumentType.GalileoCode && x.GalileoCode == clinicalDocumentType.GalileoCode)
                    )
                {
                    await _clinicalDocumentTypeRepository.InsertAsync(clinicalDocumentType);
                }
            }

        }
    }
}
