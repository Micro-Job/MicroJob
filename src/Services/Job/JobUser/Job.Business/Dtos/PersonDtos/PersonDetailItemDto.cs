using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Core.Enums;

namespace Job.Business.Dtos.PersonDtos
{
    public record PersonDetailItemDto
    {
        public Guid Id { get; set; }
        public string FatherName { get; set; }
        public string? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
    }
}