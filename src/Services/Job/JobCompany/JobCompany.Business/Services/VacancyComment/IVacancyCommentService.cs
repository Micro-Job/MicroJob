using JobCompany.Business.Dtos.VacancyComment;

namespace JobCompany.Business.Services.VacancyComment;

public interface IVacancyCommentService
{
    Task<ICollection<VacancyCommentListDto>> VacancyCommentGetAllAsync();
    Task<VacancyCommentDetailDto> VacancyCommentGetDetailAsync(string id);
    Task VacancyCommentCreateAsync(VacancyCommentCreateDto dto);
    Task VacancyCommentUpdateAsync(List<VacancyCommentUpdateDto> dtos);
    Task VacancyCommentDeleteAsync(string id);
}
