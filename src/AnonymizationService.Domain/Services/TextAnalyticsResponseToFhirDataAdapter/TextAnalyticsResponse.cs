using Hl7.Fhir.Model;
using System.Collections.Generic;

namespace AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter
{
    public class TextAnalyticsResponse
    {
        public string JobId { get; set; }
        public TextAnalyticsTaskResult Tasks { get; set; }
    }

    public class TextAnalyticsTaskResult
    {
        public TextAnalyticsTaskItems[] Items { get; set; }
    }

    public class TextAnalyticsTaskItems
    {
        public TextAnalyticsTaskItemResult Results { get; set; }
    }

    public class TextAnalyticsTaskItemResult
    {
        public TextAnalyticsDocument[] Documents { get; set; }
    }

    public class TextAnalyticsDocument
    {
        public List<TextAnalyticsEntity> Entities { get; set; }
        public List<TextAnalyticsRelation> Relations { get; set; }
        public Bundle FhirBundle { get; set; }
    }

    public class TextAnalyticsRelation
    {
        public string RelationType { get; set; }
        public TextAnalyticsRelationEntity[] Entities { get; set; }
    }

    public class TextAnalyticsRelationEntity
    {
        public string Ref { get; set; }
        public string Role { get; set; }
    }

    public class TextAnalyticsEntity
    {
        public int Offset { get; set; }
        public int Length { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public float ConfidenceScore { get; set; }
        public string Name { get; set; }
        public TextAnalyticsEntityLink[] Links { get; set; }
        public TextAnalyticsEntityAssertion Assertion { get; set; }
    }

    public class TextAnalyticsEntityLink
    {
        public string DataSource { get; set; }
        public string Id { get; set; }
    }

    /// <summary>
    ///    
    ///     Introdotto per interpretare le assertions delle entities identificate
    ///      "assertion":{
    ///        "certainly":"positivePossible"
    ///        ,"conditional":"testo"
    ///        ,"association":"testo"
    ///        ,"temporal":"testo"
    ///       }
    ///      
    ///     Per difficoltà di identificazione dei casi di test
    ///     si sceglie di non portare le proprietà seguenti
    ///    
    ///     public string Conditional { get; set; }
    ///     public string Association { get; set; }
    ///     public string Temporal { get; set; }
    ///    
    /// </summary>

    public class TextAnalyticsEntityAssertion
    {
        public string Certainty { get; set; }
    }
}
