using System.Collections.Generic;
using System.Xml.Serialization;

namespace AnonymizationService.Services.Pathox
{

	[XmlRoot("examIdResponse")]
	public class PathoxListResponse
	{

		public string statusField;

		public string errorField;

		public string patientIdField;

		public List<string> examIdListField;

		public string status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		public string Error
		{
			get
			{
				return this.errorField;
			}
			set
			{
				this.errorField = value;
			}
		}

		public string patientId
		{
			get
			{
				return this.patientIdField;
			}
			set
			{
				this.patientIdField = value;
			}
		}

		[System.Xml.Serialization.XmlArrayItemAttribute("examId", IsNullable = false)]
		public List<string> examIdList
		{
			get
			{
				return this.examIdListField;
			}
			set
			{
				this.examIdListField = value;
			}
		}
	}


	[XmlRoot("ReadyReport")]
	public partial class PathoxExamResult
	{
		public PathoxPatient patient { get; set; }

		public ReadyReportExam exam { get; set; }

		public string examBinded { get; set; }

		public PathoxReport report { get; set; }

		public string requestType { get; set; }

		public string messageId { get; set; }

		public string allRequestClosed { get; set; }

	}

	public class PathoxPatient
	{
		public int? patientId { get; set; }
		public string SSNNumber { get; set; }
	}

	public class ReadyReportExam
	{
		public int? antibodyCount { get; set; }

		public string dateTimeOfTransaction { get; set; }

		public string stringOfTransaction { get; set; }

		public string examTypeId { get; set; }

		public string fillerPendingLocationId { get; set; }

		public string fillerExpectedDeliveryDate { get; set; }

		public string fillerEntityId { get; set; }

		public string fillerEntityDisplayName { get; set; }

		public string fillerOperatingUnit { get; set; }

		public string fillerOrderingProviderId { get; set; }

		public string groupNumber { get; set; }
		public string groupNumberAlt { get; set; }

		public string hospitalDisplayName { get; set; }
		public string hospitalId { get; set; }

		public int? immunohistochemistryCount { get; set; }

		public int? materialCount { get; set; }

		public string orderingProviderId { get; set; }
		public string origin { get; set; }
		public string fillerOrigin { get; set; }

		public string pendingLocationId { get; set; }

		public string pendingLocationDisplayName { get; set; }

		public string pendingLocationMail { get; set; }
		public string priority { get; set; }

		public string processingstring { get; set; }

		public string specimenTiming { get; set; }

		public string specimenDeliveryTiming { get; set; }

		public string visitNumber { get; set; }

		public string nosographicNumber { get; set; }
		public bool? frozenSectionManagement { get; set; }
		[XmlArrayItemAttribute("Material", IsNullable = true)]
		public List<ExamMaterialsUser> ExamMaterialsUserList { get; set; }
		[XmlArrayItemAttribute("Service", IsNullable = true)]
		public List<ExamServices> ExamServicesList { get; set; }
		[XmlArrayItemAttribute("Slide", IsNullable = true)]
		public List<ExamColorSlides> ExamColorSlidesList { get; set; }


	}


	public partial class ExamMaterialsUser
	{

		public int? pathoxId { get; set; }

		public int? pathoxMaterialProg { get; set; }

		public string barcode { get; set; }

		public string code { get; set; }

		public string displayName { get; set; }

		public string fillerNotes { get; set; }

		public string fillerTypeCode { get; set; }

		public string fillerTopographyCode { get; set; }

		public string fillerTopographyDescription { get; set; }

		public int? quantity { get; set; }
	}

	public class ExamServices
	{
		public string IUP { get; set; }
		public string NRE { get; set; }
		public string placerOrderNumber { get; set; }
		public string quantity { get; set; }
		public string RUR { get; set; }
		public string universalServiceIdentifierId { get; set; }
		public string universalServiceIdentifierDisplayName { get; set; }
		public string fillerCode { get; set; }
		public string fillerDescription { get; set; }
		public string fillerNationalCode { get; set; }
		public string fillerRegionalCode { get; set; }
		public string fillerRegionalDescription { get; set; }
	}

	public class ExamColorSlides
	{

		public string barcode { get; set; }

		public int? typeId { get; set; }

		public string typeCode { get; set; }

		public string typeDescription { get; set; }

	}

	public class PathoxReport
	{
		public int? documentId { get; set; }

		public int? documentVersion { get; set; }

		public int? replacedDocumentId { get; set; }

		public string documentTiming { get; set; }

		public string acceptanceTiming { get; set; }

		public string macroscopyTiming { get; set; }

		public string processingTiming { get; set; }

		public string reportingTiming { get; set; }

		public string validationTiming { get; set; }

		public string signedPdf { get; set; }

		public string type { get; set; }
		public bool? isPositive { get; set; }

		public bool? caution { get; set; }

		[System.Xml.Serialization.XmlArrayItemAttribute("Data", IsNullable = true)]
		public List<ReportFreeTextField> ReportFreeTextFieldList { get; set; }
		[System.Xml.Serialization.XmlArrayItemAttribute("Paragraph", IsNullable = true)]
		public List<Paragraph> ReportParagraphs { get; set; }
		[System.Xml.Serialization.XmlArrayItemAttribute("TnmCoding", IsNullable = true)]
		public List<TnmCoding> TnmCodings { get; set; }


	}

	[XmlType("Data")]
	public class ReportFreeTextField
	{

		public string key { get; set; }

		public string value { get; set; }
	}


	public partial class Paragraph
	{

		public int? materialId { get; set; }

		public int? internalId { get; set; }

		public string description { get; set; }

		public string snomedProcedureCode { get; set; }

		public string snomedProcedureDescription { get; set; }

		public string snomedTopographyCode { get; set; }

		public string snomedTopographyDescription { get; set; }

		public string macroscopyReportTxtBase64 { get; set; }

		public string macroscopyReportRtfBase64 { get; set; }

		public string microscopyReportTxtBase64 { get; set; }

		public string microscopyReportRtfBase64 { get; set; }

		public string diagnosisReportTxtBase64 { get; set; }

		public string diagnosisReportRtfBase64 { get; set; }
		[System.Xml.Serialization.XmlArrayItemAttribute("Diagnosis", IsNullable = true)]
		public List<Diagnosis> DiagnosisList { get; set; }

	}

	public class TnmCoding
	{
		public int? id { get; set; }
		public int? examId { get; set; }
		public string parentType { get; set; }
		public string parentId { get; set; }
		public string prefix { get; set; }
		public string tnmT { get; set; }
		public string tnmN { get; set; }
		public string tnmM { get; set; }
		public string tnmG { get; set; }
		public string tnmR { get; set; }
		public string tnmPL { get; set; }
		public string tnmIV { get; set; }
		public string tnmStadio { get; set; }
		public string tnmV { get; set; }
		public string tnmL { get; set; }
		public string tnmPN { get; set; }
		public string tnmPrefissoG { get; set; }
		public string idStaging { get; set; }
		public string note { get; set; }

	}

	public class Diagnosis
	{

		public int? type { get; set; }

		public string code { get; set; }

		public string description { get; set; }
	}

}

