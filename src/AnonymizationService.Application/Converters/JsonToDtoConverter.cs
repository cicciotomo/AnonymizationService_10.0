using AnonymizationService.Services.Dicom;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonToDtoConverter : ITypeConverter<string, DicomParametersJson>
{
    public DicomParametersJson Convert(string source, DicomParametersJson destination, ResolutionContext context)
    {
        return string.IsNullOrEmpty(source)
            ? null
            : JsonSerializer.Deserialize<DicomParametersJson>(source);
    }
}

public class DtoToJsonConverter : ITypeConverter<DicomParametersJson, string>
{
    public string Convert(DicomParametersJson source, string destination, ResolutionContext context)
    {
        return source == null
            ? null
            : JsonSerializer.Serialize(source);
    }
}
