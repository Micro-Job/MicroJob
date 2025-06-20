using JobPayment.Core.Enums;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.PacketDtos
{
    public class PacketListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public double Coin { get; set; }
        public decimal Value { get; set; }
        public PacketType PacketType { get; set; }
    }
}
