using AutoMapper;
using AnonymizationService.ClinicalDocuments;
using System;
using System.Collections.Generic;
using System.Text.Json;

public class JsonToClinicalDocumentTypeFilterListConverter
    : IValueConverter<string, ClinicalDocumentTypeFilter[]>
{
    public ClinicalDocumentTypeFilter[] Convert(string sourceMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(sourceMember))
            return Array.Empty<ClinicalDocumentTypeFilter>();

        return JsonSerializer.Deserialize<ClinicalDocumentTypeFilter[]>(sourceMember)
               ?? Array.Empty<ClinicalDocumentTypeFilter>();
    }
}

public class ClinicalDocumentTypeFilterListToJsonConverter
    : IValueConverter<ClinicalDocumentTypeFilter[], string>
{
    public string Convert(ClinicalDocumentTypeFilter[] sourceMember, ResolutionContext context)
    {
        if (sourceMember == null || sourceMember.Length == 0)
            return "[]";

        return JsonSerializer.Serialize(sourceMember);
    }
}

public class JsonToStringArrayConverter : IValueConverter<string, string[]>
{
    public string[] Convert(string sourceMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(sourceMember))
            return Array.Empty<string>();

        return JsonSerializer.Deserialize<string[]>(sourceMember)
               ?? Array.Empty<string>();
    }
}

public class StringArrayToJsonConverter : IValueConverter<string[], string>
{
    public string Convert(string[] sourceMember, ResolutionContext context)
    {
        if (sourceMember == null || sourceMember.Length == 0)
            return "[]";

        return JsonSerializer.Serialize(sourceMember);
    }
}