using System;
using System.Threading.Tasks;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Number
{
    public class NumberService : INumberService
    {
        private readonly AppDbContext _context;

        public NumberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(NumberCreateDto dto)
        {
            var userId = Guid.Parse(dto.PersonId);

            var person = await _context.Persons
                .Include(p => p.PhoneNumbers)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new NotFoundException<Core.Entities.Person>();

            var existingNumbers = person.PhoneNumbers.Where(n => n.PhoneNumber == dto.PhoneNumber).ToList();
            if (existingNumbers != null)
            {
                throw new IsAlreadyExistException<Core.Entities.Number>();
            }

            var newNumber = new Core.Entities.Number
            {
                PersonId = userId,
                PhoneNumber = dto.PhoneNumber
            };

            await _context.Numbers.AddAsync(newNumber);
        }

        public async Task<IEnumerable<NumberListDto>> GetAllAsync()
        {
            var numbers = await _context.Numbers.ToListAsync();

            var numberList = numbers.Select(n => new NumberListDto
            {
                Id = n.Id,
                PersonId = n.PersonId,
                PhoneNumber = n.PhoneNumber
            });

            return numberList;
        }

        public async Task<NumberDetailItemDto> GetByIdAsync(Guid id)
        {
            var number = await _context.Numbers
                .FirstOrDefaultAsync(n => n.Id == id)
                ?? throw new NotFoundException<Core.Entities.Number>();

            var numberDetail = new NumberDetailItemDto
            {
                Id = number.Id,
                PersonId = number.PersonId,
                PhoneNumber = number.PhoneNumber
            };

            return numberDetail;
        }

        public async Task UpdateAsync(NumberUpdateDto dto)
        {
            var numberId = Guid.Parse(dto.Id);
            var userId = Guid.Parse(dto.PersonId);

            var person = await _context.Persons
                .Include(p => p.PhoneNumbers)
                .FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new NotFoundException<Core.Entities.Person>();

            var existingNumber = person.PhoneNumbers.FirstOrDefault(n => n.Id == numberId);
            if (existingNumber == null)
            {
                throw new NotFoundException<Core.Entities.Number>();
            }

            var isDuplicateNumber = person.PhoneNumbers
                .Where(n => n.PhoneNumber == dto.PhoneNumber && n.Id != numberId);
            if (isDuplicateNumber != null)
            {
                throw new IsAlreadyExistException<Core.Entities.Number>();
            }

            existingNumber.PhoneNumber = dto.PhoneNumber;

            await _context.SaveChangesAsync();
        }
    }
}
