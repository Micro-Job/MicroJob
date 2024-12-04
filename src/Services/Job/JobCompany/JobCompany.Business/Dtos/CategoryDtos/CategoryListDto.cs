using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.CategoryDtos
{
    public record CategoryListDto
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; }
    }
}