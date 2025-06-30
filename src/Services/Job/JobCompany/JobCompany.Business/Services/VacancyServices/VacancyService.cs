using JobCompany.Business.Dtos.Common;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.SkillDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Business.Extensions;
using JobCompany.Business.Statistics;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using SharedLibrary.Statics;
using System.Data;

namespace JobCompany.Business.Services.VacancyServices
{
    public class VacancyService(JobCompanyDbContext _context, IFileService _fileService, IPublishEndpoint _publishEndpoint, ICurrentUser _currentUser, IRequestClient<CheckBalanceRequest> _checkBalanceRequest)
    {
        /// <summary> vacancy yaradılması </summary>
        /// vacancy yaradilan zaman exam yaradılması
        public async Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDto)
        {
            string? companyLogoPath = null;
            var company = await _context.Companies.Where(x => x.UserId == _currentUser.UserGuid).Select(x => new
            {
                x.Id,
                x.CompanyName,
                x.CompanyLogo
            }).FirstOrDefaultAsync();

            if (company != null && !string.IsNullOrEmpty(company.CompanyLogo))
            {
                companyLogoPath = company.CompanyLogo;
            }
            else if (vacancyDto.CompanyLogo != null)
            {
                FileDto fileResult = await _fileService.UploadAsync(FilePaths.image, vacancyDto.CompanyLogo);
                companyLogoPath = $"{fileResult.FilePath}/{fileResult.FileName}";
            }

            var vacancy = new Vacancy
            {
                Id = Guid.NewGuid(),
                CompanyName = _currentUser.UserRole == (byte)UserRole.CompanyUser ? company.CompanyName : vacancyDto.CompanyName.Trim(),
                CompanyId = company?.Id,
                Title = vacancyDto.Title.Trim(),
                CompanyLogo = companyLogoPath,
                PaymentDate = vacancyDto.StartDate,
                StartDate = vacancyDto.StartDate,
                EndDate = vacancyDto.EndDate,
                CreatedDate = DateTime.Now,
                Location = vacancyDto.Location,
                CountryId = vacancyDto.CountryId,
                CityId = vacancyDto.CityId,
                Email = vacancyDto.Email,
                WorkType = vacancyDto.WorkType,
                WorkStyle = vacancyDto.WorkStyle,
                MainSalary = vacancyDto.MainSalary,
                MaxSalary = vacancyDto.MaxSalary,
                Requirement = vacancyDto.Requirement,
                Description = vacancyDto.Description,
                Gender = vacancyDto.Gender,
                Military = vacancyDto.Military,
                Driver = vacancyDto.Driver,
                Family = vacancyDto.Family,
                Citizenship = vacancyDto.Citizenship,
                CategoryId = vacancyDto.CategoryId,
                ExamId = vacancyDto.ExamId,
                VacancyStatus = VacancyStatus.Pending,
                ViewCount = 0,
                SalaryCurrency = vacancyDto.SalaryCurrency
            };

            var numbers = new List<VacancyNumber>();
            if (numberDto != null)
            {
                numbers = numberDto
                    .Select(numberCreateDto => new VacancyNumber
                    {
                        Number = numberCreateDto.PhoneNumber,
                        VacancyId = vacancy.Id,
                    })
                    .ToList();

                await _context.VacancyNumbers.AddRangeAsync(numbers);
                vacancy.VacancyNumbers = numbers;
            }

            var vacancySkills =
                vacancyDto.SkillIds != null
                    ? vacancyDto
                        .SkillIds.Select(skillId => new VacancySkill
                        {
                            SkillId = skillId,
                            VacancyId = vacancy.Id,
                        })
                        .ToList()
                    : [];
            vacancy.VacancySkills = vacancySkills;

            await _context.Vacancies.AddAsync(vacancy);
            await _context.SaveChangesAsync();

            //TODO : bu skillere uygun mesajlarin getmesi ucundur buna baxmaq lazimdir islemirdi en son
            //if (vacancyDto.SkillIds != null)
            //{
            //    await _publishEndpoint.Publish(
            //        new VacancyCreatedEvent
            //        {
            //            SenderId = (Guid)_currentUser.UserGuid,
            //            SkillIds = vacancyDto.SkillIds,
            //            InformationId = vacancy.Id,
            //            InformatioName = vacancy.Title,
            //        }
            //    );
            //}
        }

        public async Task DeleteAsync(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException(MessageHelper.GetMessage("NOT_EMPTY"));
            var vacancyGuids = ids.Select(id =>
                {
                    if (Guid.TryParse(id, out var guid))
                        return guid;
                    throw new FormatException(MessageHelper.GetMessage("INVALID_FORMAT"));
                })
                .ToList();

            var deletedCount = await _context
                .Vacancies.Where(x => vacancyGuids.Contains(x.Id) && x.Company.UserId == _currentUser.UserGuid)
                .ExecuteUpdateAsync(x => x.SetProperty(v => v.VacancyStatus, VacancyStatus.Deactive));

            if (deletedCount == 0)
                throw new NotFoundException();
        }

        /// <summary> Şirkətin profilində bütün vakansiyalarını gətirmək(Filterlerle birlikde) </summary>
        public async Task<DataListDto<VacancyGetAllDto>> GetAllOwnVacanciesAsync(string? titleName, List<Guid>? categoryIds, List<Guid>? countryIds, List<Guid>? cityIds, VacancyStatus? IsActive, decimal? minSalary, decimal? maxSalary, List<byte>? workStyles, List<byte>? workTypes, List<Guid>? skillIds, int skip = 1, int take = 6)
        {
            var query = _context
                .Vacancies.Where(x => x.Company.UserId == _currentUser.UserGuid && x.VacancyStatus != VacancyStatus.Deleted)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            query = ApplyVacancyFilters(
                query,
                titleName,
                categoryIds,
                countryIds,
                cityIds,
                IsActive,
                minSalary,
                maxSalary,
                null,
                workTypes,
                workStyles,
                skillIds
            );

            var vacancies = await query
                .Select(x => new VacancyGetAllDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    StartDate = x.StartDate,
                    Location = x.Location,
                    CompanyLogo = $"{_currentUser.BaseUrl}/companyFiles/{x.Company.CompanyLogo}",
                    CompanyName = x.Company.IsCompany ? x.Company.CompanyName : x.CompanyName,
                    ViewCount = x.ViewCount,
                    WorkType = x.WorkType,
                    WorkStyle = x.WorkStyle,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                    VacancyStatus = x.VacancyStatus,
                    SalaryCurrency = x.SalaryCurrency
                })
                .Skip((skip - 1) * take)
                .Take(take)
                .ToListAsync();

            return new DataListDto<VacancyGetAllDto>
            {
                Datas = vacancies,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary> şirkət id'sinə görə vacanciyaların gətirilməsi </summary>
        public async Task<DataListDto<VacancyGetByCompanyIdDto>> GetVacanciesByCompanyIdAsync(Guid companyId, Guid? vacancyId, int skip = 1, int take = 9)
        {
            var query = _context.Vacancies
                .Where(x => x.CompanyId == companyId &&
                            x.VacancyStatus == VacancyStatus.Active &&
                            x.EndDate >= DateTime.Now)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            if (vacancyId != null)
                query = query.Where(x => x.Id != vacancyId);

            var vacancies = await query
                .Select(x => new VacancyGetByCompanyIdDto
                {
                    VacancyId = x.Id,
                    CompanyName = x.Company.IsCompany ? x.Company.CompanyName : x.CompanyName,
                    Title = x.Title,
                    Location = x.Location,
                    CompanyLogo = $"{_currentUser.BaseUrl}/companyFiles/{x.Company.CompanyLogo}",
                    WorkStyle = x.WorkStyle,
                    WorkType = x.WorkType,
                    StartDate = x.StartDate,
                    ViewCount = x.ViewCount,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                    IsSaved = x.SavedVacancies.Any(sv => sv.UserId == _currentUser.UserGuid && sv.VacancyId == x.Id),
                    SalaryCurrency = x.SalaryCurrency
                })
                .Skip((skip - 1) * take)
                .Take(take)
                .ToListAsync();

            return new DataListDto<VacancyGetByCompanyIdDto>
            {
                Datas = vacancies,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary> vacanciya id'sinə görə vacancyın gətirilməsi </summary>
        public async Task<VacancyGetByIdDto> GetByIdVacancyAsync(Guid vacancyId)
        {
            Guid? userGuid = _currentUser.UserGuid;

            var vacancyDto = await _context.Vacancies
                .Where(x => x.Id == vacancyId)
                    .Include(x => x.Applications)
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
                        CompanyLogo = $"{_currentUser.BaseUrl}/companyFiles/{x.Company.CompanyLogo}",
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
                        IsSaved = userGuid != null ? x.SavedVacancies.Any(y => y.UserId == userGuid && y.VacancyId == vacancyId) : false,
                        VacancyNumbers = x
                            .VacancyNumbers.Select(vn => new VacancyNumberDto
                            {
                                Id = vn.Id,
                                VacancyNumber = vn.Number,
                            })
                            .ToList(),
                        //Skills = x.VacancySkills
                        //        .Where(vc => vc.Skill != null)
                        //        .Select(vc => new SkillDto
                        //        {
                        //            Id = vc.Skill.Id,
                        //            Name = vc.Skill.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
                        //        }).ToList(),
                        CompanyName = x.Company.IsCompany ? x.Company.CompanyName : x.CompanyName,
                        CategoryName = x.Category.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name),
                        CompanyUserId = x.Company.UserId,
                        Messages = _currentUser.UserGuid == x.Company.UserId
                            ? x.VacancyMessages.Select(vm => vm.Message.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Content)).ToList()
                            : null,
                        VacancyStatus = x.VacancyStatus,
                        IsApplied = userGuid.HasValue && x.Applications.Any(a => a.UserId == userGuid && a.IsActive == true),
                        //ApplicationId = userGuid.HasValue ? x.Applications.Where(a=> a.UserId == userGuid && a.VacancyId == x.Id && a.).Select(a=> a.Id).FirstOrDefault(): null
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException();


            if (vacancyDto.CompanyUserId != userGuid)
            {
                if (vacancyDto.EndDate < DateTime.Now && vacancyDto.VacancyStatus != VacancyStatus.Active)
                    throw new NotFoundException();

                var existVacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyId);
                existVacancy.ViewCount++;
                await _context.SaveChangesAsync();
            }

            return vacancyDto;
        }

        /// <summary> Vakansiyanın bütün detallarını gətirir </summary>
        public async Task<VacancyDetailsDto> GetVacancyDetailsAsync(Guid id)
        {
            var vacancy = await _context.Vacancies
                .Where(x => x.Id == id && x.Company.UserId == _currentUser.UserGuid)
                .Include(x => x.Country).ThenInclude(c => c.Translations)
                .Include(x => x.City).ThenInclude(c => c.Translations)
                .Include(x => x.Category).ThenInclude(c => c.Translations)
                .Include(v => v.VacancySkills)
                            .ThenInclude(vs => vs.Skill)
                                .ThenInclude(s => s.Translations)
                .Select(v => new VacancyDetailsDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    CompanyId = v.CompanyId,
                    CompanyName = v.Company.IsCompany ? v.Company.CompanyName : v.CompanyName,
                    CompanyUserId = v.Company.UserId,
                    CompanyLogo = $"{_currentUser.BaseUrl}/companyFiles/{v.Company.CompanyLogo}",
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Location = v.Location,
                    ViewCount = v.ViewCount,
                    WorkType = v.WorkType,
                    WorkStyle = v.WorkStyle,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                    SalaryCurrency = v.SalaryCurrency,
                    Requirement = v.Requirement,
                    Description = v.Description,
                    Email = v.Email,
                    Gender = v.Gender,
                    Military = v.Military,
                    Family = v.Family,
                    Driver = v.Driver,
                    Citizenship = v.Citizenship,
                    ExamId = v.ExamId,
                    CreatedDate = v.CreatedDate,
                    VacancyStatus = v.VacancyStatus,
                    CategoryId = v.CategoryId,
                    CategoryName = v.Category != null ? v.Category.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) : null,
                    CityId = v.CityId,
                    CityName = v.City != null ? v.City.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) : null,
                    CountryId = v.CountryId,
                    CountryName = v.Country != null ? v.Country.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name) : null,

                    VacancyNumbers = v.VacancyNumbers != null ? v
                                .VacancyNumbers.Select(vn => new VacancyNumberDto
                                {
                                    Id = vn.Id,
                                    VacancyNumber = vn.Number,
                                }).ToList() : null,

                    Skills = v.VacancySkills
                                .Where(vc => vc.Skill != null)
                                .Select(vc => new SkillDto
                                {
                                    Id = vc.Skill.Id,
                                    Name = vc.Skill.Translations.GetTranslation(_currentUser.LanguageCode, GetTranslationPropertyName.Name)
                                }).ToList()
                }).FirstOrDefaultAsync() ?? throw new NotFoundException();

            return vacancy;
        }


        /// <summary> vacancynin update olunması usere notification </summary>
        public async Task UpdateVacancyAsync(UpdateVacancyDto vacancyDto, ICollection<UpdateNumberDto>? numberDtos)
        {
            var vacancyGuid = Guid.Parse(vacancyDto.Id);
            var existingVacancy =
                await _context
                    .Vacancies.Where(v => v.Id == vacancyGuid && v.Company.UserId == _currentUser.UserGuid)
                    .Include(v => v.VacancySkills)
                    .Include(v => v.VacancyNumbers)
                    .FirstOrDefaultAsync() ?? throw new NotFoundException();

            if (existingVacancy.VacancyStatus == VacancyStatus.Block)
                throw new VacancyUpdateException();

            existingVacancy.CompanyId = Guid.Parse(vacancyDto.CompanyId);
            existingVacancy.CompanyName = vacancyDto.CompanyName;
            existingVacancy.Title = vacancyDto.Title;
            existingVacancy.PaymentDate = vacancyDto.EndDate;
            existingVacancy.StartDate = vacancyDto.StartDate;
            existingVacancy.EndDate = vacancyDto.EndDate;
            existingVacancy.Location = vacancyDto.Location;
            existingVacancy.CountryId = Guid.Parse(
                vacancyDto.CountryId ?? throw new NotFoundException()
            );
            existingVacancy.CityId = Guid.Parse(vacancyDto.CityId ?? throw new NotFoundException());
            existingVacancy.Email = vacancyDto.Email;
            existingVacancy.ExamId = vacancyDto.ExamId;
            existingVacancy.WorkType = vacancyDto.WorkType;
            existingVacancy.WorkStyle = vacancyDto.WorkStyle;
            existingVacancy.MainSalary = vacancyDto.MainSalary;
            existingVacancy.MaxSalary = vacancyDto.MaxSalary;
            existingVacancy.SalaryCurrency = vacancyDto.SalaryCurrency;
            existingVacancy.Requirement = vacancyDto.Requirement;
            existingVacancy.Description = vacancyDto.Description;
            existingVacancy.Gender = vacancyDto.Gender;
            existingVacancy.Military = vacancyDto.Military;
            existingVacancy.Driver = vacancyDto.Driver;
            existingVacancy.Family = vacancyDto.Family;
            existingVacancy.Citizenship = vacancyDto.Citizenship;
            existingVacancy.VacancyStatus = existingVacancy.VacancyStatus == VacancyStatus.Pending ? VacancyStatus.Pending : VacancyStatus.Update;
            existingVacancy.CategoryId = Guid.Parse(
                vacancyDto.CategoryId ?? throw new NotFoundException()
            );

            if (numberDtos is not null)
            {
                var existingNumbers = existingVacancy.VacancyNumbers ?? [];
                var existingNumberDict = existingNumbers.ToDictionary(n => n.Id, n => n);

                var incomingNumberDict = numberDtos
                    .Where(n => !string.IsNullOrWhiteSpace(n.Id))
                    .ToDictionary(n => Guid.Parse(n.Id!), n => n.PhoneNumber);

                // Id-si olan nömrələri güncəlləyirik
                foreach (var kvp in incomingNumberDict)
                {
                    if (existingNumberDict.TryGetValue(kvp.Key, out var existingNumber))
                    {
                        if (existingNumber.Number != kvp.Value)
                            existingNumber.Number = kvp.Value;
                    }
                }

                // Müqayisə edib silinəcək nömrələri tapırıq
                var toRemove = existingNumbers
                    .Where(n => !incomingNumberDict.ContainsKey(n.Id))
                    .ToList();

                if (toRemove.Count != 0)
                {
                    _context.VacancyNumbers.RemoveRange(toRemove);
                }

                // Id-si olmayan yeni nömrələri əlavə et
                var newNumbers = numberDtos
                    .Where(n => string.IsNullOrWhiteSpace(n.Id))
                    .Select(n => new VacancyNumber
                    {
                        Number = n.PhoneNumber,
                        VacancyId = existingVacancy.Id
                    }).ToList();

                if (newNumbers.Count != 0)
                {
                    await _context.VacancyNumbers.AddRangeAsync(newNumbers);
                }
            }



            if (vacancyDto.Skills is not null)
            {
                var existingSkillIds = existingVacancy.VacancySkills.Select(vs => vs.SkillId).ToList(); // Mövcud olan skill id-ləri
                var incomingSkillIds = vacancyDto.Skills.Select(s => s.Id).ToList(); // Request-də gələn skill id-ləri

                var skillsToAdd = incomingSkillIds.Except(existingSkillIds).ToList(); // Müqayisə edib əlavə olunacaq skill id-ləri tapırıq

                var newVacancySkills = skillsToAdd.Select(skillId => new VacancySkill
                {
                    VacancyId = existingVacancy.Id,
                    SkillId = skillId
                }).ToList();

                if (newVacancySkills.Count != 0)
                {
                    await _context.VacancySkills.AddRangeAsync(newVacancySkills);  // Yeni skilllər əlavə olunur
                }


                var skillsToRemove = existingSkillIds.Except(incomingSkillIds).ToList(); // Müqayisə edib silinəcək skill id-ləri tapırıq
                if (skillsToRemove.Count != 0)
                {
                    var vacancySkillsToRemove = existingVacancy.VacancySkills
                        .Where(vs => skillsToRemove.Contains(vs.SkillId))
                        .ToList();

                    _context.VacancySkills.RemoveRange(vacancySkillsToRemove);  // skillər silinir
                }
            }


            await _context.SaveChangesAsync();

            var userIds = await _context
                .Applications.Where(a => a.VacancyId == vacancyGuid && a.Status.StatusEnum != StatusEnum.Rejected)
                .Select(a => a.UserId)
                .ToListAsync();

            await _publishEndpoint.Publish(  //Vakansiya update olunduqda müraciət edən userlərə notification göndərilir
                new NotificationToUserEvent
                {
                    InformationId = vacancyGuid,
                    SenderId = (Guid)_currentUser.UserGuid,
                    ReceiverIds = userIds,
                    InformationName = existingVacancy.Title,
                    NotificationType = NotificationType.VacancyUpdate,
                    SenderName = existingVacancy.CompanyName,
                    //TODO : burada baseUrl gonderilmeli deyil 
                    SenderImage = $"{_currentUser.BaseUrl}/{existingVacancy.CompanyLogo}",
                }
            );
        }

        /// <summary> Şirkət profilində vakansiya axtarışı vakansiya filterlere görə </summary>
        public async Task<DataListDto<VacancyGetAllDto>> GetAllVacanciesAsync(string? titleName, List<Guid>? categoryIds, List<Guid>? countryIds, List<Guid>? cityIds, decimal? minSalary, decimal? maxSalary, List<Guid>? companyIds, List<byte>? workStyles, List<byte>? workTypes, List<Guid>? skillIds, int skip = 1, int take = 9)
        {
            var query = _context.Vacancies.Where(x => x.VacancyStatus == VacancyStatus.Active && x.EndDate > DateTime.Now)
                .OrderByDescending(x => x.CreatedDate)
                .AsNoTracking();

            query = ApplyVacancyFilters(query, titleName, categoryIds, countryIds, cityIds, null, minSalary, maxSalary, companyIds, workTypes, workStyles, skillIds);

            var vacancies = await query
                .Select(v => new VacancyGetAllDto
                {
                    Id = v.Id,
                    Title = v.Title,
                    CompanyLogo = v.Company.CompanyLogo != null ? $"{_currentUser.BaseUrl}/companyFiles/{v.Company.CompanyLogo}" : null,
                    CompanyName = v.Company.IsCompany ? v.Company.CompanyName : v.CompanyName,
                    StartDate = v.StartDate,
                    Location = v.Location,
                    ViewCount = v.ViewCount,
                    WorkType = v.WorkType,
                    WorkStyle = v.WorkStyle,
                    MainSalary = v.MainSalary,
                    MaxSalary = v.MaxSalary,
                    IsSaved = _currentUser.UserId != null && v.SavedVacancies.Any(x => x.VacancyId == v.Id && x.UserId == _currentUser.UserGuid),
                    SalaryCurrency = v.SalaryCurrency
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return new DataListDto<VacancyGetAllDto> { Datas = vacancies, TotalCount = await query.CountAsync() };
        }

        private static IQueryable<Vacancy> ApplyVacancyFilters(IQueryable<Vacancy> query, string? titleName, List<Guid>? categoryIds, List<Guid>? countryIds, List<Guid>? cityIds, VacancyStatus? isActive, decimal? minSalary, decimal? maxSalary, List<Guid>? companyIds, List<byte>? workTypes, List<byte>? workStyles, List<Guid>? skillIds)
        {
            if (titleName != null)
            {
                titleName = titleName.Trim();
                query = query.Where(x => x.Title.Contains(titleName));
            }

            if (isActive != null)
                query = query.Where(x => x.VacancyStatus == isActive);

            if (minSalary.HasValue)
            {
                var minVal = minSalary.Value;
                query = query.Where(x => x.MainSalary.HasValue && x.MainSalary.Value >= minVal);
            }

            if (maxSalary.HasValue)
            {
                var maxVal = maxSalary.Value;
                query = query.Where(x => x.MainSalary.HasValue && ((x.MaxSalary ?? x.MainSalary) <= maxVal));
            }

            if (workTypes != null && workTypes.Any())
                query = query.Where(x => x.WorkType.HasValue && workTypes.Contains((byte)x.WorkType.Value));

            if (workStyles != null && workStyles.Any())
                query = query.Where(x => x.WorkStyle.HasValue && workStyles.Contains((byte)x.WorkStyle.Value));

            if (companyIds != null && companyIds.Any())
            {
                query = query.Where(x => companyIds.Contains((Guid)x.CompanyId));
            }

            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(x => categoryIds.Contains((Guid)x.CategoryId));
            }

            if (countryIds != null && countryIds.Any())
            {
                query = query.Where(x => countryIds.Contains((Guid)x.CountryId));
            }

            if (cityIds != null && cityIds.Any())
            {
                query = query.Where(x => cityIds.Contains((Guid)x.CityId));
            }

            if (skillIds != null && skillIds.Any())
            {
                query = query.Where(v => v.VacancySkills.Any(vs => skillIds.Contains(vs.SkillId)));
            }

            return query;
        }

        public async Task ToggleSaveVacancyAsync(string vacancyId)
        {
            Guid vacancyGuid = Guid.Parse(vacancyId);

            if (!await _context.Vacancies.AnyAsync(x => x.Id == vacancyGuid))
                throw new NotFoundException();

            var vacancyCheck = await _context.SavedVacancies.FirstOrDefaultAsync(x => x.VacancyId == vacancyGuid);

            if (vacancyCheck != null)
            {
                _context.SavedVacancies.Remove(vacancyCheck);
            }
            else
            {
                await _context.SavedVacancies.AddAsync(
                    new SavedVacancy { UserId = _currentUser.UserGuid, VacancyId = vacancyGuid, SavedAt = DateTime.Now }
                );
            }
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Pause - Vakansiyanın artıq işaxtaranların qarşısına çıxmamağı və növbəti günün ödənişini etməməsi üçündür.
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException{Vacancy}"></exception>
        /// <exception cref="VacancyStatusNotToggableException"></exception>
        public async Task TogglePauseVacancyAsync(Guid vacancyId)
        {
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(x => x.Id == vacancyId && x.Company.UserId == _currentUser.UserGuid)
                ?? throw new NotFoundException();

            if (vacancy.VacancyStatus == VacancyStatus.Active)
            {
                vacancy.VacancyStatus = VacancyStatus.Pause;
            }
            else if (vacancy.VacancyStatus == VacancyStatus.Pause)
            {
                if (DateTime.Now > vacancy.PaymentDate)
                {
                    var balanceResponse = await _checkBalanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
                    {
                        InformationType = InformationType.Vacancy,
                        UserId = (Guid)_currentUser.UserGuid!
                    });

                    if (balanceResponse.Message.HasEnoughBalance)
                    {
                        vacancy.VacancyStatus = VacancyStatus.Active;
                        vacancy.PaymentDate = DateTime.Now.AddDays(1);
                    }
                    else
                    {
                        vacancy.VacancyStatus = VacancyStatus.PendingActive;
                        vacancy.PaymentDate = null;
                    }
                }
                else
                {
                    vacancy.VacancyStatus = VacancyStatus.Active;
                }
            }
            else
                throw new VacancyStatusNotToggableException();

            await _context.SaveChangesAsync();
        }

        public async Task<List<VacancyGetAllDto>> SimilarVacanciesAsync(string vacancyId, int take = 8)
        {
            var mainVacancy = await _context.Vacancies
                .AsNoTracking()
                .Where(x => x.Id == Guid.Parse(vacancyId) && x.EndDate > DateTime.Now)
                .Select(x => new
                {
                    x.Id,
                    x.CategoryId
                }).FirstOrDefaultAsync();

            var vacancies = await _context.Vacancies.AsNoTracking()
                .Where(x => x.CategoryId == mainVacancy.CategoryId && x.Id != mainVacancy.Id && x.VacancyStatus == VacancyStatus.Active && x.EndDate > DateTime.Now)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new VacancyGetAllDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    CompanyLogo = x.CompanyLogo != null ? $"{_currentUser.BaseUrl}/companyFiles/{x.Company.CompanyLogo}" : null,
                    CompanyName = x.Company.IsCompany ? x.Company.CompanyName : x.CompanyName,
                    StartDate = x.StartDate,
                    Location = x.Location,
                    ViewCount = x.ViewCount,
                    WorkType = x.WorkType,
                    WorkStyle = x.WorkStyle,
                    MainSalary = x.MainSalary,
                    MaxSalary = x.MaxSalary,
                    IsSaved = x.SavedVacancies.Any(y => y.VacancyId == x.Id && y.UserId == _currentUser.UserGuid),
                    SalaryCurrency = x.SalaryCurrency
                })
                .Take(take)
                .ToListAsync();

            return vacancies;
        }

        //TODO : burada iseduzelden eger vakansiya yaradarsa bu zaman vakansiyaya sekil qoyur ona gore de sekil hissesinde companyLogo olmali deyil
        public async Task<DataListDto<VacancyGetAllDto>> GetAllSavedVacancyAsync(int skip, int take, string? vacancyName)
        {
            var query = _context.SavedVacancies.Where(x => x.UserId == _currentUser.UserGuid)
                                               .AsNoTracking();

            if (!string.IsNullOrEmpty(vacancyName))
            {
                vacancyName = vacancyName.Trim();
                query = query.Where(x => x.Vacancy.Title.Contains(vacancyName));
            }

            var vacancies = await query
                .OrderByDescending(x => x.SavedAt)
                .Select(x => new VacancyGetAllDto
                {
                    Id = x.Vacancy.Id,
                    Title = x.Vacancy.Title,
                    CompanyLogo = x.Vacancy.Company.CompanyLogo != null ? $"{_currentUser.BaseUrl}/companyFiles/{x.Vacancy.Company.CompanyLogo}" : null,
                    CompanyName = x.Vacancy.Company.IsCompany ? x.Vacancy.Company.CompanyName : x.Vacancy.CompanyName,
                    StartDate = x.Vacancy.StartDate,
                    Location = x.Vacancy.Location,
                    ViewCount = x.Vacancy.ViewCount,
                    WorkType = x.Vacancy.WorkType,
                    WorkStyle = x.Vacancy.WorkStyle,
                    MainSalary = x.Vacancy.MainSalary,
                    MaxSalary = x.Vacancy.MaxSalary,
                    IsSaved = true,
                    SalaryCurrency = x.Vacancy.SalaryCurrency
                })
                .Skip(Math.Max(0, (skip - 1) * take))
                .Take(take)
                .ToListAsync();

            return new DataListDto<VacancyGetAllDto>
            {
                Datas = vacancies,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary>
        /// Vakansiyanın şirkət tərəfindən ləğv edilməsi
        /// </summary>
        /// <param name="vacancyId"></param>
        /// <returns></returns>
        public async Task DeleteVacancyAsync(Guid vacancyId)
        {
            var vacancy = await _context.Vacancies.Include(x => x.Applications).FirstOrDefaultAsync(x => x.Id == vacancyId)
                ?? throw new NotFoundException();

            vacancy.PaymentDate = null;
            vacancy.VacancyStatus = VacancyStatus.Deleted;

            if (vacancy.Applications != null)
            {
                await _context.Applications.Where(x => vacancy.Applications.Select(a => a.Id).Contains(x.Id))
                    .ExecuteUpdateAsync(setter => setter
                                                        .SetProperty(a => a.IsDeleted, true)
                                                        .SetProperty(a => a.IsActive, false));
            }
            await _context.SaveChangesAsync();
        }

        public async Task ActivateVacancyAsync(Guid vacancyId)
        {
            var vacancy = await _context.Vacancies.FirstOrDefaultAsync(x=> x.Id == vacancyId && x.Company.UserId == _currentUser.UserGuid)
                ?? throw new NotFoundException();

            var balance = await _checkBalanceRequest.GetResponse<CheckBalanceResponse>(new CheckBalanceRequest
            {
                InformationType = InformationType.Vacancy,
                UserId = (Guid)_currentUser.UserGuid
            });

            if(vacancy.VacancyStatus == VacancyStatus.PendingActive && vacancy.EndDate > DateTime.Now && vacancy.PaymentDate == null)
            {
                if (balance.Message.HasEnoughBalance)
                {
                    await _publishEndpoint.Publish(new PayEvent
                    {
                        InformationId = vacancyId,
                        InformationType = InformationType.Vacancy,
                        UserId = (Guid)_currentUser.UserGuid
                    });

                    vacancy.PaymentDate = DateTime.Now.AddDays(1);
                    vacancy.VacancyStatus = VacancyStatus.Active;
                    await _context.SaveChangesAsync();
                }
                else
                    throw new BadRequestException(MessageHelper.GetMessage("INSUFFICIENT_BALANCE"));
            }
            else
                throw new BadRequestException();

        }
    }
}
