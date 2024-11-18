using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record ApplicationUpdateDto
    {
        public string? StatusId { get; set; }
    }
}