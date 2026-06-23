using AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter;
using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.FhirResourcesFilterService
{
    public class FhirResourcesFilterService : IFhirResourcesFilterService, ITransientDependency
    {
        public List<Resource> FilterFhirResources(List<DocumentSectionExtractionResult> documentSectionsExtracted, FilterManipulationData filterManipulationData)
        {
            var filterResult = new List<Resource>();

            var encounterIdentifier = filterManipulationData.EncounterId.ToString();

            // Generate one composition per document with a custom Identifier

            var compositionIdentifier = Guid.NewGuid();

            var composition = new Composition()
            {
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{filterManipulationData.FhirPatientId}", Identifier = new Identifier() { Value = filterManipulationData.PatientIdentifier.ToString() } },
                Identifier = new Identifier() { Value = compositionIdentifier.ToString() },
                Section = new List<Composition.SectionComponent>(),
                Title = filterManipulationData.DocumentName,
                //Type = new CodeableConcept("http://loinc.org", "18842-5", "Discharge summary"),
                Type = new CodeableConcept("http://loinc.org", filterManipulationData.DocumentType.LoincCode, filterManipulationData.DocumentType.LoincDescription),
                Status = CompositionStatus.Final,
                Date = filterManipulationData.CreationDate.ToString("yyyy-MM-ddThh:mm:sszzzz"),
                Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = filterManipulationData.DepartmentIdentifier }, Type = "Organization" } },
                Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } }
            };

            foreach (var documentSection in documentSectionsExtracted)
            {
                var resourceBundleList = documentSection
                    .FhirResources
                    .Select(resource => FilterAndAdaptResource(
                        resource, 
                        filterManipulationData.FhirPatientId, 
                        filterManipulationData.PatientIdentifier.ToString(), 
                        encounterIdentifier, 
                        filterManipulationData.EncounterEndDate, 
                        filterManipulationData.CreationDate))
                    .Where(adaptedResource => adaptedResource != null)
                    .ToList();

                var compositionSection = new Composition.SectionComponent()
                {
                    Title = documentSection.SectionName,
                    Entry = new List<ResourceReference>()
                };

                var observations = resourceBundleList.Where(x => x.TryDeriveResourceType(out var rt) && rt == ResourceType.Observation).ToList();
                if (observations.Any())
                {
                    compositionSection.Entry = observations.Select(r => new ResourceReference()
                    {
                        Type = r.TypeName,
                        Identifier = new Identifier() { Value = ((Observation)r).Identifier.FirstOrDefault()?.Value },
                    }).ToList();
                }

                // For additional document data (like entities and relations)
                // we create a DocumentReference
                if (documentSection.Entities?.Any() == true || documentSection.Relations?.Any() == true)
                {
                    var documentReference = GenerateDocumentReference(documentSection, filterManipulationData.FhirPatientId, filterManipulationData.PatientIdentifier.ToString(), encounterIdentifier, filterManipulationData.EncounterEndDate);

                    compositionSection.Entry.Add(new ResourceReference()
                    {
                        Type = documentReference.TypeName,
                        Identifier = new Identifier() { Value = documentReference.Identifier.First().Value }
                    });

                    filterResult.Add(documentReference);
                }

                composition.Section.Add(compositionSection);

                filterResult.AddRange(resourceBundleList);
            }

            filterResult.Add(composition);

            return filterResult;
        }

        private DocumentReference GenerateDocumentReference(DocumentSectionExtractionResult documentSection, string patientId, string patientIdentifier, string encounterIdentifier, DateTime encounterDateTime)
        {
            var additionalData = new
            {
                documentSection.Entities,
                documentSection.Relations
            };

            var serializedData = JsonSerializer.Serialize(additionalData,
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var utf8SerializedDataBytes = System.Text.Encoding.UTF8.GetBytes(serializedData);
            var base64String = Convert.ToBase64String(utf8SerializedDataBytes);

            var documentReferenceIdentifier = Guid.NewGuid().ToString();

            var documentReference = new DocumentReference()
            {
                Identifier = new List<Identifier>()
                    { new() { Value = documentReferenceIdentifier } },
                Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{patientId}", Identifier = new Identifier() { Value = patientIdentifier } },
                Context = new DocumentReference.ContextComponent()
                {
                    Encounter = new List<ResourceReference>()
                    {
                        new() { Identifier = new Identifier() { Value = encounterIdentifier } }
                    }
                },
                Date = encounterDateTime,
                Status = DocumentReferenceStatus.Current,
                Content = new List<DocumentReference.ContentComponent>()
                {
                    new()
                    {
                        Attachment = new Attachment()
                        {
                            ContentType = "application/json",
                            Data = Convert.FromBase64String(base64String)
                        }
                    }
                }
            };
            return documentReference;
        }

        private Resource FilterAndAdaptResource(Resource resource, string patientId, string patientIdentifier, string encounterIdentifier, DateTime encounterDateTime, DateTime documentCreationDate)
        {
            if (resource.TryDeriveResourceType(out var resourceType))
            {
                switch (resourceType)
                {
                    case ResourceType.Observation:
                        var observation = (Observation)resource;
                        observation.Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{patientId}", Identifier = new Identifier() { Value = patientIdentifier } };
                        observation.Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } };
                        observation.Identifier = new List<Identifier>() { new Identifier() { Value = observation.Id } };
                        observation.Issued ??= encounterDateTime;
                        return observation;
                    case ResourceType.MedicationStatement:
                        var medicationStatement = (MedicationStatement)resource;
                        medicationStatement.Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{patientId}", Identifier = new Identifier() { Value = patientIdentifier } };
                        medicationStatement.Context = new ResourceReference
                        {
                            Reference = $"Encounter/{encounterIdentifier}",
                            Type = "Encouter",
                            Display = "encounterIdentifier",
                            Identifier = new Identifier() { Value = encounterIdentifier }
                        };
                        medicationStatement.Identifier = new List<Identifier>() { new Identifier() { Value = medicationStatement.Id } };
                        medicationStatement.DateAsserted = documentCreationDate.ToString("yyyy-MM-dd");
                        return medicationStatement;
                    case ResourceType.Condition:
                        var condition = (Condition)resource;
                        condition.Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{patientId}", Identifier = new Identifier() { Value = patientIdentifier } };
                        condition.Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } };
                        condition.Identifier = new List<Identifier>() { new Identifier() { Value = condition.Id } };
                        condition.RecordedDate = documentCreationDate.ToString("yyyy-MM-dd");
                        return condition;
                    case ResourceType.Procedure:
                        var procedure = (Procedure)resource;
                        procedure.Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{patientId}", Identifier = new Identifier() { Value = patientIdentifier } };
                        procedure.Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } };
                        procedure.Identifier = new List<Identifier>() { new Identifier() { Value = procedure.Id } };
                        //procedure.Performed.  ************* DA COMPLETARE
                        return procedure;
                }
            }

            return null;
        }
    }
}
