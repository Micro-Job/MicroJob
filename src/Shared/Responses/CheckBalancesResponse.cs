using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Responses
{
    public class CheckBalancesResponse
    {
        public ICollection<BalancesResponseDto>? UserBalanceInformations { get; set; }
    }

    public class BalancesResponseDto
    {
        public Guid UserId { get; set; }
        public bool HasEnoughBalance { get; set; }
        public double Coin { get; set; }
    }
}
