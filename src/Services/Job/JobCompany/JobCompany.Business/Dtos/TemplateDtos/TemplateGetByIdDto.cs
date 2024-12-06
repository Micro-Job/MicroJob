using JobCompany.Core.Entites;

namespace JobCompany.Business.Dtos.TemplateDtos;

public class TemplateGetByIdDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    //TODO: Exam əvəzinə ExamListDto əlavə edilməlidir
    public ICollection<Exam>? Exams { get; set; }
}
