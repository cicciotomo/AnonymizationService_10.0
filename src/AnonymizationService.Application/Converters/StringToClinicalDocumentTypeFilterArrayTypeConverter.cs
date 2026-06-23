using AnonymizationService.ClinicalDocuments;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AnonymizationService.Converters
{


    public class StringToClinicalDocumentTypeFilterArrayTypeConverter
        : ITypeConverter<string, ClinicalDocumentTypeFilter[]>
    {
        public ClinicalDocumentTypeFilter[] Convert(
            string source,
            ClinicalDocumentTypeFilter[] destination,
            ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source) || source == "null")
                return Array.Empty<ClinicalDocumentTypeFilter>();

            try
            {
                return JsonSerializer.Deserialize<ClinicalDocumentTypeFilter[]>(source)
                       ?? Array.Empty<ClinicalDocumentTypeFilter>();
            }
            catch
            {
                return Array.Empty<ClinicalDocumentTypeFilter>();
            }
        }
    }


}
