using AnonymizationService.Services.DbUri;
using Porini.Abp.StateMachineEngine.Jobs;
using System;
using System.Collections.Generic;

namespace AnonymizationService.Jobs.UpsertFhirDbUriData
{
	internal class UpsertFhirDbUriDataArgs : JobArgs
	{
		public Guid CloudPatientId { get; set; }
		public string FhirPatientId { get; set; }
		public List<Event> DbUriEventToUpload { get; set; }
		public List<Item> DbUriFUpItemToUpload { get; set; }
		public FupData FupData { get; set; }
		public Patient Patient { get; set; }
	}
	public class FupData
	{
		public long id { get; set; }
		public long? idStructure { get; set; }
		public Double? examEmCreatininemia06 { get; set; }
		public string examEmCreatininemia06Um { get; set; }
		public string examEmCreatininemia06Range { get; set; }
		public Double? examEmCreatininemia12 { get; set; }
		public string examEmCreatininemia12Um { get; set; }
		public string examEmCreatininemia12Range { get; set; }
		public Double? examEmCreatininemia24 { get; set; }
		public string examEmCreatininemia24Um { get; set; }
		public string examEmCreatininemia24Range { get; set; }
		public Double? examEmCreatininemia36 { get; set; }
		public string examEmCreatininemia36Um { get; set; }
		public string examEmCreatininemia36Range { get; set; }
		public Double? examEmCreatininemia48 { get; set; }
		public string examEmCreatininemia48Um { get; set; }
		public string examEmCreatininemia48Range { get; set; }
		public Double? examEmCreatininemia60 { get; set; }
		public string examEmCreatininemia60Um { get; set; }
		public string examEmCreatininemia60Range { get; set; }
		public Double? examEmCreatininemia72 { get; set; }
		public string examEmCreatininemia72Um { get; set; }
		public string examEmCreatininemia72Range { get; set; }
		public Double? examEmCreatininemia84 { get; set; }
		public string examEmCreatininemia84Um { get; set; }
		public string examEmCreatininemia84Range { get; set; }
		public Double? examEmCreatininemia96 { get; set; }
		public string examEmCreatininemia96Um { get; set; }
		public string examEmCreatininemia96Range { get; set; }
		public Double? examEmCreatininemia108 { get; set; }
		public string examEmCreatininemia108Um { get; set; }
		public string examEmCreatininemia108Range { get; set; }
		public Double? examEmCreatininemia120 { get; set; }
		public string examEmCreatininemia120Um { get; set; }
		public string examEmCreatininemia120Range { get; set; }
		public Double? examEmGFR12 { get; set; }
		public string patientEnrolledProtocol { get; set; }
		public int? hasCardiovascularEvent { get; set; }
		public int? cardiovascularEventSurvival { get; set; }
		public int? hasPrimaryTumor { get; set; }
		public int? primaryTumorTimeTo { get; set; }
		public int? hasKidneyPrimaryTumor { get; set; }
		public int? kidneyPrimaryTumorTimeTo { get; set; }
		public int? hasRelapse { get; set; }
		public int? relapseSurvival { get; set; }
		public int? hasTherapyAdjuvant { get; set; }
		public int? hasDiabetes { get; set; }
		public int? diabetesTimeTo { get; set; }
	}
}
