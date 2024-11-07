using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;

namespace Job.Business.Services.Number
{
    public class NumberService : INumberService
    {
        readonly JobDbContext _context;

        public NumberService(JobDbContext context)
        {
            _context = context;
        }

        public async Task CreateNumberAsync(NumberCreateDto numberCreateDto)
        {
            var number = new Core.Entities.Number
            {
                PhoneNumber = numberCreateDto.PhoneNumber,
            };
            await _context.Numbers.AddAsync(number);
        }

        public async Task<ICollection<Core.Entities.Number>> CreateBulkNumberAsync(ICollection<NumberCreateDto> numberCreateDtos)
        {
            var numbersToAdd = new List<Core.Entities.Number>();

            foreach (var numberCreateDto in numberCreateDtos)
            {
                var number = new Core.Entities.Number
                {
                    PhoneNumber = numberCreateDto.PhoneNumber,
                };

                numbersToAdd.Add(number);
            }
            await _context.Numbers.AddRangeAsync(numbersToAdd);
            return numbersToAdd;
        }


        public async Task UpdateNumberAsync(string id, NumberUpdateDto numberUpdateDto)
        {
            var numberId = Guid.Parse(id);
            var number = await _context.Numbers.FindAsync(numberId);
            if (number is null) throw new NotFoundException<Core.Entities.Number>();

            number.PhoneNumber = numberUpdateDto.PhoneNumber;
        }
    }
}