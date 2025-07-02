using JobCompany.Business.Dtos.CategoryDtos;
using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.CompanyDtos;
using JobCompany.Business.Dtos.MessageDtos;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.SagaStateMachine;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.Extensions;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.ManageService;

public class ManageService(JobCompanyDbContext _context, ICurrentUser _currentUser, IPublishEndpoint _publishEndpoint)
{
    #region Vacancy
    public async Task VacancyAcceptAsync(string vacancyId)
    {
        await _publishEndpoint.Publish(new VacancyAcceptEvent
        {
            vacancyId = vacancyId
        });
    }

    public async Task VacancyRejectAsync(VacancyStatusUpdateDto dto)
    {
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancyMessageGuid = Guid.Parse(dto.VacancyMessageId);

        if (!await _context.Messages.AnyAsync(x => x.Id == vacancyMessageGuid))
            throw new NotFoundException();

        var vacancy = await _context.Vacancies
            .Include(v => v.Applications)
            .Include(v => v.Company)
            .Include(v => v.VacancyMessages)
            .FirstOrDefaultAsync(v => v.Id == vacancyGuid)
            ?? throw new NotFoundException();

        var appliedUserIds = vacancy.Applications
             .Where(a => !a.IsDeleted)
             .Select(a => a.UserId)
             .ToList();

        //vacancy.VacancyCommentId = vacancyMessageGuid; TODO: burda vakansiya kommentinə niyə mesajın id-sini veririk? 
        vacancy.VacancyMessages?.Add(new VacancyMessage
        {
            MessageId = vacancyMessageGuid,
            VacancyId = vacancyGuid,
        });

        if (vacancy.VacancyStatus == VacancyStatus.Active)
        {
            ///summary
            /// Vakansiya reject olunanda bu vakansiyaya muraciet edenlere bildiris getmesi
            ///summary
            await _publishEndpoint.Publish(
                new NotificationToUserEvent
                {
                    InformationId = vacancyGuid,
                    SenderId = null,
                    ReceiverIds = appliedUserIds,
                    InformationName = vacancy.Title,
                    NotificationType = NotificationType.VacancyReject,
                    SenderName = null,
                    SenderImage = null,
                }
            );
        }

        ///summary
        /// Vakansiya reject olunanda bu vakansiyaya olunan muracietlerin isdeleted olunmasi
        ///summary
        await _context.Applications
        .Where(a => a.VacancyId == vacancyGuid)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(a => a.IsDeleted, true)
        );

        vacancy.VacancyStatus = VacancyStatus.Reject;

        var newNotification = new Notification
        {
            ReceiverId = vacancy.Company.Id,
            SenderId = null,
            NotificationType = NotificationType.VacancyReject,
            InformationId = vacancyGuid,
            InformationName = vacancy.Title,
            IsSeen = false,
        };
        await _context.Notifications.AddAsync(newNotification);
        await _context.SaveChangesAsync();
    }

    public async Task ToggleBlockVacancyStatusAsync(VacancyStatusUpdateDto dto)
    {
        var vacancy = await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == Guid.Parse(dto.VacancyId))
            ?? throw new NotFoundException();

        vacancy.VacancyStatus = VacancyStatus.Block;
        vacancy.VacancyMessages?.Add(new VacancyMessage
        {
            MessageId = Guid.Parse(dto.VacancyMessageId),
            VacancyId = vacancy.Id,
        });

        await _context.SaveChangesAsync();
    }

    public async Task<DataListDto<VacancyGetAllDto>> GetAllVacanciesAsync(string? vacancyName, string? startMinDate, string? startMaxDate, string? endMinDate, string? endMaxDate, string? companyName, byte? vacancyStatus, int skip = 1, int take = 10)
    {
        var query = _context.Vacancies
            .OrderBy(x => x.VacancyStatus).ThenBy(x => x.CreatedDate)
            .AsNoTracking();

        if (vacancyStatus != null)
            query = query.Where(x => x.VacancyStatus == (VacancyStatus)vacancyStatus);

        if (startMinDate != null)
        {
            DateTime? Min = startMinDate.ToNullableDateTime();
            if (Min != null)
                query = query.Where(x => x.StartDate >= Min);
        }

        if (startMaxDate != null)
        {
            DateTime? Max = startMaxDate.ToNullableDateTime();
            if (Max != null)
                query = query.Where(x => x.StartDate <= Max);
        }

        if (endMinDate != null)
        {
            DateTime? Min = endMinDate.ToNullableDateTime();
            if (Min != null)
                query = query.Where(x => x.EndDate >= Min);
        }

        if (endMaxDate != null)
        {
            DateTime? Max = endMaxDate.ToNullableDateTime();
            if (Max != null)
                query = query.Where(x => x.EndDate <= Max);
        }

        if (companyName != null)
            query = query.Where(x => x.CompanyName.Contains(companyName));

        if (vacancyName != null)
            query = query.Where(x => x.Title.Contains(vacancyName));

        var data = await query.Select(x => new VacancyGetAllDto
        {
            Id = x.Id,
            Title = x.Title,
            VacancyStatus = x.VacancyStatus,
            CompanyName = x.CompanyName,
            CityName = x.City.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name),
            CountryName = x.Country.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name),
            ViewCount = x.ViewCount,
            MainSalary = x.MainSalary,
            MaxSalary = x.MaxSalary,
            SalaryCurrency = x.SalaryCurrency,
            WorkStyle = x.WorkStyle,
            WorkType = x.WorkType,
            //bu yaradilma tarixidir(normalda baslama tarixi olur)
            StartDate = x.CreatedDate,
            EndDate = x.EndDate, 
            CompanyLogo = x.CompanyLogo != null ? $"{_currentUser.BaseUrl}/company/{x.CompanyLogo}" : null
        })
        .Skip((skip - 1) * take)
        .Take(take)
        .ToListAsync();

        return new DataListDto<VacancyGetAllDto>
        {
            Datas = data,
            TotalCount = await query.CountAsync()
        };
    }

    public async Task<VacancyGetByIdDto> GetByIdVacancyAsync(Guid vacancyGuid)
    {
        var vacancyDto = await _context.Vacancies
            .Where(x => x.Id == vacancyGuid)
                .Include(x => x.Category)
                    .ThenInclude(x => x.Translations)
                .Include(x => x.VacancyMessages)
                    .ThenInclude(c => c.Message)
                        .ThenInclude(x => x.Translations)
                .Select(x => new VacancyGetByIdDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    CompanyId = x.CompanyId,
                    CompanyLogo = $"{_currentUser.BaseUrl}/company/{x.Company.CompanyLogo}",
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Location = x.Location,
                    ViewCount = x.ViewCount,
                    WorkType = x.WorkType,
                    WorkStyle = x.WorkStyle,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                    SalaryCurrency = x.SalaryCurrency,
                    Requirement = x.Requirement,
                    Description = x.Description,
                    Email = x.Email,
                    Gender = x.Gender,
                    Military = x.Military,
                    Family = x.Family,
                    Driver = x.Driver,
                    Citizenship = x.Citizenship,
                    ExamId = x.ExamId,
                    VacancyNumbers = x
                        .VacancyNumbers.Select(vn => new VacancyNumberDto
                        {
                            Id = vn.Id,
                            VacancyNumber = vn.Number,
                        })
                        .ToList(),
                    CompanyName = x.CompanyName,
                    CategoryName = x.Category.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name),
                    CompanyUserId = x.Company.UserId,
                    //Messages = _currentUser.UserGuid == x.Company.UserId
                    //    ? x.VacancyMessages.Select(vm => vm.Message.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Content)).ToList()
                    //    : null,
                    VacancyStatus = x.VacancyStatus
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException();

        return vacancyDto;
    }

    #endregion

    #region Message
    public async Task<DataListDto<MessageWithTranslationsDto>> GetAllMessagesAsync(string? content, int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Messages.AsQueryable();

        if (content != null)
            query = query.Where(x => x.Translations.Any(z => z.Content.Contains(content)));

        var totalCount = await query.CountAsync();

        var messageDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new MessageWithTranslationsDto
            {
                Id = m.Id,
                CreatedDate = m.CreatedDate,
                Translations = m.Translations.Select(t => new MessageTranslationDto
                {
                    Language = t.Language,
                    Content = t.Content
                }).ToList()
            })
            .ToListAsync();

        return new DataListDto<MessageWithTranslationsDto>
        {
            Datas = messageDtos,
            TotalCount = totalCount
        };
    }

    /// <summary> </summary>
    public async Task<List<MessageSelectDto>> GetAllMessagesForSelectAsync()
    {
        var messages = await _context.Messages
            .AsNoTracking()
            .Include(m => m.Translations)
            .Select(m => new MessageSelectDto
            {
                Id = m.Id,
                Content = m.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Content)
            }).ToListAsync();

        return messages;
    }

    public async Task<MessageDto> GetMessageByIdAsync(string id)
    {
        var messageGuid = Guid.Parse(id);
        var message = await _context.Messages
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == messageGuid)
            ?? throw new NotFoundException();

        return new MessageDto
        {
            Id = message.Id,
            CreatedDate = message.CreatedDate,
            Content = message.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Content)
        };
    }

    public async Task<MessageWithTranslationsDto> CreateMessageAsync(CreateMessageDto dto)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.Now,
            Translations = dto.Translations.Select(t => new MessageTranslation
            {
                Content = t.Content,
                Language = t.Language
            }).ToList()
        };

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        return new MessageWithTranslationsDto
        {
            Id = message.Id,
            CreatedDate = message.CreatedDate,
            Translations = message.Translations.Select(t => new MessageTranslationDto
            {
                Language = t.Language,
                Content = t.Content
            }).ToList()
        };
    }

    public async Task UpdateMessageAsync(string id, UpdateMessageDto dto)
    {
        var messageGuid = Guid.Parse(id);
        var message = await _context.Messages
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == messageGuid)
            ?? throw new NotFoundException();

        foreach (var dtoTranslation in dto.Translations)
        {
            var existingTranslation = message.Translations
                .FirstOrDefault(t => t.Language == dtoTranslation.Language);

            if (existingTranslation != null) // Mövcud olan tərcüməni yeniləyirik
            {
                existingTranslation.Content = dtoTranslation.Content;
            }
            else // Yeni tərcümə əlavə edirik
            {
                message.Translations.Add(new MessageTranslation
                {
                    MessageId = messageGuid,
                    Language = dtoTranslation.Language,
                    Content = dtoTranslation.Content
                });
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(string id)
    {
        var messageGuid = Guid.Parse(id);
        var message = await _context.Messages
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == messageGuid)
            ?? throw new NotFoundException();

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
    }
    #endregion

    #region Company
    public async Task<CompanyProfileDto> GetCompanyDetailsAsync(string companyUserId)
    {
        var currentLanguage = _currentUser.LanguageCode;
        var companyUserGuid = Guid.Parse(companyUserId);

        var companyProfile = await _context.Companies
            .Where(c => c.UserId == companyUserGuid)
            .Include(x => x.Category.Translations)
            .Include(x => x.City.Translations)
            .Include(x => x.Country.Translations)
            .Select(x => new CompanyProfileDto
            {
                Id = x.Id,
                Name = x.CompanyName,
                Information = x.CompanyInformation,
                Location = x.CompanyLocation,
                WebLink = x.WebLink,
                CreatedDate = x.CreatedDate,
                EmployeeCount = x.EmployeeCount,
                CompanyLogo = $"{_currentUser.BaseUrl}/company/{x.CompanyLogo}",
                Category = x.Category.Translations.GetTranslation(currentLanguage, GetTranslationPropertyName.Name),
                City = x.City != null ? x.City.Translations.GetTranslation(currentLanguage, GetTranslationPropertyName.Name) : null,
                Country = x.Country != null ? x.Country.Translations.GetTranslation(currentLanguage, GetTranslationPropertyName.Name) : null,
                CategoryId = x.CategoryId,
                CityId = x.CityId,
                CountryId = x.CountryId,
                Email = x.Email,
                CompanyNumbers = x.CompanyNumbers != null
                    ? x.CompanyNumbers.Select(cn => new CompanyNumberDto
                    {
                        Id = cn.Id,
                        Number = cn.Number,
                    }).ToList()
                    : new List<CompanyNumberDto>()
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundException();

        return companyProfile;
    }

    public async Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyUserIdAsync(string companyUserId, int skip = 1, int take = 9)
    {
        var companyUserGuid = Guid.Parse(companyUserId);

        var query = _context.Vacancies.Where(x => x.Company.UserId == companyUserGuid).AsNoTracking();

        var vacancies = await query
            .Skip(Math.Max(0, (skip - 1) * take))
            .Take(take)
            .Select(x => new VacancyGetByCompanyIdDto
            {
                Id = x.Id,
                CompanyName = x.CompanyName,
                Title = x.Title,
                Location = x.Location,
                CompanyLogo = $"{_currentUser.BaseUrl}/company/{x.Company.CompanyLogo}",
                WorkStyle = x.WorkStyle,
                WorkType = x.WorkType,
                StartDate = x.StartDate,
                ViewCount = x.ViewCount,
                MainSalary = x.MainSalary,
                MaxSalary = x.MaxSalary,
                Status = x.VacancyStatus
            })
            .ToListAsync();

        return new DataListDto<VacancyGetByCompanyIdDto>
        {
            Datas = vacancies,
            TotalCount = await query.CountAsync()
        };
    }
    #endregion

    #region Category
    public async Task<DataListDto<CategoryWithTranslationsDto>> GetAllCategoriesAsync(string? content , int pageNumber = 1, int pageSize = 10)
    {
        var query = _context.Categories.Include(x=> x.Translations).AsQueryable();

        if (content != null)
            query = query.Where(x=> x.Translations.Any(x=> x.Name.Contains(content)));

        var totalCount = await query.CountAsync();

        var categoryDtos = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new CategoryWithTranslationsDto
            {
                Id = m.Id,
                IsCompany = m.IsCompany,
                Translations = m.Translations.Select(t => new CategoryTranslationDto
                {
                    Language = t.Language,
                    Name = t.Name
                }).ToList()
            }).ToListAsync();

        return new DataListDto<CategoryWithTranslationsDto>
        {
            Datas = categoryDtos,
            TotalCount = totalCount
        };
    }

    public async Task<CategoryDto> GetCategoryByIdAsync(string id)
    {
        var categoryGuid = Guid.Parse(id);
        var category = await _context.Categories
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == categoryGuid)
            ?? throw new NotFoundException();

        return new CategoryDto
        {
            Id = category.Id,
            IsCompany = category.IsCompany,
            Name = category.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
        };
    }

    public async Task CreateCategoryAsync(CategoryCreateDto dto)
    {
        var category = new Category
        {
            IsCompany = false,
            Translations = dto.Categories.Select(t => new CategoryTranslation
            {
                Name = t.Name,
                Language = t.language
            }).ToList()
        };
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(string id, List<CategoryTranslationDto> categories)
    {
        var categoryGuid = Guid.Parse(id);
        var category = await _context.Categories
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == categoryGuid)
            ?? throw new NotFoundException();
        foreach (var dtoTranslation in categories)
        {
            var existingTranslation = category.Translations
                .FirstOrDefault(t => t.Language == dtoTranslation.Language);
            if (existingTranslation != null) // Mövcud olan tərcüməni yeniləyirik
            {
                existingTranslation.Name = dtoTranslation.Name;
            }
            else // Yeni tərcümə əlavə edirik
            {
                category.Translations.Add(new CategoryTranslation
                {
                    CategoryId = categoryGuid,
                    Language = dtoTranslation.Language,
                    Name = dtoTranslation.Name
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(string id)
    {
        var categoryGuid = Guid.Parse(id);
        var category = await _context.Categories
            .Include(m => m.Translations)
            .FirstOrDefaultAsync(m => m.Id == categoryGuid)
            ?? throw new NotFoundException();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
    #endregion
}
