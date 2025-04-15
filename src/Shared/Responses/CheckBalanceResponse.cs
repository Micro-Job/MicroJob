using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Responses
{
    public class CheckBalanceResponse
    {
        public bool HasBalance { get; set; }
        public bool HasEnoughBalance { get; set; }
        public double Coin { get; set; }
    }
}
