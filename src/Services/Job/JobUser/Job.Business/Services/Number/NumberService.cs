using System;
using System.Threading.Tasks;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.Core.Entities;
using Job.DAL.Contexts;

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
            var person = await _context.Persons.FindAsync(dto.PersonId);
            if (person == null)
            {
                throw new NotFoundException<Core.Entities.Person>();
            }

            var isExistNumber = _context.Numbers.Any(n => n.PhoneNumber == dto.PhoneNumber);

            if (isExistNumber)
            {
                throw new IsAlreadyExistException<Core.Entities.Number>();
            }

            var newNumber = new Core.Entities.Number
            {
                PersonId = dto.PersonId,
                PhoneNumber = dto.PhoneNumber
            };

            await _context.Numbers.AddAsync(newNumber);
            await _context.SaveChangesAsync();
        }
    }
}
