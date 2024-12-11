namespace JobCompany.Core.Entites
{
    public class Skill
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<VacancySkill> Skills { get; set; }
    }
}