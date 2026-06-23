using System.Collections.Generic;

namespace AnonymizationService.Services.Galileo
{
    public class PatientDocumentsRequest
    {
        public string callingApplication { get; set; }
        public string token { get; set; }
        public string id_mpi { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string userId { get; set; }
    }

    public class PatientDocument
    {
        public int episodeId { get; set; }
        public string episodeType { get; set; }
        public string episodeStartDate { get; set; }
        public string episodeEndDate { get; set; }
        public List<Order> orders { get; set; }
        public List<EpisodeDocument> episodeDocuments { get; set; }
    }
    public class Order
    {
        public int orderId { get; set; }
        public string orderDate { get; set; }
        public string orderPriority { get; set; }
        public string department { get; set; }
        public string filler { get; set; }
        public List<Asset> assets { get; set; }
        public List<Document> documents { get; set; }
        public List<Study> studies { get; set; }
    }

    public class Asset
    {
        public int assetId { get; set; }
        public string code { get; set; }
        public string description { get; set; }
    }
    public class Document
    {
        public int documentId { get; set; }
        public string description { get; set; }
        public string creationDate { get; set; }
        public string mimeType { get; set; }
        public bool? confidential { get; set; }
        public bool restricted { get; set; }
    }

    public class Study
    {
        public object studyId { get; set; }
        public string accessionNumber { get; set; }
    }
    public class EpisodeDocument
    {
        public int documentId { get; set; }
        public string description { get; set; }
        public string creationDate { get; set; }
        public string mimeType { get; set; }
        public bool? confidential { get; set; }
        public bool restricted { get; set; }
    }

    public class PatientDocumentFile
    {
        public int documentId { get; set; }
        public string mimeType { get; set; }
        public string url { get; set; }
        public string content { get; set; }
    }

    public class PatientFileRequest
    {
        public string userId { get; set; }
        public string token { get; set; }
        public int documentId { get; set; }
    }
}
