using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Core.Entities.Number>> UpdateBulkNumberAsync(ICollection<NumberUpdateDto> numberUpdateDtos, Guid resumeId)
        {
            var numbersToUpdate = new List<Core.Entities.Number>();

            foreach (var numberUpdateDto in numberUpdateDtos)
            {
                var number = await context.Numbers
                    .FirstOrDefaultAsync(n => n.PhoneNumber == numberUpdateDto.PhoneNumber && n.ResumeId == resumeId)
                    ?? throw new NotFoundException<Core.Entities.Number>();
                
                number.PhoneNumber = numberUpdateDto.PhoneNumber;
                numbersToUpdate.Add(number);
            }

            context.Numbers.UpdateRange(numbersToUpdate);
            await context.SaveChangesAsync();

            return numbersToUpdate;
        }

        public async Task UpdateNumberAsync(NumberUpdateDto numberUpdateDto)
        {
            var number = await context.Numbers
                .FirstOrDefaultAsync(n => n.PhoneNumber == numberUpdateDto.PhoneNumber)
                ?? throw new NotFoundException<Core.Entities.Number>();

            number.PhoneNumber = numberUpdateDto.PhoneNumber;
            await context.SaveChangesAsync();
        }
    }
}