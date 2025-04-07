using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.PriceDtos
{
    public class OldPriceDto
    {
        public Guid Id { get; set; }
        public Guid MainPriceId { get; set; }

        public double OldCoin { get; set; }
    }
}
