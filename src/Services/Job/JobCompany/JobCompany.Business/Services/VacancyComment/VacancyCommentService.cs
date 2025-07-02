using JobCompany.Business.Dtos.VacancyComment;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.VacancyComment;

public class VacancyCommentService(JobCompanyDbContext _context, ICurrentUser _user)
{
    public async Task VacancyCommentCreateAsync(VacancyCommentCreateDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Core.Entites.VacancyComment vacancyComment = new()
            {
                CommentType = dto.CommentType
            };

            await _context.VacancyComments.AddAsync(vacancyComment);
            await _context.SaveChangesAsync();

            var vacancyCommentTranslations = dto.VacancyComments.Select(x => new Core.Entites.VacancyCommentTranslation
            {
                VacancyCommentId = vacancyComment.Id,
                Language = x.language,
                Comment = x.Comment.Trim()
            }).ToList();

            await _context.VacancyCommentTranslations.AddRangeAsync(vacancyCommentTranslations);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task VacancyCommentDeleteAsync(Guid id)
    {
        var noticeComment = await _context.VacancyComments.Include(x => x.Translations).Where(x => x.Id == id).FirstOrDefaultAsync();
        var noticeCommentTranslation = noticeComment.Translations.ToList();
        _context.VacancyCommentTranslations.RemoveRange(noticeCommentTranslation);
        _context.VacancyComments.Remove(noticeComment);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<VacancyCommentListDto>> VacancyCommentGetAllAsync()
    {
        var vacancyComments = await _context.VacancyComments.Include(x=> x.Translations)
        .Select(b => new VacancyCommentListDto
        {
            Id = b.Id.ToString(),
            Comment = b.Translations.GetTranslation(_user.LanguageCode, GetTranslationPropertyName.Comment)
        })
        .ToListAsync();

        return vacancyComments!;
    }

    public async Task<VacancyCommentDetailDto> VacancyCommentGetDetailAsync(Guid id)
    {
        var res = await _context.VacancyComments
          .Where(x => x.Id == id)
          .Include(x => x.Translations)
          .Select(x => new VacancyCommentDetailDto
          {
              Id = x.Id,
              VacancyCommentTranslations = x.Translations.Select(t => new VacancyCommentTranslationDetailDto
              {
                  Id = t.Id,
                  Comment = t.Comment,
                  LanguageCode = t.Language
              }).ToList()
          })
          .FirstOrDefaultAsync();

        return res;
    }

    public async Task VacancyCommentUpdateAsync(List<VacancyCommentUpdateDto> dtos)
    {
        var vacancyCommentTranslations = await _context.VacancyCommentTranslations
            .Where(x => dtos.Select(b => b.Id).Contains(x.Id))
            .ToListAsync();

        foreach (var translation in vacancyCommentTranslations)
        {
            var vacancyComment = dtos.FirstOrDefault(b => b.Id == translation.Id);
            if (vacancyComment != null)
            {
                translation.Comment = vacancyComment.Comment;
            }
        }

        await _context.SaveChangesAsync();
    }
}
