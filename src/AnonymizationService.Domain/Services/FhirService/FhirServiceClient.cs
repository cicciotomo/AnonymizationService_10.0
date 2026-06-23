using AnonymizationService.Jobs.UpsertFhirLaboratoryData;
using AnonymizationService.LaboratoryExams;
using AnonymizationService.Services.AzureIdentity;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Task = System.Threading.Tasks.Task;

namespace AnonymizationService.Services.FhirService
{
    public class FhirServiceClient : IFhirServiceClient, ITransientDependency
    {
        private readonly FhirServiceSettings _fhirSettings;
        private readonly ILogger<FhirServiceClient> _logger;
        private readonly IAzureTokenProvider _azureTokenProvider;
        private readonly ILaboratoryCodeMappingService _laboratoryCodeMappingService;
        private FhirClient _fhirClient;
        private const int MAX_NUMBER_OF_ENTRIES_PER_BUNDLE = 500;
        private const string FHIR_OBSERVATION_SEPARATOR = "|||||";
        private const string DEFAULT_CODE_SYSTEM_PROVIDER = "https://www.hsr.it";

        private readonly List<string> HANDLED_FHIR_RESOURCE_LIST = new()
        {
            nameof(Observation),
            nameof(MedicationStatement),
            nameof(Condition),
            nameof(Procedure),
            nameof(ImagingStudy),
            nameof(DocumentReference),
            nameof(Composition),
            nameof(Encounter),
        };

        public FhirServiceClient(IOptions<FhirServiceSettings> fhirSettings, ILogger<FhirServiceClient> logger, IAzureTokenProvider azureTokenProvider, ILaboratoryCodeMappingService laboratoryCodeMappingService)
        {
            _fhirSettings = fhirSettings.Value;
            _logger = logger;
            _azureTokenProvider = azureTokenProvider;
            _laboratoryCodeMappingService = laboratoryCodeMappingService;
        }

        public async Task<Encounter> GetEncounterByIdentifier(string encounterIdentifier)
        {
            await InitializeFhirClientAsync();

            var encounters = await _fhirClient.SearchAsync<Encounter>(new SearchParams().Where($"identifier={encounterIdentifier}"));

            if (encounters.Entry?.Count > 0)
            {
                return (Encounter)encounters.Entry.First().Resource;
            }

            return null;
        }

        public async Task CreateTransactionBundlesForResourcesAsync(List<Resource> resourcesToCreate)
        {
            if (resourcesToCreate == null || resourcesToCreate.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Initializing Fhir Client");

            await InitializeFhirClientAsync();

            _logger.LogInformation($"Sending transaction bundle for {resourcesToCreate.Count} resources");
            try
            {
                for (int i = 0; i < resourcesToCreate.Count(); i = i + MAX_NUMBER_OF_ENTRIES_PER_BUNDLE)
                {
                    var items = resourcesToCreate.Skip(i).Take(MAX_NUMBER_OF_ENTRIES_PER_BUNDLE).ToList();

                    var builder = new TransactionBuilder(_fhirSettings.FhirServiceUrl, Bundle.BundleType.Transaction);

                    foreach (var item in items)
                    {
                        builder = builder.Create(item);
                    }

                    var transaction = builder.ToBundle();

                    _logger.LogInformation("Sending transaction bundle for resources {i}-{count}", i, i + items.Count);

                    await _fhirClient.TransactionAsync(transaction);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message} - {ex.InnerException?.Message}");
            }

        }

        public async Task UpsertEncounterWithObservationsAsync(string fhirPatientId, List<FhirLaboratoryDataDto> laboratoryExams)
        {
            if (laboratoryExams is null || laboratoryExams.Count == 0)
            {
                return;
            }

            _logger.LogInformation("Initializing Fhir Client");

            await InitializeFhirClientAsync();
            await _laboratoryCodeMappingService.InitializeMapAsync();

            _logger.LogInformation("Creating Laboratory Analysis Organization");

            var laboratoryAnalysisDepartment = await this.UpsertHospitalDepartment("LAB-1 Laboratorio Analisi");

            _logger.LogInformation("Creating Fhir Encounters");

            var encounters = laboratoryExams.Select(exam => new Encounter()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = exam.CloudPatientId.ToString() } },
                Identifier = new List<Identifier>() { new() { Value = exam.Id } },
                Period = new Period()
                {
                    Start = exam.ExamStartDate.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
                    End = exam.ExamEndDate.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
                },
                StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB")
            });

            _logger.LogInformation("Creating Fhir Observations");

            var observations = laboratoryExams.SelectMany(e => e.ValueResults.Select(result =>
            {
                var mappedCode = _laboratoryCodeMappingService.MapLaboratoryCode(result.ExamType);
                var referenceRange = string.IsNullOrEmpty(result.ReferenceRange)
                    ? null
                    : new List<Observation.ReferenceRangeComponent>()
                    {
                        new()
                        {
                            Text = result.ReferenceRange
                        }
                    };

                return new Observation()
                {
                    Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = e.CloudPatientId.ToString() } },
                    Encounter = new ResourceReference() { Identifier = new Identifier() { Value = e.Id } },
                    Identifier = new List<Identifier>() { new() { Value = result.Id } },
                    Status = ObservationStatus.Final,
                    Value = new FhirString($"{result.Value}{FHIR_OBSERVATION_SEPARATOR}{result.Unit}{FHIR_OBSERVATION_SEPARATOR}{result.Comment}"),
                    ReferenceRange = referenceRange,
                    Issued = result.ResultDate,
                    Code = new CodeableConcept(mappedCode.System, mappedCode.Code, result.ExamTypeDescription, null),
                    Method = new CodeableConcept(DEFAULT_CODE_SYSTEM_PROVIDER, result.ExamMethod, result.ExamMethodDescription, null),
                };
            }));

            _logger.LogInformation("Creating Fhir Compositions");

            var compositions = laboratoryExams.Select(exam => new Composition()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = exam.CloudPatientId.ToString() } },
                Identifier = new Identifier() { Value = exam.Id },
                Section = new List<Composition.SectionComponent>()
                {
                    new Composition.SectionComponent()
                    {
                        Title = "Laboratory data",
                        Entry = exam.ValueResults.Select(e => new ResourceReference()
                        {
                            Type = "Observation",
                            Identifier = new Identifier(){Value=e.Id}
                        }).ToList()
                    }
                },
                Title = $"Laboratory exam {exam.Id}",
                Type = new CodeableConcept("http://loinc.org", "11502-2", "Laboratory report"),
                Status = CompositionStatus.Final,
                Date = exam.ExamEndDate.ToString("yyyy-MM-ddTHH:mm:sszzzz"),
                Encounter = new ResourceReference() { Identifier = new Identifier() { Value = exam.Id } },
                Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = laboratoryAnalysisDepartment.Identifier.FirstOrDefault().Value }, Type = "Organization" } }
            });

            for (var i = 0; i < encounters.Count(); i += MAX_NUMBER_OF_ENTRIES_PER_BUNDLE)
            {
                var items = encounters.Skip(i).Take(MAX_NUMBER_OF_ENTRIES_PER_BUNDLE);

                var builder = new TransactionBuilder(_fhirSettings.FhirServiceUrl, Bundle.BundleType.Transaction);

                foreach (var item in items)
                {
                    builder = builder.Update(new SearchParams().Where($"identifier=|{item.Identifier.FirstOrDefault().Value}"), item);
                }

                var transaction = builder.ToBundle();

                _logger.LogInformation("Sending transaction bundle for encounters {i}-{count}", i, i + items.Count());

                await _fhirClient.TransactionAsync(transaction);
            }

            for (var i = 0; i < observations.Count(); i += MAX_NUMBER_OF_ENTRIES_PER_BUNDLE)
            {
                var items = observations.Skip(i).Take(MAX_NUMBER_OF_ENTRIES_PER_BUNDLE);

                var builder = new TransactionBuilder(_fhirSettings.FhirServiceUrl, Bundle.BundleType.Transaction);

                foreach (var item in items)
                {
                    builder = builder.Update(new SearchParams().Where($"identifier=|{item.Identifier.FirstOrDefault().Value}"), item);
                }

                var transaction = builder.ToBundle();

                _logger.LogInformation("Sending transaction bundle for observations {i}-{count}", i, i + items.Count());

                await _fhirClient.TransactionAsync(transaction);
            }

            for (var i = 0; i < compositions.Count(); i += MAX_NUMBER_OF_ENTRIES_PER_BUNDLE)
            {
                var items = compositions.Skip(i).Take(MAX_NUMBER_OF_ENTRIES_PER_BUNDLE);

                var builder = new TransactionBuilder(_fhirSettings.FhirServiceUrl, Bundle.BundleType.Transaction);

                foreach (var item in items)
                {
                    builder = builder.Update(new SearchParams().Where($"identifier=|{item.Identifier.FirstOrDefault().Value}"), item);
                }

                var transaction = builder.ToBundle();

                _logger.LogInformation("Sending transaction bundle for compositions {i}-{count}", i, i + items.Count());

                await _fhirClient.TransactionAsync(transaction);
            }
        }

        public async Task<ResearchStudy> GetResearchStudyByIdAsync(string studyId)
        {
            _logger.LogInformation($"Retrieving research study {studyId}");

            await InitializeFhirClientAsync();

            var studySearchResponse = await _fhirClient.SearchByIdAsync<ResearchStudy>(studyId);

            if (studySearchResponse.Entry?.Count > 0)
            {
                return (ResearchStudy)studySearchResponse.Entry.First().Resource;
            }

            return null;
        }

        public async Task<ResearchSubject> UpsertResearchSubjectAsync(string fhirPatientIdentifier, string fhirPatientId, string fhirStudyId, string fhirStudyIdentifier)
        {
            _logger.LogInformation($"Upserting research subject for patient {fhirPatientId} and study {fhirStudyId}");

            await InitializeFhirClientAsync();

            var researchSubject = new ResearchSubject()
            {
                Individual = new ResourceReference()
                {
                    Reference = $"Patient/{fhirPatientId}",
                    Type = "Patient",
                    Identifier = new Identifier()
                    {
                        Value = fhirPatientIdentifier
                    }
                },
                Study = new ResourceReference()
                {
                    Reference = $"ResearchStudy/{fhirStudyId}",
                    Type = "ResearchStudy",
                    Identifier = new Identifier()
                    {
                        Value = fhirStudyIdentifier
                    }
                },
                Status = ResearchSubject.ResearchSubjectStatus.OnStudy
            };

            var createdSubject = await _fhirClient.UpdateAsync(researchSubject, new SearchParams()
                .Where($"study=ResearchStudy/{fhirStudyId}")
                .Where($"individual=Patient/{fhirPatientId}"));

            return createdSubject;
        }

        public async Task<Patient> UpsertPatientAsync(Guid cloudPatientId, AdministrativeGender gender, DateTime? birthDate)
        {
            _logger.LogInformation($"Upserting patient with cloudPatientId {cloudPatientId}");

            await InitializeFhirClientAsync();

            var organization = await GetHsrOrganizationAsync();

            var newPatient = new Patient
            {
                Active = true,
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        Value = $"{cloudPatientId}"
                    }
                },
                ManagingOrganization = new ResourceReference
                {
                    ElementId = organization.Id
                },
                Gender = gender,
                BirthDate = birthDate.HasValue ? birthDate.Value.ToString("yyyy-MM-dd") : null
            };

            var patient = await _fhirClient.UpdateAsync(newPatient, new SearchParams().Where($"identifier={cloudPatientId}"));

            _logger.LogInformation($"Upserted patient id {patient.Id}");

            return patient;
        }

        private async Task InitializeFhirClientAsync()
        {
            _logger.LogInformation($"Initialize FHIR client");
            if (_fhirClient != null)
            {
                return;
            }

            var token = await _azureTokenProvider.GetTokenForScopeAsync(_fhirSettings.FhirServiceUrl + "/.default");

            // Handler per l'autenticazione
            var authHandler = new AuthorizationMessageHandler();
            authHandler.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Handler personalizzato per aggiungere l'header
            var customHandler = new CustomHeaderHandler
            {
                InnerHandler = authHandler
            };

            var settings = FhirClientSettings.CreateDefault();
            settings.PreferredFormat = ResourceFormat.Json;

            _fhirClient = new FhirClient(
                new Uri(_fhirSettings.FhirServiceUrl),
                settings,
                customHandler
            );
        }

        private async Task<Organization> GetHsrOrganizationAsync()
        {
            _logger.LogInformation($"Get HSR organization");
            await InitializeFhirClientAsync();

            var newOrganization = new Organization
            {
                Identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        Value = _fhirSettings.FhirOrganizationIdentifier
                    }
                },
                Name = _fhirSettings.FhirRootOrganizationName
            };

            var organization = await _fhirClient.UpdateAsync(newOrganization, new SearchParams().Where($"identifier={_fhirSettings.FhirOrganizationIdentifier}"));

            return organization;
        }

        public async Task<Encounter> UpsertEncounter(DateTime startDate, DateTime endDate, string patientIdentifier, string patientId, string identifier)
        {
            _logger.LogInformation($"Upsert encounter");
            await InitializeFhirClientAsync();

            var encounterToCreate = new Encounter()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{patientId}", Identifier = new Identifier() { Value = patientIdentifier } },
                Identifier = new List<Identifier>() { new() { Value = identifier } },
                Period = new Period()
                {
                    Start = startDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    End = endDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                },
                StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
                Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "AMB")
            };

            var encounter = await _fhirClient.UpdateAsync(encounterToCreate, new SearchParams().Where($"identifier={identifier}"));
            return encounter;
        }

        public async Task<Organization> UpsertHospitalDepartment(string departmentName)
        {
            _logger.LogInformation($"Upsert hospital department");
            await InitializeFhirClientAsync();

            var departmentSearchResult =
                await _fhirClient.SearchAsync<Organization>(new SearchParams() { Count = 100 });

            var namedEntry = departmentSearchResult.Entry
                .Where(org => org.Resource.TypeName == "Organization")
                .FirstOrDefault(org => ((Organization)org.Resource).Name == departmentName);

            if (namedEntry != null)
            {
                return (Organization)namedEntry.Resource;
            }

            var newOrganization = new Organization
            {
                Identifier = new List<Identifier>
                {
                    new()
                    {
                        Value = Guid.NewGuid().ToString()
                    }
                },
                Name = departmentName,
                PartOf = new ResourceReference()
                {
                    Type = "Organization",
                    Identifier = new Identifier { Value = _fhirSettings.FhirOrganizationIdentifier }
                }
            };

            var organization = await _fhirClient.CreateAsync(newOrganization);

            return organization;
        }

        public async Task<DocumentReference> UpsertDocumentReference(DocumentReference doc)
        {
            _logger.LogInformation($"Upsert document reference");

            await InitializeFhirClientAsync();
            var docFound =
                await _fhirClient.SearchAsync<DocumentReference>(new SearchParams().Where($"identifier={doc.Identifier[0]}"));

            if (docFound.Entry.Count > 0)
            {
                return await _fhirClient.UpdateAsync(doc);
            }
            else
            {
                return await _fhirClient.CreateAsync(doc);
            }
        }

    }

    public class CustomHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Aggiungi l'header per la logica parallela
            request.Headers.Add("x-bundle-processing-logic", "parallel");

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
