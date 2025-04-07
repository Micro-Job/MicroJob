using JobPayment.Core.Entities.Base;
using JobPayment.Core.Enums;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Core.Entities
{
    public class Service : BaseEntity
    {
        public InformationType InformationType { get; set; }
        public double Coin { get; set; }

        public ICollection<OldService>? OldPrices { get; set; }
    }
}
