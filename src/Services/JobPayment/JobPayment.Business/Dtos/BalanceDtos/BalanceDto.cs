using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.BalanceDtos
{
    public class BalanceDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public double Coin { get; set; }
        public double? BonusCoin { get; set; }
    }
}
