using AnonymizationService.Redcap;
using Azure;
using DemographicWS;
using EnsureThat.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace AnonymizationService.Services.Redcap
{
    public class RedcapClient: IRedcapClient, ITransientDependency
    {
        private readonly HttpClient _httpClient;
        public readonly ILogger<RedcapClient> _logger;

        public RedcapClient(HttpClient httpClient, ILogger<RedcapClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

        }

        public class RedcapReturnData { 
            public string RedCapID { get; set; }
            public string Data { get; set; }
        }


        public async Task<RedcapReturnData> GetPatientDataByMPIAsync(string masterPatientIndex,
                                                           string mpiColumnName,
                                                           string redcapToken,
                                                           string endpoint,
                                                           string studyFields,
                                                           string cloudPatientIndex
                                                           )
        {


            if (!studyFields.Contains(mpiColumnName))
            {
                studyFields += String.Format(",{0}",mpiColumnName);
            }
            #region Retrieve Patient Redcap Study
                try
                {
                    _logger.LogInformation("Requesting Study patient data from redcap api...");
                    var payload = new Dictionary<string, string>
                {
                    { "token", redcapToken },
                    { "content", "record" },
                    { "format", "json" },
                    { "type", "flat" },
                    { "filterLogic", $"[{mpiColumnName}] = '{masterPatientIndex}'" },
                    { "fields", studyFields },
                    { "csvDelimiter", "" },
                    { "rawOrLabel", "raw" },
                    { "rawOrLabelHeaders", "raw" },
                    { "exportCheckboxLabel", "false" },
                    { "exportSurveyFields", "false" },
                    { "exportDataAccessGroups", "false" },
                    { "returnFormat", "json" }
                };

                var content = new FormUrlEncodedContent(payload);

                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                #endregion

                return AddCPIandRemoveMpiToRedcapData(data, cloudPatientIndex, mpiColumnName, masterPatientIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in retrieving data from redcapAPI");
                throw new Exception(ex.Message, ex);
            }


        }

        private string GetRecordId(string data)
        {
            return "15";
        }

        public async Task<RedcapReturnData> GetPatientDataByRecordIdAsync(string redcapToken,
                                                           string mpiColumnName,
                                                           string endpoint,
                                                           string studyFields,
                                                           string cloudPatientIndex,
                                                           string record_id)
        {
            try
            {
                _logger.LogInformation("Requesting Study patient data from redcap api...");
                var payload = new Dictionary<string, string>
            {
                { "token", redcapToken },
                { "content", "record" },
                { "format", "json" },
                { "type", "flat" },
                { "filterLogic", $"[{mpiColumnName}] = '{record_id}'" },
                { "fields", studyFields },
                { "rawOrLabel", "raw" },
                { "rawOrLabelHeaders", "raw" },
                { "exportCheckboxLabel", "false" },
                { "exportSurveyFields", "false" },
                { "exportDataAccessGroups", "false" },
                { "returnFormat", "json" }
            };

                var content = new FormUrlEncodedContent(payload);

                var response = await _httpClient.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();


                return AddCPIToRedcapData(data, cloudPatientIndex);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in retrieving data from redcapAPI");
                throw new Exception(ex.Message, ex);
            }


        }


        public async Task<string> GetStudyMetadataAsync(string redcapToken,
                                                        string endpoint)
        {

            #region Retrieve Redcap Study Metadata
            try
            {
                _logger.LogInformation("Requesting Study metadata from redcap api...");
                var payload = new Dictionary<string, string>
        {
            { "token", redcapToken }, // Ottieni il token per il progetto specifico
            { "content", "metadata" },
            { "format", "json" },
            { "returnFormat", "json" }
        };

            var content = new FormUrlEncodedContent(payload);

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();


                var metadata = await response.Content.ReadAsStringAsync();
            #endregion
            return metadata;
            }
            catch 
            {
                _logger.LogError("Error in retrieving metadata from redcapAPI");
                throw;
            }
        }

        public async Task<string> GetStudyDataAsync(string redcapToken, string endpoint, string studyFields, string requestFilter)
        {


            #region Retrieve Redcap Study data
            try
            {
                _logger.LogInformation("Requesting Study data from redcap api...");
                var payload = new Dictionary<string, string>();
                if (requestFilter == "")
                {
                    payload = new Dictionary<string, string>
                    {
                        { "token", redcapToken },
                        { "content", "record" },
                        { "fields", studyFields },
                        { "format", "json" },
                        { "returnFormat", "json" }
                    };

                } 
                else
                {
                     payload = new Dictionary<string, string>
                    {
                        { "token", redcapToken },
                        { "content", "record" },
                        { "filterLogic", requestFilter },
                        { "fields", studyFields },
                        { "format", "json" },
                        { "returnFormat", "json" }
                    };
                }
            var content = new FormUrlEncodedContent(payload);

            var response = await _httpClient.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();


                var data = await response.Content.ReadAsStringAsync();
            #endregion
            return data;
        }
            catch (Exception ex)
            {
                _logger.LogError("Error in retrieving data from redcapAPI");
                throw;
    }
}
        private static string RecordIdIdentifierString = "record_id";
        internal RedcapReturnData ModifyRedcapData(string jsonData, string[] fieldsToAdd, string cpi)
        {
            RedcapReturnData ret = new RedcapReturnData();

            using (JsonDocument doc = JsonDocument.Parse(jsonData))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            writer.WriteStartArray();

                            writer.WriteStartObject();
                            writer.WriteString("CloudPatientIndex", cpi);

                            foreach (var property in doc.RootElement.EnumerateObject())
                            {
                                if (property.Name == "record_id")
                                    ret.RedCapID = property.Value.ToString();   // Verificare!!!
                                if (fieldsToAdd.Contains(property.Name))
                                {
                                    if (property.Value.ValueKind == JsonValueKind.Object)
                                    {
                                        writer.WritePropertyName(property.Name);
                                        WriteObjectRecursive(writer, property.Value, fieldsToAdd);
                                    }
                                    else if (property.Value.ValueKind == JsonValueKind.Array)
                                    {
                                        writer.WritePropertyName(property.Name);
                                        WriteArrayRecursive(writer, property.Value, fieldsToAdd);
                                    }
                                    else
                                    {
                                        property.WriteTo(writer);
                                    }
                                }
                            }

                            writer.WriteEndObject();
                            writer.WriteEndArray();
                        }
                        else
                        {
                            writer.WriteStartArray();

                            foreach (var element in doc.RootElement.EnumerateArray())
                            {
                                writer.WriteStartObject();
                                writer.WriteString("CloudPatientIndex", cpi);

                                foreach (var property in element.EnumerateObject())
                                {

                                    if (fieldsToAdd.Contains(property.Name))
                                    {
                                        if (property.Value.ValueKind == JsonValueKind.Object)
                                        {
                                            writer.WritePropertyName(property.Name);
                                            WriteObjectRecursive(writer, property.Value, fieldsToAdd);
                                        }
                                        else if (property.Value.ValueKind == JsonValueKind.Array)
                                        {
                                            writer.WritePropertyName(property.Name);
                                            WriteArrayRecursive(writer, property.Value, fieldsToAdd);
                                        }
                                        else
                                        {
                                            property.WriteTo(writer);
                                        }
                                    }
                                }



                                writer.WriteEndObject();
                            }

                            writer.WriteEndArray();
                        }
                    }

                    ret.Data = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    return ret;
                }
            }
        }

        // Recursive method to manage nested objects
        internal void WriteObjectRecursive(Utf8JsonWriter writer, JsonElement element, string[] fieldsToAdd)
        {
            writer.WriteStartObject();
            foreach (var property in element.EnumerateObject())
            {
                if (fieldsToAdd.Contains(property.Name))
                {
                    if (property.Value.ValueKind == JsonValueKind.Object)
                    {
                        writer.WritePropertyName(property.Name);
                        WriteObjectRecursive(writer, property.Value, fieldsToAdd);
                    }
                    else if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        writer.WritePropertyName(property.Name);
                        WriteArrayRecursive(writer, property.Value, fieldsToAdd);
                    }
                    else
                    {
                        property.WriteTo(writer);
                    }
                }
            }
            writer.WriteEndObject();
        }


        internal void WriteArrayRecursive(Utf8JsonWriter writer, JsonElement element, string[] fieldsToAdd)
        {
            writer.WriteStartArray();
            foreach (var item in element.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    WriteObjectRecursive(writer, item, fieldsToAdd);
                }
                else if (item.ValueKind == JsonValueKind.Array)
                {
                    WriteArrayRecursive(writer, item, fieldsToAdd);
                }
                else
                {
                    item.WriteTo(writer);
                }
            }
            writer.WriteEndArray();
        }

        internal RedcapReturnData AddCPIToRedcapData(string jsonData, string cpi)
        {
            RedcapReturnData ret = new RedcapReturnData();

            using (JsonDocument doc = JsonDocument.Parse(jsonData))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            writer.WriteStartObject();

                            // Aggiungere il campo "cloud_patient_index" all'inizio
                            writer.WriteString("cloud_patient_index", cpi);

                            // Scrivere le altre proprietà, escludendo "record_id"
                            foreach (var property in doc.RootElement.EnumerateObject())
                            {
                                if (property.Name != "record_id")
                                {
                                    property.WriteTo(writer);
                                }
                                else
                                {
                                    ret.RedCapID = property.Value.ToString();
                                }
                            }

                            writer.WriteEndObject();
                        }
                        else if (doc.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            writer.WriteStartArray();

                            foreach (var element in doc.RootElement.EnumerateArray())
                            {
                                writer.WriteStartObject();

                                // Aggiungere il campo "cloud_patient_index" all'inizio di ogni oggetto
                                writer.WriteString("cloud_patient_index", cpi);

                                // Scrivere le altre proprietà, escludendo "record_id"
                                foreach (var property in element.EnumerateObject())
                                {
                                    if (property.Name != "record_id")
                                    {
                                        property.WriteTo(writer);
                                    }
                                    else
                                    {
                                        ret.RedCapID = property.Value.ToString();
                                    }
                                }

                                writer.WriteEndObject();
                            }

                            writer.WriteEndArray();
                        }
                    }

                    // Assegnare il risultato serializzato al campo Data
                    ret.Data = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    return ret;
                }
            }
        }

        internal RedcapReturnData AddCPIandRemoveMpiToRedcapData(string jsonData, string cpi, string mpiColunmName, string masterPatientIndex)
        {
            RedcapReturnData ret = new RedcapReturnData();

            using (JsonDocument doc = JsonDocument.Parse(jsonData))
            {
                using (var stream = new MemoryStream())
                {
                    using (var writer = new Utf8JsonWriter(stream))
                    {
                        if (doc.RootElement.ValueKind == JsonValueKind.Object)
                        {
                            writer.WriteStartObject();

                            // Aggiungere il campo "cloud_patient_index" all'inizio
                            writer.WriteString("cloud_patient_index", cpi);

                            // Scrivere le altre proprietà, escludendo "record_id"
                            foreach (var property in doc.RootElement.EnumerateObject())
                            {
                                if (property.Name == mpiColunmName && property.Value.ToString() != masterPatientIndex)
                                {
                                    throw new Exception("Mpi not found");
                                }
                                if ((property.Name != "record_id" && property.Name != mpiColunmName))
                                {
                                    property.WriteTo(writer);
                                }
                                if(property.Name == "record_id")
                                {
                                    ret.RedCapID = property.Value.ToString();
                                }
                            }

                            writer.WriteEndObject();
                        }
                        else if (doc.RootElement.ValueKind == JsonValueKind.Array)
                        {
                            writer.WriteStartArray();

                            foreach (var element in doc.RootElement.EnumerateArray())
                            {
                                writer.WriteStartObject();

                                // Aggiungere il campo "cloud_patient_index" all'inizio di ogni oggetto
                                writer.WriteString("cloud_patient_index", cpi);

                                // Scrivere le altre proprietà, escludendo "record_id"
                                foreach (var property in element.EnumerateObject())
                                {
                                    if (property.Name == mpiColunmName && property.Value.ToString() != masterPatientIndex)
                                    {
                                        throw new Exception("Mpi not found");
                                    }
                                    if ((property.Name != "record_id" && property.Name != mpiColunmName))
                                    {
                                        property.WriteTo(writer);
                                    }
                                    if (property.Name == "record_id")
                                    {
                                        ret.RedCapID = property.Value.ToString();
                                    }
                                }

                                writer.WriteEndObject();
                            }

                            writer.WriteEndArray();
                        }
                    }

                    // Assegnare il risultato serializzato al campo Data
                    ret.Data = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    return ret;
                }
            }
        }


    }
}
