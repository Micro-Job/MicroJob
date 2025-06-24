using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos.VOEN
{
    public class Taxpayer
    {
        public string name { get; set; }
    }

    public class TaxpayerInfoRoot
    {
        public List<Taxpayer> taxpayers { get; set; }
    }

}
