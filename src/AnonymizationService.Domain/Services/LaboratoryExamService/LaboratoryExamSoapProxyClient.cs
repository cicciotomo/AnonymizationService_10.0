using AnonymizationService.Services.Galileo;
using Microsoft.Extensions.Options;
using Quartz.Util;
using SimpleSOAPClient;
using SimpleSOAPClient.Helpers;
using SimpleSOAPClient.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Volo.Abp.DependencyInjection;

namespace AnonymizationService.Services.LaboratoryExamService
{
    public class LaboratoryExamSoapProxyClient : ITransientDependency
    {
        private readonly GalileoSettings _galileoSettings;

        public LaboratoryExamSoapProxyClient(IOptionsSnapshot<GalileoSettings> galileoOptions)
        {
            _galileoSettings = galileoOptions.Value;
        }

        internal async Task<GetLabResultResponse> GetLaboratoryExamsAsync(string masterPatientIndex)
        {
            using var client = SoapClient.Prepare();

            var getLabRequest = new GetLabResultRequest() { MPI_ID = masterPatientIndex };
            var requestEnvelope = SoapEnvelope.Prepare().Body(getLabRequest);

            SoapEnvelope responseEnvelope;
            GetLabResultResponse response = null;
            responseEnvelope = await client.SendAsync(_galileoSettings.LabResDownloadBaseUrl, _galileoSettings.LabResDownloadAction, requestEnvelope);
            response = responseEnvelope.Body<GetLabResultResponse>();
            return response;
        }

        internal async Task<GetLabResultResponse> GetLaboratoryExamsFromDateToDateAsync(string masterPatientIndex, DateTime startDate, DateTime? endDate)
        {
            using var client = SoapClient.Prepare();

            var getLabRequest = new GetLabResultRequest() { MPI_ID = masterPatientIndex, DATE_FROM = startDate };
            if (endDate.HasValue) { 
                getLabRequest.DATE_TO = endDate.Value;
            }
            var requestEnvelope = SoapEnvelope.Prepare().Body(getLabRequest);

            SoapEnvelope responseEnvelope;
            GetLabResultResponse response = null;
            responseEnvelope = await client.SendAsync(_galileoSettings.LabResDownloadBaseUrl, _galileoSettings.LabResDownloadAction, requestEnvelope);
            response = responseEnvelope.Body<GetLabResultResponse>();
            return response;
        }

    }
    #region Soap Classes

    [XmlRoot("GetLabResultRequest")]
    public class GetLabResultRequest
    {
        [XmlElement("NAME")]
        public string NAME { get; set; }

        [XmlElement("PRENAME")]
        public string PRENAME { get; set; }

        [XmlElement("SEX")]
        public string SEX { get; set; }

        [XmlElement("BIRTH_DATE")]
        public DateTime? BIRTH_DATE { get; set; }

        [XmlElement("TAXCODE")]
        public string TAXCODE { get; set; }

        [XmlElement("MPI_ID")]
        public string MPI_ID { get; set; }

        [XmlElement("ID_VISIT_HOSP")]
        public string ID_VISIT_HOSP { get; set; }

        [XmlElement("METHOD_ID")]
        public string METHOD_ID { get; set; }

        [XmlElement("ID_LAB_RESULT")]
        public long? ID_LAB_RESULT { get; set; }

        [XmlElement("DATE_FROM")]
        public DateTime? DATE_FROM { get; set; }

        [XmlElement("DATE_TO")]
        public DateTime? DATE_TO { get; set; }
    }

    [XmlRoot("getLabResultResponse", Namespace = "http://hsr.dedalus.eu")]
    public class GetLabResultResponse
    {
        [XmlElement("LabResult")]
        public LabResult[] LabResult { get; set; }
    }

    [XmlRoot("LabResult")]
    public class LabResult
    {

        [XmlElement("NAME")]
        public string NAME { get; set; }

        [XmlElement("PRENAME")]
        public string PRENAME { get; set; }

        [XmlElement("SEX")]
        public string SEX { get; set; }

        [XmlElement("BIRTH_DATE")]
        private string _BIRTH_DATE { get; set; }

        [XmlIgnore]
        public DateTime? BIRTH_DATE
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_BIRTH_DATE))
                {
                    return DateTime.Parse(_BIRTH_DATE);
                }

                return null;
            }
        }

        [XmlElement("TAXCODE")]
        public string TAXCODE { get; set; }

        [XmlElement("MPI_ID")]
        public string MPI_ID { get; set; }

        [XmlElement("ADM_TYPE")]
        public string ADM_TYPE { get; set; }

        [XmlElement("ID_VISIT_HOSP")]
        public string ID_VISIT_HOSP { get; set; }

        [XmlElement("ID_ORDER_PLACER")]
        public string ID_ORDER_PLACER { get; set; }

        [XmlElement("ID_ORDER_FILLER")]
        public string ID_ORDER_FILLER { get; set; }

        [XmlElement("DAY")]
        public string DAY_STRING { get; set; }

        [XmlElement("DAY")]
        public DateTime DAY 
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DAY_STRING))
                {
                    string dateToConvert = ID_EXAM_FILLER.Substring(0, 14);
                    return DateTime.ParseExact(dateToConvert, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                }
                return DateTime.Parse(DAY_STRING);
            }
        }

        [XmlElement("ID_EXAM_FILLER")]
        public string ID_EXAM_FILLER { get; set; }

        [XmlElement("METHOD_ID")]
        public string METHOD_ID { get; set; }

        [XmlElement("METHOD_TEXT")]
        public string METHOD_TEXT { get; set; }

        [XmlElement("ID_LAB_RESULT")]
        public long ID_LAB_RESULT { get; set; }

        [XmlElement("RESULT_VALUE")]
        public string RESULT_VALUE { get; set; }

        [XmlElement("UNIT")]
        public string UNIT { get; set; }

        [XmlElement("LAB_COMMENT")]
        public string LAB_COMMENT { get; set; }

        [XmlElement("LAST_MODIFICATION")]
        public DateTime LAST_MODIFICATION { get; set; }

        [XmlElement("REFERENCE_LOW")]
        public string REFERENCE_LOW { get; set; }

        [XmlElement("REFERENCE_HIGH")]
        public string REFERENCE_HIGH { get; set; }

        [XmlElement("REFERENCE_RANGE")]
        public string REFERENCE_RANGE { get; set; }
    }

    #endregion
}
