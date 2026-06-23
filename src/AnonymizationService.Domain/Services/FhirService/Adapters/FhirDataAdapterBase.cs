using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnonymizationService.Services.FhirService.Adapters
{
	public class FhirDataAdapterBase
	{
		private const string DEFAULT_CODE_SISTEM_PROVIDER = "https//www.hsr.it";
		protected List<Observation> CreateObservationsFromProperties<T>(T obj, Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? examDate, List<string> propertyNamesToSkip = null)
		{
			if (obj == null)
			{
				return null;
			}

			propertyNamesToSkip ??= new List<string>();

			var properties = typeof(T).GetProperties().Where(p => !propertyNamesToSkip.Contains(p.Name));

			var observations = new List<Observation>();

			foreach (var property in properties)
			{
				var observationCode = property.Name;
				var observationValue = property.GetValue(obj)?.ToString();
				Observation observation = CreateObservation(cloudPatientId, fhirPatientId, encounterIdentifier, examDate, observationCode, observationValue);

				observations.Add(observation);
			}
			return observations;
		}

		protected Observation CreateObservation(Guid cloudPatientId, string fhirPatientId, string encounterIdentifier, DateTime? examDate, string observationCode, string observationValue)
		{
			return new Observation()
			{
				Subject = new ResourceReference() { Type = "Patient", Reference = $"Patient/{fhirPatientId}", Identifier = new Identifier() { Value = cloudPatientId.ToString() } },
				Encounter = new ResourceReference() { Identifier = new Identifier() { Value = encounterIdentifier } },
				Identifier = new List<Identifier>() { new() { Value = Guid.NewGuid().ToString() } },
				Status = ObservationStatus.Final,
				Issued = examDate,
				Value = new FhirString(observationValue),
				Code = new CodeableConcept(DEFAULT_CODE_SISTEM_PROVIDER, observationCode),
			};
		}


	}
}
