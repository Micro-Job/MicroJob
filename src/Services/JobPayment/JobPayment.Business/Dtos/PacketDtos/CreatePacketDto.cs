using Azure.Identity;
using JobPayment.Core.Enums;
using MassTransit.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Dtos.PacketDtos
{
    public class CreatePacketDto
    {

        public string Title { get; set; }
        public PacketType PacketType { get; set; }
        public decimal Value { get; set; }
        public double Coin { get; set; }
    }
}
