using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Job.Business.Dtos.PersonDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.Business.ExternalServices;
using Job.Business.Statics;
using Job.Core.Entities;
using Job.DAL.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Job.Business.Services.Person
{
    public class PersonService : IPersonService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMapper _map;
        private readonly string _baseUrl;

        public PersonService(AppDbContext context, IFileService fileService, IMapper map, IHttpContextAccessor accessor)
        {
            _context = context;
            _fileService = fileService;
            _map = map;
            _baseUrl = $"{accessor.HttpContext.Request.Scheme}://{accessor.HttpContext.Request.Host}{accessor.HttpContext.Request.PathBase}";
        }

        public async Task CreateAsync(PersonCreateDto dto)
        {
            var userId = Guid.Parse(dto.UserId);
            try
            {
                var person = _map.Map<Core.Entities.Person>(dto);
                person.UserId = userId;

                if (dto.UserPhoto != null)
                {
                    var fileDto = await _fileService.UploadAsync(FilePaths.image, dto.UserPhoto);
                    person.UserPhoto = fileDto.FilePath;
                }

                await _context.Persons.AddAsync(person);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new CreateException<Core.Entities.Person>(ex.Message);
            }
        }

        public async Task<IEnumerable<PersonListDto>> GetAllAsync()
        {
            var persons = await _context.Persons.ToListAsync();
            var map = _map.Map<IEnumerable<PersonListDto>>(persons);
            foreach (var item in map)
            {
                item.UserPhoto = $"{_baseUrl}/{item.UserPhoto}";
            }
            return map;
        }

        public async Task<PersonDetailItemDto> GetByIdAsync(Guid id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                throw new NotFoundException<Core.Entities.Person>();
            }

            var map = _map.Map<PersonDetailItemDto>(person);
            map.UserPhoto = $"{_baseUrl}/{map.UserPhoto}";
            return map;
        }

        public async Task UpdateAsync(PersonUpdateDto dto)
        {
            var personId = Guid.Parse(dto.Id);
            var person = await _context.Persons.FindAsync(personId);
            if (person == null)
            {
                throw new NotFoundException<Core.Entities.Person>();
            }

            _map.Map(dto, person);

            if (dto.UserPhoto != null)
            {
                if (person.UserPhoto != null)
                {
                    _fileService.DeleteFile(person.UserPhoto);
                }
                var fileDto = await _fileService.UploadAsync(FilePaths.image, dto.UserPhoto);
                person.UserPhoto = fileDto.FilePath;
            }

            await _context.SaveChangesAsync();
        }
    }
}