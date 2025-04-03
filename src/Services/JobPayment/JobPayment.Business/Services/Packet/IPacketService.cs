using JobPayment.Business.Dtos.PacketDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.Packet
{
    public interface IPacketService
    {
        public Task CreatePacketAsync(CreatePacketDto dto);
        public Task UpdatePacketAsync(UpdatePacketDto dto);
    }
}
