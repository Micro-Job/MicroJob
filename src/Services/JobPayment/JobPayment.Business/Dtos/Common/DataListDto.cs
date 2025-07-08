using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.Common
{
    public class DataListDto<T> where T : class
    {
        public ICollection<T>? Datas { get; set; }
        public int? TotalCount { get; set; }
    }
}
