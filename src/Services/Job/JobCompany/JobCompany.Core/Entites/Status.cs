using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Status : BaseEntity
    {
        public string StatusName { get; set; }
        public string StatusColor { get; set; }
        public bool IsDefault { get; set; }
        public int Order { get; set; }
        public Guid CompanyId { get; set; }
        public Company Company { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}