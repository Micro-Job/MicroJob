using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Job.Business.Dtos.PersonDtos;
using Job.Core.Entities;

namespace Job.Business.Profiles
{
    public class PersonMappingProfile : Profile
    {
        public PersonMappingProfile()
        {
            CreateMap<PersonCreateDto, Person>().ReverseMap();
            CreateMap<PersonUpdateDto, Person>().ReverseMap();
            CreateMap<PersonListDto, Person>().ReverseMap();
            CreateMap<PersonDetailItemDto, Person>().ReverseMap();
        }
    }
}