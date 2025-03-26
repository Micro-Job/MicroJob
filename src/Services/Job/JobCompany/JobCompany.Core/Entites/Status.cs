using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class Status : BaseEntity
    {
        public string StatusColor { get; set; }
        public bool IsVisible { get; set; }
        public byte Order { get; set; }
        public StatusEnum StatusEnum { get; set; }

        public Guid CompanyId { get; set; }
        public Company Company { get; set; }

        public ICollection<Application>? Applications { get; set; }
    }
}