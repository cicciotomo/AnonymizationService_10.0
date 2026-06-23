using AnonymizationService.Jobs.SendClinicalDataToAzure;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.TextAnalyticsResponseToFhirDataAdapter
{
    public class TextAnalyticsResponseToFhirDataAdapter : ITextAnalyticsResponseToFhirDataAdapter, ITransientDependency
    {
        private readonly ILogger<TextAnalyticsResponseToFhirDataAdapter> _logger;

        public TextAnalyticsResponseToFhirDataAdapter(ILogger<TextAnalyticsResponseToFhirDataAdapter> logger) {
            _logger = logger;
        }

        public List<DocumentSectionExtractionResult> ExtractFhirResources(List<DocumentSection> documentSections)
        {
            var result = new List<DocumentSectionExtractionResult>();
            int count = 0;
            TextAnalyticsResponse textAnalyticsResponse;
            foreach (var documentSection in documentSections)
            {
                ++count;
                var options = new JsonSerializerOptions().ForFhir(typeof(Bundle).Assembly);

                options.PropertyNameCaseInsensitive = true;
                try
                {
                    textAnalyticsResponse = JsonSerializer.Deserialize<TextAnalyticsResponse>(documentSection.SerializedFhirBundle, options);

                    var existingResources = textAnalyticsResponse.Tasks?
                        .Items?.SelectMany(i =>
                            i.Results?.Documents?.SelectMany(d =>
                                d.FhirBundle?.Entry?.Select(e => e.Resource)));

                    var foundEntities = textAnalyticsResponse.Tasks?
                        .Items?.SelectMany(i =>
                            i.Results?.Documents?.SelectMany(d =>
                                d.Entities));

                    var foundRelations = textAnalyticsResponse.Tasks?
                        .Items?.SelectMany(i =>
                            i.Results?.Documents?.SelectMany(d =>
                                d.Relations));

                    result.Add(new DocumentSectionExtractionResult()
                    {
                        Id = textAnalyticsResponse.JobId,
                        SectionName = documentSection.SectionName,
                        FhirResources = existingResources?.ToList(),
                        Entities = foundEntities?.ToList(),
                        Relations = foundRelations?.ToList()
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"La sezione ({count}) {documentSection.SectionName} è stata skippata a causa di un errore");
                    _logger.LogError("ExtractFhirResources Exception - " + ex.Message);
                    _logger.LogException(ex);
                }
            }
            _logger.LogDebug($"Sezioni del documento pronte ad essere inviate su FHIR {result.Count} di {documentSections.Count} ");
            return result;
        }
    }
}