using System;
using System.Collections.Generic;

namespace AnonymizationService.Services.PlatformManagementConsole;

public class FullStudyMetadata
{
    public Guid Id { get; set; }
    public string FhirResearchStudyId { get; set; }
    public string Acronym { get; set; }
    public string FullTitle { get; set; }
    public CohortDefinitionDto cohorteDefinition { get; set; }
    public List<PatientDetail> Patients { get; set; }
}


public class PatientDetail
{
    public string MPI { get; set; }
    public Guid CPI { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsFromCohortBuilder { get; set; }
    public bool IsManuallyManaged { get; set; }
}

public class CohortDefinitionDto
{
    public string ShortDescription { get; set; }
    public string LongDescription { get; set; }
    public Condition Conditions { get; set; }
}

public class Condition
{
    public string Operator { get; set; }
    public List<Filter>? Filters { get; set; }
    public List<Condition>? InnerCondition { get; set; }
}

public class Filter
{
    public string Key { get; set; }
    public string Entity { get; set; }
    public string Operator { get; set; }
    public string ValueType { get; set; }
    public string ValueString { get; set; }
    public decimal ValueNumber { get; set; }
    public DateTime ValueDate { get; set; }
}
