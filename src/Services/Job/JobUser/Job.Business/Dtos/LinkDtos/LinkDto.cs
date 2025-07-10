using Job.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Dtos.LinkDtos
{
    public class LinkDto
    {
        public LinkEnum LinkType { get; set; }
        public string Url { get; set; }
    }
}
