namespace AnonymizationService.Enums
{
    public enum ExternalServiceRequestTypeEnum
    {
        ValidateMasterPatientIndex = 0,
        GetPatientLaboratoryExamResults = 1,
        GetPatientClinicalDataResults = 2,
        GetDocument = 3,
        GetAuthToken = 4,
        GetDbUriData = 5,
		GetPathoxExamList = 6,
		GetPathoxExamDetail = 7,
	}
}
