using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites
{
    public class CategoryTranslation : BaseEntity
    {
        public string Name { get; set; }
        public LanguageCode Language { get; set; }

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}