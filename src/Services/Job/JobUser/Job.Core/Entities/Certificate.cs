using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Core.Entities
{
    public class Certificate : BaseEntity
    {
        public ExtraInformation ExtraInformation { get; set; }
        public Guid ExtraInformationId { get; set; }
        public string CertificateName { get; set; }
        public string Certificates { get; set; }
        public string CertificateFile { get; set; }
    }
}
