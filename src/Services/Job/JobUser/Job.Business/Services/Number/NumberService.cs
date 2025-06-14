using Job.Business.Dtos.NumberDtos;
using Job.DAL.Contexts;

namespace Job.Business.Services.Number;

public class NumberService(JobDbContext context) : INumberService
{
    public List<Core.Entities.Number> CreateBulkNumber(ICollection<NumberCreateDto> numbersDto, Guid resumeId, string? mainNumber = null)
    {
        var normalizedSet = new HashSet<string>();
        var result = new List<Core.Entities.Number>();

        if (!string.IsNullOrWhiteSpace(mainNumber))
        {
            var normalizedMain = NormalizePhoneNumber(mainNumber);
            if (normalizedSet.Add(normalizedMain))
            {
                result.Add(new Core.Entities.Number
                {
                    PhoneNumber = mainNumber,
                    ResumeId = resumeId
                });
            }
        }

        foreach (var dto in numbersDto)
        {
            var normalized = NormalizePhoneNumber(dto.PhoneNumber);

            if (normalizedSet.Add(normalized))
            {
                result.Add(new Core.Entities.Number
                {
                    PhoneNumber = dto.PhoneNumber.Trim(),
                    ResumeId = resumeId
                });
            }
        }

        return result;
    }

    public async Task<List<Core.Entities.Number>> UpdateBulkNumberAsync(ICollection<NumberUpdateDto> numberUpdateDtos, ICollection<Core.Entities.Number> existingNumbers, Guid resumeId, string? mainNumber = null)
    {
        var resultList = new List<Core.Entities.Number>(); // Yenilənmiş və əlavə olunmuş nömrələrin siyahısı
        var numbersToAdd = new List<Core.Entities.Number>(); // Yalnız əlavə olunacaq yeni nömrələrin siyahısı

        // Artıq mövcud olan nömrələrin normallaşdırılmış formatlarını qeyd edirik ki, təkrar yazılmasın
        var processedPhoneNumbers = existingNumbers
            .Select(n => NormalizePhoneNumber(n.PhoneNumber))
            .ToHashSet();

        //Dto-da hansı Id-li nömrələrin mövcud olduğunu saxlayırıq (silinənləri müəyyən eləmək üçün)
        var inputIds = numberUpdateDtos
            .Where(dto => !string.IsNullOrWhiteSpace(dto.Id))
            .Select(dto => Guid.Parse(dto.Id!))
            .ToHashSet();

        // Əgər əsas nömrə qeyd olunbsa və artıq mövcud deyilsə əlavə edirik
        if (!string.IsNullOrWhiteSpace(mainNumber))
        {
            var normalizedMain = NormalizePhoneNumber(mainNumber);
            if (!processedPhoneNumbers.Contains(normalizedMain))
            {
                var number = new Core.Entities.Number
                {
                    PhoneNumber = mainNumber.Trim(),
                    ResumeId = resumeId
                };
                numbersToAdd.Add(number);
                processedPhoneNumbers.Add(normalizedMain); //Burda artıq istifadə edilmiş nömrələr siyahısına əlavə edirik
            }
        }

        // Dto-dan gələn nömrələri normallaşdırırıq (çünki istifadəçi eyni nömrəni həm boşluqla həm də boşluq olmadan əlavə eliyə bilər)
        foreach (var dto in numberUpdateDtos)
        {
            var trimmed = dto.PhoneNumber.Trim();
            var normalized = NormalizePhoneNumber(trimmed);

            // İd varsa deməli bu mövcud nömrədir və onu yeniləyirik
            if (!string.IsNullOrWhiteSpace(dto.Id))
            {
                var id = Guid.Parse(dto.Id);
                var existing = existingNumbers.FirstOrDefault(n => n.Id == id);
                if (existing != null)
                {
                    existing.PhoneNumber = trimmed;
                    resultList.Add(existing);
                    processedPhoneNumbers.Add(normalized);
                }
            }
            else
            {
                // Əgər bu yeni nömrədirsə və təkrar deyilsə əlavə edirik
                if (!processedPhoneNumbers.Contains(normalized))
                {
                    numbersToAdd.Add(new Core.Entities.Number
                    {
                        PhoneNumber = trimmed,
                        ResumeId = resumeId
                    });
                    processedPhoneNumbers.Add(normalized);
                }
            }
        }

        // Artıq istifadə olunmayan mövcud nömrələri silmək üçün seçirik
        var numbersToRemove = existingNumbers.Where(n => !inputIds.Contains(n.Id)).ToList();

        if (numbersToRemove.Any())
            context.Numbers.RemoveRange(numbersToRemove);

        if (numbersToAdd.Any())
            await context.Numbers.AddRangeAsync(numbersToAdd);

        resultList.AddRange(numbersToAdd);

        return resultList;
    }

    private static string NormalizePhoneNumber(string number)
    {
        return new string(number.Where(char.IsDigit).ToArray());
    }
}