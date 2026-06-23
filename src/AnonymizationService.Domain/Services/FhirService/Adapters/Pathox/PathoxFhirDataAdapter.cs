using AnonymizationService.Services.Pathox;
using Hl7.Fhir.Model;
using Hl7.FhirPath.Sprache;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.FhirService.Adapters.Pathox
{
	public class PathoxFhirDataAdapter : FhirDataAdapterBase, IPathoxFhirDataAdapter, ITransientDependency
	{
		public List<Resource> Transform(PathoxExamResult examResult, PathoxTransformationAdditionalInfo pathoxTransformationAdditionalInfo)
		{
			var fhirResources = new List<Resource>();

			var fhirPatientId = pathoxTransformationAdditionalInfo.FhirPatientId;
			var cloudPatientId = pathoxTransformationAdditionalInfo.CloudPatientId;
			var encounterIdentifier = examResult.exam.groupNumberAlt;
			CultureInfo enUS = new CultureInfo("en-US");
			if (!DateTime.TryParseExact(examResult.exam.dateTimeOfTransaction, "yyyyMMddHHmm", enUS, System.Globalization.DateTimeStyles.None, out var encounterDate))
			{
				encounterDate = DateTime.Now;
			}
			var encounterDateString = encounterDate.ToString("yyyy-MM-ddThh:mm:sszzzz");
			var pathoxOrganizationId = pathoxTransformationAdditionalInfo.PathoxOrganizationId;

			var encounter = new Encounter()
			{
				Subject = new ResourceReference()
				{
					Type = "Pathology",
					Reference = $"Patients/{fhirPatientId}",
					Identifier = new Identifier() { Value = cloudPatientId.ToString() }
				},
				Identifier = new List<Identifier>() { new Identifier() { Value = encounterIdentifier } },
				StatusElement = new Code<Encounter.EncounterStatus>(Encounter.EncounterStatus.Finished),
				Class = new Coding("http://terminology.hl7.org/CodeSystem/v3-ActCode", "IMP"),
				Period = new Period() { Start = encounterDateString, End = encounterDateString }
			};

			var examDataFhirResources = GetExamDataFhirResources(examResult.exam, examResult.examBinded, fhirPatientId, cloudPatientId, encounterIdentifier, encounterDate, encounterDateString, pathoxOrganizationId);
			fhirResources.AddRange(examDataFhirResources);

			var reportDataFhirResources = GetReportDataFhirResources(examResult.report, fhirPatientId, cloudPatientId, encounterIdentifier, encounterDate, encounterDateString, pathoxOrganizationId);
			fhirResources.AddRange(reportDataFhirResources);

			fhirResources.Add(encounter);

			return fhirResources;
		}

		private List<Resource> GetReportDataFhirResources(PathoxReport report, string fhirPatientId, Guid cloudPatientId, string encounterIdentifier, DateTime encounterDate, string encounterDateString, string pathoxOrganizationId)
		{
			var fhirResources = new List<Resource>();

			var reportDataComposition = new Composition()
			{
				Subject = new ResourceReference() { Type = "Pathology", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
				Identifier = new Identifier() { Value = $"{encounterIdentifier}-report" },
				Section = new List<Composition.SectionComponent>(),
				Title = "ReportData",
				Type = new CodeableConcept("http://loinc.org", "11526-1", "Pathology study"),
				Status = CompositionStatus.Final,
				Date = encounterDateString,
				Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
				Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = pathoxOrganizationId }, Type = "Organization" } }
			};

			var propertyNameToSkip = new List<string>()
			{
				nameof(report.ReportFreeTextFieldList),
				nameof(report.ReportParagraphs),
				nameof(report.TnmCodings),
			};

			var reportGeneralInformationObservations = CreateObservationsFromProperties(report, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate, propertyNameToSkip);

			if (reportGeneralInformationObservations.Any())
			{
				reportDataComposition.Section.Add(new Composition.SectionComponent()
				{
					Title = "GeneralInformation",
					Entry = reportGeneralInformationObservations.Select(x => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = x.Identifier.First().Value } }).ToList()
                });
				fhirResources.AddRange(reportGeneralInformationObservations);
			}

			//Map ReportFreeTextFieldList
			var reportFreeTextFieldObservations = new List<Observation>();
			foreach (var reportFreeTextField in report.ReportFreeTextFieldList)
			{
				var observation = CreateObservation(cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate, reportFreeTextField.key, reportFreeTextField.value);
				if (observation != null)
				{
					reportFreeTextFieldObservations.Add(observation);
				}
			}

			if (reportFreeTextFieldObservations.Any())
			{
				reportDataComposition.Section.Add(new Composition.SectionComponent()
				{
					Title = "ReportFreeTextFieldList",
					Entry = reportFreeTextFieldObservations.Select(x => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = x.Identifier.First().Value } }).ToList()
                });
				fhirResources.AddRange(reportFreeTextFieldObservations);
			}

			//Map ReportParagraphs
			var reportParagraphsCompositionSection = new Composition.SectionComponent()
			{
				Title = "ReportParagraphs",
				Section = new List<Composition.SectionComponent>()
			};

			foreach (var reportParagraph in report.ReportParagraphs)
			{
				var reportParagraphPropertyNameToSkip = new List<string>()
				{
					nameof(reportParagraph.DiagnosisList),
				};

				var reportParagraphObservations = CreateObservationsFromProperties(reportParagraph, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate, reportParagraphPropertyNameToSkip);
				if (reportParagraphObservations.Any())
				{
					var paragraphCompositionSection = new Composition.SectionComponent()
					{
						Title = reportParagraph.materialId.ToString(),
						Entry = reportParagraphObservations.Select(x => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = x.Identifier.First().Value } }).ToList(),
						Section = new List<Composition.SectionComponent>()
					};

					int diagnosisIndex = 0;
					foreach (var diagnosis in reportParagraph.DiagnosisList)
					{
						var diagnosisObservations = CreateObservationsFromProperties(diagnosis, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate);
						if (diagnosisObservations.Any())
						{
							var diagnosisCompositionSection = new Composition.SectionComponent()
							{
								Title = $"Diagnosis-{diagnosisIndex}",
								Entry = diagnosisObservations.Select(x => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = x.Identifier.First().Value } }).ToList()
                            };

							paragraphCompositionSection.Section.Add(diagnosisCompositionSection);
							fhirResources.AddRange(diagnosisObservations);

							diagnosisIndex++;
						}
					}

					reportParagraphsCompositionSection.Section.Add(paragraphCompositionSection);

					fhirResources.AddRange(reportParagraphObservations);
				}
			}

			if (reportParagraphsCompositionSection.Section.Any())
			{
				reportDataComposition.Section.Add(reportParagraphsCompositionSection);
			}

			//Map TnmCodings
			var tnmCodingsCompositionSection = new Composition.SectionComponent()
			{
				Title = "TnmCodings",
				Section = new List<Composition.SectionComponent>()
			};

			int tnmCodingIndex = 0;
			foreach (var tnmCoding in report.TnmCodings)
			{
				var tnmCodingObservations = CreateObservationsFromProperties(tnmCoding, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate);
				if (tnmCodingObservations.Any())
				{
					var tnmCodingCompositionSection = new Composition.SectionComponent()
					{
						Title = $"TnmCoding-{tnmCodingIndex}",
						Entry = tnmCodingObservations.Select(x => new ResourceReference() { Reference = $"Observation/{x.Id}" }).ToList()
					};

					tnmCodingsCompositionSection.Section.Add(tnmCodingCompositionSection);

					fhirResources.AddRange(tnmCodingObservations);
					tnmCodingIndex++;
				}
			}

			if (tnmCodingsCompositionSection.Section.Any())
			{
				reportDataComposition.Section.Add(tnmCodingsCompositionSection);
			}

			fhirResources.Add(reportDataComposition);

			return fhirResources;
		}

		private List<Resource> GetExamDataFhirResources(ReadyReportExam exam, string examBinded, string fhirPatientId, Guid cloudPatientId, string encounterIdentifier, DateTime encounterDate, string encounterDateString, string pathoxOrganizationId)
		{
			var fhirResources = new List<Resource>();

			var examDataComposition = new Composition()
			{
				Subject = new ResourceReference() { Type = "Pathology", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
				Identifier = new Identifier() { Value = $"{encounterIdentifier}-exam" },
				Section = new List<Composition.SectionComponent>(),
				Title = "ExamData",
				Type = new CodeableConcept("http://loinc.org", "11526-1", "Pathology study"),
				Status = CompositionStatus.Final,
				Date = encounterDateString,
				Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
				Author = new List<ResourceReference>() { new() { Identifier = new Identifier() { Value = pathoxOrganizationId }, Type = "Organization" } }
			};

			var propertyNameToSkip = new List<string>()
			{
				nameof(exam.ExamMaterialsUserList),
				nameof(exam.ExamServicesList),
				nameof(exam.ExamColorSlidesList),
			};

			var examGeneralInformationObservations = CreateObservationsFromProperties(exam, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate, propertyNameToSkip);
			var examBindedObservation = CreateObservation(cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate, "examBinded", examBinded);
			examGeneralInformationObservations.Add(examBindedObservation);

			examDataComposition.Section.Add(new Composition.SectionComponent()
			{
				Title = "GeneralInformation",
				Entry = examGeneralInformationObservations.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
			});
			fhirResources.AddRange(examGeneralInformationObservations);

			//Map ExamMaterialsUserList
			var examMaterialsUserListCompositionSection = new Composition.SectionComponent()
			{
				Title = "ExamMaterialsUserList",
				Section = new List<Composition.SectionComponent>()
			};

			foreach (var examMaterialsUser in exam.ExamMaterialsUserList)
			{
				var examMaterialsUserObservations = CreateObservationsFromProperties(examMaterialsUser, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate);
				if (examMaterialsUserObservations != null)
				{
					examMaterialsUserListCompositionSection.Section.Add(new Composition.SectionComponent()
					{
						Title = examMaterialsUser.pathoxId.ToString(),
						Entry = examMaterialsUserObservations.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
					});
					fhirResources.AddRange(examMaterialsUserObservations);
				}
			}

			if (examMaterialsUserListCompositionSection.Section.Any())
			{
				examDataComposition.Section.Add(examMaterialsUserListCompositionSection);
			}

			//Map ExamServicesList
			var examServicesListCompositionSection = new Composition.SectionComponent()
			{
				Title = "ExamServicesList",
				Section = new List<Composition.SectionComponent>()
			};

			foreach (var examServices in exam.ExamServicesList)
			{
				var examServicesObservations = CreateObservationsFromProperties(examServices, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate);
				if (examServicesObservations != null)
				{
					examServicesListCompositionSection.Section.Add(new Composition.SectionComponent()
					{
						Title = examServices.placerOrderNumber,
						Entry = examServicesObservations.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
					});
					fhirResources.AddRange(examServicesObservations);
				}
			}

			if (examServicesListCompositionSection.Section.Any())
			{
				examDataComposition.Section.Add(examServicesListCompositionSection);
			}

			//Map ExamColorSlidesList
			var examColorSlidesListCompositionSection = new Composition.SectionComponent()
			{
				Title = "ExamColorSlidesList",
				Section = new List<Composition.SectionComponent>()
			};

			foreach (var examColorSlides in exam.ExamColorSlidesList)
			{
				var examColorSlidesObservations = CreateObservationsFromProperties(examColorSlides, cloudPatientId, fhirPatientId, encounterIdentifier, encounterDate);
				if (examColorSlidesObservations != null)
				{
					examColorSlidesListCompositionSection.Section.Add(new Composition.SectionComponent()
					{
						Title = examColorSlides.barcode.ToString(),
						Entry = examColorSlidesObservations.Select(m => new ResourceReference() { Type = "Observation", Identifier = new Identifier() { Value = m.Identifier.First().Value } }).ToList()
					});
					fhirResources.AddRange(examColorSlidesObservations);
				}
			}

			if (examColorSlidesListCompositionSection.Section.Any())
			{
				examDataComposition.Section.Add(examColorSlidesListCompositionSection);
			}

			fhirResources.Add(examDataComposition);

			return fhirResources;
		}
	}
}
