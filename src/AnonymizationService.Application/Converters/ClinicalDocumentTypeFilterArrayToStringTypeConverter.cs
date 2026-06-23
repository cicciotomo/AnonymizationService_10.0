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


    public class ClinicalDocumentTypeFilterArrayToStringTypeConverter
        : ITypeConverter<ClinicalDocumentTypeFilter[], string>
    {
        public string Convert(
            ClinicalDocumentTypeFilter[] source,
            string destination,
            ResolutionContext context)
        {
            return JsonSerializer.Serialize(source ?? Array.Empty<ClinicalDocumentTypeFilter>());
        }
    }


}
