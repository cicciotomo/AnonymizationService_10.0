using AnonymizationService.Jobs.UpsertFhirDbUriData;
using AnonymizationService.Services.DbUri;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.DependencyInjection;
using Patient = AnonymizationService.Services.DbUri.Patient;

namespace AnonymizationService.Services.FhirService.Adapters.DbUri
{
    public class DbUriFhirDataAdapter : FhirDataAdapterBase, IDbUriFhirDataAdapter, ITransientDependency
    {
        public (string encounterIdentifier, List<Resource> resources) TransformPatientEncounter(Patient patient, DbUriTransformationAdditionalInfo dbUriTransformationAdditionalInfo)
        {
            var fhirResources = new List<Resource>();

            var fhirPatientId = dbUriTransformationAdditionalInfo.FhirPatientId;
            var cloudPatientId = dbUriTransformationAdditionalInfo.CloudPatientId;
            var encounterIdentifier = $"{cloudPatientId}-uri-patient-data";
            var encounterDate = DateTime.Now;
            var encounterDateString = encounterDate.ToString("yyyy-MM-ddThh:mm:sszzzz");
            var urologyOrganizationId = dbUriTransformationAdditionalInfo.UrologyOrganizationId;

            var encounter = new Encounter()
            {
                Subject = new ResourceReference()
                {
                    Type = "Patient",
                    Reference = $"Patients/{fhirPatientId}",
                    Identifier = new Identifier() { Value = cloudPatientId.ToString() }
                },
                Identifier = new List<Identifier>() { new Identifier() { Value = encounterIdentifier } },
                StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP"),
                Period = new Period() { Start = encounterDateString, End = encounterDateString }
            };

            var composition = new Composition()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
                Identifier = new Identifier() { Value = encounterIdentifier },
                Section = new List<Composition.SectionComponent>(),
                Title = "Urology Patient Info",
                Type = new CodeableConcept("http://loinc.org", "100542-0", "Urology Outpatient Progress note"),
                Status = CompositionStatus.Final,
                Date = encounterDateString,
                Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
                Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = urologyOrganizationId }, Type = "Organization" } }
            };

            var propertyNameToSkip = new List<string>()
            {
                nameof(patient.id),
                nameof(patient.dateOfBirth),
                nameof(patient.sex),
                nameof(patient.municipality),
            };

            var patientObservations = CreateObservationsFromProperties(patient, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate, propertyNameToSkip);

            if (patientObservations != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Urology Patient Data",
                    Entry = patientObservations.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(patientObservations);
                fhirResources.Add(encounter);
                fhirResources.Add(composition);
            }

            return (encounterIdentifier, fhirResources);
        }

        public (string encounterIdentifier, List<Resource> resources) TransformFupEncounter(FupData fupData, DbUriTransformationAdditionalInfo dbUriTransformationAdditionalInfo)
        {
            var fhirResources = new List<Resource>();

            var fhirPatientId = dbUriTransformationAdditionalInfo.FhirPatientId;
            var cloudPatientId = dbUriTransformationAdditionalInfo.CloudPatientId;
            var encounterIdentifier = $"{cloudPatientId}-uri-follow-up-data";
            var encounterDate = DateTime.Now;
            var encounterDateString = encounterDate.ToString("yyyy-MM-ddThh:mm:sszzzz");
            var urologyOrganizationId = dbUriTransformationAdditionalInfo.UrologyOrganizationId;

            var encounter = new Encounter()
            {
                Subject = new ResourceReference()
                {
                    Type = "Patient",
                    Reference = $"Patients/{fhirPatientId}",
                    Identifier = new Identifier() { Value = cloudPatientId.ToString() }
                },
                Identifier = new List<Identifier>() { new Identifier() { Value = encounterIdentifier } },
                StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP"),
				Period = new Period() { Start = encounterDateString, End = encounterDateString }
			};

            var composition = new Composition()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
                Identifier = new Identifier() { Value = encounterIdentifier },
                Section = new List<Composition.SectionComponent>(),
                Title = "Urology Follow Up Info",
                Type = new CodeableConcept("http://loinc.org", "100542-0", "Urology Outpatient Progress note"),
                Status = CompositionStatus.Final,
                Date = encounterDateString,
                Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
                Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = urologyOrganizationId }, Type = "Organization" } }
            };

            var fupObservations = CreateObservationsFromProperties(fupData, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate);

            if (fupObservations != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Urology Follow Up Data",
                    Entry = fupObservations.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(fupObservations);
                fhirResources.Add(encounter);
                fhirResources.Add(composition);
            }

            return (encounterIdentifier, fhirResources);
        }

        public List<Resource> Transform(List<Event> events, List<Item> fupItems, DbUriTransformationAdditionalInfo dbUriTransformationAdditionalInfo)
        {
            var fhirResources = new List<Resource>();

            foreach (var medicalEvent in events ?? new List<Event>())
            {
                var medicalEventResources = TransformMedicalEvent(medicalEvent, dbUriTransformationAdditionalInfo.CloudPatientId, dbUriTransformationAdditionalInfo.FhirPatientId, dbUriTransformationAdditionalInfo.UrologyOrganizationId);
                if (medicalEventResources != null)
                {
                    fhirResources.AddRange(medicalEventResources);
                }
            }

            foreach (var fupItem in fupItems ?? new List<Item>())
            {
                var fupItemResources = TransformFupItem(fupItem, dbUriTransformationAdditionalInfo.CloudPatientId, dbUriTransformationAdditionalInfo.FhirPatientId, dbUriTransformationAdditionalInfo.UrologyOrganizationId);
                if (fupItemResources != null)
                {
                    fhirResources.AddRange(fupItemResources);
                }
            }

            return fhirResources;
        }

        private List<Resource> TransformFupItem(Item fupItem, Guid cloudPatientId, string fhirPatientId, string urologyOrganizationId)
        {
            var fhirResources = new List<Resource>();

            var encounterIdentifier = fupItem.id.ToString();
            var fupDate = fupItem.fupDate;
            var fupDateString = fupDate != null ? fupDate?.ToString("yyyy-MM-ddThh:mm:sszzzz") : null;

            var encounter = new Encounter()
            {
                Subject = new ResourceReference()
                {
                    Type = "Patient",
                    Reference = $"Patients/{fhirPatientId}",
                    Identifier = new Identifier() { Value = cloudPatientId.ToString() }
                },
                Identifier = new List<Identifier>() { new Identifier() { Value = encounterIdentifier } },
                StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
                Period = new Period()
                {
                    Start = fupDateString,
                    End = fupDateString,
                },
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP")
            };

            var composition = new Composition()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
                Identifier = new Identifier() { Value = encounterIdentifier },
                Section = new List<Composition.SectionComponent>(),
                Title = "Urology Report",
                Type = new CodeableConcept("http://loinc.org", "100646-9", "Urology Hospital Progress note"),
                Status = CompositionStatus.Final,
                Date = fupDateString,
                Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
                Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = urologyOrganizationId }, Type = "Organization" } }
            };

            var progressResources = TransformProgress(fupItem.progress, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
            if (progressResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Progress",
                    Entry = progressResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(progressResources);
            }

            var renalRecurrenceResources = TransformRenalRecurrence(fupItem.renalRecurrence, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
            if (renalRecurrenceResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Renal Recurrence",
                    Entry = renalRecurrenceResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(renalRecurrenceResources);
            }

            var newKidneyPrimaryTumorResources = TransformNewKidneyPrimaryTumor(fupItem.newKidneyPrimaryTumor, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
            if (newKidneyPrimaryTumorResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "New Kidney Primary Tumor",
                    Entry = newKidneyPrimaryTumorResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(newKidneyPrimaryTumorResources);
            }

            var newPrimaryTumorResources = TransformNewPrimaryTumor(fupItem.newPrimaryTumor, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
            if (newPrimaryTumorResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "New Primary Tumor",
                    Entry = newPrimaryTumorResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(newPrimaryTumorResources);
            }

            var cardiovascularEventTumorResources = TransformCardiovascularEvent(fupItem.cardiovascularEvent, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
            if (cardiovascularEventTumorResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Cardiovascular Event",
                    Entry = cardiovascularEventTumorResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(cardiovascularEventTumorResources);
            }

            var generalFupItem = TransformGeneralFupItem(fupItem, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
            if (generalFupItem != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "General Follow Up Item",
                    Entry = generalFupItem.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(generalFupItem);
            }

            fhirResources.Add(encounter);
            fhirResources.Add(composition);

            return fhirResources;
        }

        private List<Observation> TransformGeneralFupItem(Item fupItem, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? fupDate)
        {
            var propertyNamesToSkip = new List<string>()
            {
                nameof(fupItem.progress),
                nameof(fupItem.renalRecurrence),
                nameof(fupItem.newKidneyPrimaryTumor),
                nameof(fupItem.newPrimaryTumor),
                nameof(fupItem.cardiovascularEvent)
            };
            return CreateObservationsFromProperties(fupItem, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate, propertyNamesToSkip);
        }

        private List<Observation> TransformCardiovascularEvent(Cardiovascularevent cardiovascularEvent, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? fupDate)
        {
            return CreateObservationsFromProperties(cardiovascularEvent, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
        }

        private List<Observation> TransformNewPrimaryTumor(Newprimarytumor newPrimaryTumor, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? fupDate)
        {
            return CreateObservationsFromProperties(newPrimaryTumor, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
        }

        private List<Observation> TransformNewKidneyPrimaryTumor(Newkidneyprimarytumor newKidneyPrimaryTumor, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? fupDate)
        {
            return CreateObservationsFromProperties(newKidneyPrimaryTumor, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
        }
        private List<Observation> TransformRenalRecurrence(Renalrecurrence renalRecurrence, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? fupDate)
        {
            return CreateObservationsFromProperties(renalRecurrence, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
        }
        private List<Observation> TransformProgress(Progress progress, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? fupDate)
        {
            return CreateObservationsFromProperties(progress, cloudPatientId, fhirPatientId, encounterIdentifier, fupDate);
        }

        private List<Resource> TransformMedicalEvent(Event medicalEvent, Guid cloudPatientId, string fhirPatientId, string urologyOrganizationId)
        {
            var fhirResources = new List<Resource>();

            var encounterIdentifier = medicalEvent.generalInfo.id.ToString();
            var examDate = medicalEvent.generalInfo.examinationDate;
            var examDateString = examDate.ToString("yyyy-MM-ddThh:mm:sszzzz");

            var encounter = new Encounter()
            {
                Subject = new ResourceReference()
                {
                    Type = "Patient",
                    Reference = $"Patients/{fhirPatientId}",
                    Identifier = new Identifier() { Value = cloudPatientId.ToString() }
                },
                Identifier = new List<Identifier>() { new Identifier() { Value = encounterIdentifier } },
                StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
                Period = new Period()
                {
                    Start = examDateString,
                    End = examDateString,
                },
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP")
            };

            var composition = new Composition()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
                Identifier = new Identifier() { Value = encounterIdentifier },
                Section = new List<Composition.SectionComponent>(),
                Title = "Urology Report",
                Type = new CodeableConcept("http://loinc.org", "100646-9", "Urology Hospital Progress note"),
                Status = CompositionStatus.Final,
                Date = examDateString,
                Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
                Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = urologyOrganizationId }, Type = "Organization" } }
            };

            var medicalHistoryResources = TransformMedicalHistory(medicalEvent.medicalHistory, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
            if (medicalHistoryResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Medical History",
                    Entry = medicalHistoryResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(medicalHistoryResources);
            }

            var histologicalResources = TransformHistological(medicalEvent.histological, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
            if (histologicalResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Histological",
                    Entry = histologicalResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(histologicalResources);
            }

            var hospitalstayResources = TransformHospitalstay(medicalEvent.hospitalStay, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
            if (hospitalstayResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Hospital Stay",
                    Entry = hospitalstayResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(hospitalstayResources);
            }

            var surgeryResources = TransformSurgery(medicalEvent.surgery, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
            if (surgeryResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Surgery",
                    Entry = surgeryResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(surgeryResources);
            }

            var questResources = TransformQuest(medicalEvent.quest, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
            if (questResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Quest",
                    Entry = questResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(questResources);
            }

            var tacResources = TransformTac(medicalEvent.tac, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
            if (tacResources != null)
            {
                composition.Section.Add(new Composition.SectionComponent()
                {
                    Title = "Tac",
                    Entry = tacResources.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
                });
                fhirResources.AddRange(tacResources);
            }

            fhirResources.Add(encounter);
            fhirResources.Add(composition);

            return fhirResources;
        }

        private List<Observation> TransformMedicalHistory(Medicalhistory medicalhistory, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime examDate)
            => CreateObservationsFromProperties(medicalhistory, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);

        private List<Observation> TransformHistological(Histological histological, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime examDate)
            => CreateObservationsFromProperties(histological, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);

        private List<Observation> TransformHospitalstay(Hospitalstay hospitalstay, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime examDate)
            => CreateObservationsFromProperties(hospitalstay, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);

        private List<Observation> TransformSurgery(Surgery surgery, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime examDate)
            => CreateObservationsFromProperties(surgery, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
        private List<Observation> TransformQuest(Quest quest, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime examDate)
            => CreateObservationsFromProperties(quest, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);

        private List<Observation> TransformTac(Tac tac, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime examDate)
            => CreateObservationsFromProperties(tac, cloudPatientId, fhirPatientId, encounterIdentifier, examDate);
      }
}
