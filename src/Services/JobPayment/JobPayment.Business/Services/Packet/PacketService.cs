using JobPayment.Business.Dtos.PacketDtos;
using JobPayment.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.Packet
{
    public class PacketService(PaymentDbContext _context) : IPacketService
    {
        public async Task CreatePacketAsync(CreatePacketDto dto)
        {
            if (await _context.Packets.AnyAsync(x => x.PacketType == dto.PacketType))
                throw new BadRequestException("Bu tipde paket movcuddur");

            await _context.Packets.AddAsync(new Core.Entities.Packet
            {
                Title = dto.Title,
                PacketType = dto.PacketType,
                Coin = dto.Coin,
                Value = dto.Value
            });
            await _context.SaveChangesAsync();
        }

        public Task UpdatePacketAsync(UpdatePacketDto dto)
        {
            
        }
    }
}
