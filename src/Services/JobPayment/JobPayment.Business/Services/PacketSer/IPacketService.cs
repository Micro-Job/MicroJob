using JobPayment.Business.Dtos.PacketDtos;
using JobPayment.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.PacketSer
{
    public interface IPacketService
    {
        public Task CreatePacketAsync(CreatePacketDto dto);
        public Task UpdatePacketAsync(UpdatePacketDto dto);
        public Task<List<PacketListDto>> GetAllPacketsAsync();
        public Task<Packet> GetPacketByIdAsync(string packetId);
    }
}
