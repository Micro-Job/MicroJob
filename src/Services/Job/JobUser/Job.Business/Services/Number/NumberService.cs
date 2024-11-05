using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Exceptions.Common;
using Job.Core.Entities;
using Job.DAL.Contexts;

namespace Job.Business.Services.Number
{
    public class NumberService : INumberService
    {
        readonly AppDbContext _context;

        public NumberService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(NumberCreateDto numberCreateDto)
        {
            var resumeId = Guid.Parse(numberCreateDto.ResumeId);
            var resume = await _context.Resumes.FindAsync(resumeId);
            if (resume is null) throw new NotFoundException<Core.Entities.Resume>();

            var number = new Core.Entities.Number
            {
                ResumeId = resumeId,
                PhoneNumber = numberCreateDto.PhoneNumber,
            };
            await _context.Numbers.AddAsync(number);
        }

        public async Task UpdateAsync(string id, NumberUpdateDto numberUpdateDto)
        {
            var numberId = Guid.Parse(id);
            var number = await _context.Numbers.FindAsync(numberId);
            if (number is null) throw new NotFoundException<Core.Entities.Number>();

            number.PhoneNumber = numberUpdateDto.PhoneNumber;
            await _context.SaveChangesAsync();
        }
    }
}