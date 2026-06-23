using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace AnonymizationService.Redcap
{
    public class RedcapStudyConfiguration: Entity<Guid>
    {
        public string Name {  get; private set; }
        public string Modality { get; private set; }
        public string Endpoint { get; private set; }
        public string RedcapToken { get; private set; }
        public string MpiColumnName { get; private set; }
        public Guid? MetadataFihrId { get; private set; }
        public DateTime? TrasmissionDate { get; private set; }
        public string Fields { get; private set; }

        public RedcapStudyConfiguration(Guid id, string name, string modality, string endpoint, string redcapToken, string mpiColumnName, Guid? metadataFihrID,
            DateTime? trasmissionDate, string? fields) : base(id)
        {
            Name = name;
            Modality = modality;
            Endpoint = endpoint;
            RedcapToken = redcapToken;
            MpiColumnName = mpiColumnName;
            MetadataFihrId = metadataFihrID ?? null;
            TrasmissionDate = trasmissionDate ?? null;
            Fields = fields ?? "";
        }

        private RedcapStudyConfiguration() { }

        public void SetFields(string fields)
        {
            Fields = fields;
        }

        public void SetMetadataFihrId(Guid id)
        {
            MetadataFihrId = id;
        }

        public void SetTrasmissionDate(DateTime date)
        {
            TrasmissionDate = date;
        }

        public void ResetMetadataFihrId()
        {
            MetadataFihrId = null;
        }

        public void ResetTrasmissionDate()
        {
            TrasmissionDate = null;
        }
    }
}
