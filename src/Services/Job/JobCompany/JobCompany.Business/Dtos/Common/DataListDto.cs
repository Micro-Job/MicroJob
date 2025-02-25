using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.Common
{
    public class DataListDto<T> where T : class
    {
        public List<T> Datas { get; set; }
        public int? TotalPage { get; set; }
        public int? TotalCount { get; set; }
    }
}
