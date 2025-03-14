using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class CountryTranslation : BaseEntity
    {
        public string Name { get; set; }
        public LanguageCode Language { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }

}