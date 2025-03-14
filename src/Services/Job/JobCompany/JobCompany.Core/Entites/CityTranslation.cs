using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class CityTranslation : BaseEntity
    {
        public string Name { get; set; }

        public LanguageCode Language { get; set; }

        public Guid CityId { get; set; } 
        public City City { get; set; }
    }
}