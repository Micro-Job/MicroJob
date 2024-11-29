using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ApplicationInfoListDto
    {
        public string FullName { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}