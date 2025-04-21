using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;

namespace Job.Business.Services.Number
{
    public class NumberService(JobDbContext context) : INumberService
    {
        public async Task<Core.Entities.Number> CreateNumberAsync(NumberCreateDto numberCreateDto, Guid resumeId)
        {
            var number = new Core.Entities.Number
            {
                PhoneNumber = numberCreateDto.PhoneNumber,
                ResumeId = resumeId
            };

            await context.Numbers.AddAsync(number);
            return number;
        }

        public async Task<List<Core.Entities.Number>> CreateBulkNumberAsync(ICollection<NumberCreateDto> numberCreateDtos, Guid resumeId)
        {
            var numbersToAdd = new List<Core.Entities.Number>();

            foreach (var numberCreate in numberCreateDtos)
            {
                var number = await CreateNumberAsync(numberCreate, resumeId);

                numbersToAdd.Add(number);
            }

            return numbersToAdd;
        }

        public async Task<List<Core.Entities.Number>> UpdateBulkNumberAsync(ICollection<NumberUpdateDto> numberUpdateDtos, ICollection<Core.Entities.Number> existingNumbers, Guid resumeId)
        {
            var resultList = new List<Core.Entities.Number>();
            var numbersToAdd = new List<Core.Entities.Number>();

            var inputIds = numberUpdateDtos
                .Where(dto => !string.IsNullOrWhiteSpace(dto.Id))
                .Select(dto => Guid.Parse(dto.Id))
                .ToHashSet();

            // Əlavə edilən nömrələrin təkrarlanmaması üçün izlənən siyahı
            var processedPhoneNumbers = existingNumbers.Select(n => n.PhoneNumber.Trim()).ToHashSet(); 

            foreach (var dto in numberUpdateDtos)
            {
                var normalizedPhoneNumber = dto.PhoneNumber.Trim();

                // Əgər nömrə artıq mövcud nömrələrdə varsa, onu güncəlləyir
                if (!string.IsNullOrWhiteSpace(dto.Id))
                {
                    var existing = existingNumbers.FirstOrDefault(n => n.Id == Guid.Parse(dto.Id));
                    if (existing != null)
                    {
                        existing.PhoneNumber = normalizedPhoneNumber;
                        resultList.Add(existing);
                    }
                }
                else
                {
                    //Əgər bu nömrə artıq əlavə edilənlərdə və ya mövcudlarda yoxdursa, əlavə edir
                    if (!processedPhoneNumbers.Contains(normalizedPhoneNumber))
                    {
                        var newNumber = new Core.Entities.Number
                        {
                            PhoneNumber = normalizedPhoneNumber,
                            ResumeId = resumeId
                        };

                        numbersToAdd.Add(newNumber);
                        processedPhoneNumbers.Add(normalizedPhoneNumber); // Təkrar əlavə olunmasın deyə izlənən siyahıya salırıq
                    }
                }
            }

            var numbersToRemove = existingNumbers
                .Where(n => !inputIds.Contains(n.Id))
                .ToList();

            if (numbersToRemove.Count > 0)
                context.Numbers.RemoveRange(numbersToRemove);

            if (numbersToAdd.Count > 0)
                await context.Numbers.AddRangeAsync(numbersToAdd);

            resultList.AddRange(numbersToAdd);

            return resultList;
        }


        public async Task UpdateNumberAsync(NumberUpdateDto numberUpdateDto)
        {
            var number = await context.Numbers
                .FirstOrDefaultAsync(n => n.PhoneNumber == numberUpdateDto.PhoneNumber)
                ?? throw new NotFoundException<Core.Entities.Number>(MessageHelper.GetMessage("NOT_FOUND"));

            number.PhoneNumber = numberUpdateDto.PhoneNumber;
            await context.SaveChangesAsync();
        }
    }
}