using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class StatusTranslation : BaseEntity
    {
        public string Name { get; set; }

        public LanguageCode Language { get; set; }

        public Guid StatusId { get; set; }
        public Status Status { get; set; }

    }
}