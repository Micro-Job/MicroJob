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
    }
}
