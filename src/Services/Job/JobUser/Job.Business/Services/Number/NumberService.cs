using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Number
{
    public class NumberService(JobDbContext context) : INumberService
    {
        readonly JobDbContext _context = context;

        public async Task CreateNumberAsync(NumberCreateDto numberCreateDto)
        {
            var number = new Core.Entities.Number
            {
                PhoneNumber = numberCreateDto.PhoneNumber,
            };
            await _context.Numbers.AddAsync(number);
        }

        public async Task<List<Core.Entities.Number>> CreateBulkNumberAsync(ICollection<NumberCreateDto> numberCreateDtos, Guid resumeId)
        {
            var numbersToAdd = new List<Core.Entities.Number>();

            foreach (var numberCreateDto in numberCreateDtos)
            {
                var number = new Core.Entities.Number
                {
                    ResumeId = resumeId,
                    PhoneNumber = numberCreateDto.PhoneNumber,
                };

                numbersToAdd.Add(number);
            }
            await _context.Numbers.AddRangeAsync(numbersToAdd);
            return numbersToAdd;
        }

        public async Task<List<Core.Entities.Number>> UpdateBulkNumberAsync(ICollection<NumberUpdateDto> numberUpdateDtos, Guid resumeId)
        {
            var numbersToUpdate = new List<Core.Entities.Number>();

            foreach (var numberUpdateDto in numberUpdateDtos)
            {
                var number = await _context.Numbers
                    .FirstOrDefaultAsync(n => n.PhoneNumber == numberUpdateDto.PhoneNumber && n.ResumeId == resumeId);

                if (number == null)
                {
                    throw new NotFoundException<Core.Entities.Number>();
                }

                number.PhoneNumber = numberUpdateDto.PhoneNumber;
                numbersToUpdate.Add(number);
            }

            _context.Numbers.UpdateRange(numbersToUpdate); 
            await _context.SaveChangesAsync(); 

            return numbersToUpdate;
        }


        public async Task UpdateNumberAsync(NumberUpdateDto numberUpdateDto)
        {
            var number = await _context.Numbers
                .FirstOrDefaultAsync(n => n.PhoneNumber == numberUpdateDto.PhoneNumber)
                ?? throw new NotFoundException<Core.Entities.Number>();

            number.PhoneNumber = numberUpdateDto.PhoneNumber;
            await _context.SaveChangesAsync();
        }
    }
}