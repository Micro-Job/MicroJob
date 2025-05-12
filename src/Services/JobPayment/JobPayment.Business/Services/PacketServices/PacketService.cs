using JobPayment.Business.Dtos.PacketDtos;
using JobPayment.Core.Entities;
using JobPayment.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.PacketServices
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

        public async Task UpdatePacketAsync(UpdatePacketDto dto)
        {
            var existPacket = await _context.Packets.FirstOrDefaultAsync(x => x.Id == dto.PacketId)
                                    ?? throw new NotFoundException<Packet>();

            if(existPacket.Coin != dto.Coin || existPacket.Value != dto.Value)
            {
                var oldPacket = new OldPacket
                {
                    PacketId = existPacket.Id,
                    Coin = existPacket.Coin,
                    OldValue = existPacket.Value,
                };

                await _context.OldPackets.AddAsync(oldPacket);
            }

            existPacket.Title = dto.Title.Trim();
            existPacket.Value = dto.Value;
            existPacket.Coin = dto.Coin;
            //Burada bu tipde paket varsa geriye exception qaytarmaq olar
            existPacket.PacketType = dto.PacketType;

            await _context.SaveChangesAsync();
        }

        public async Task<List<PacketListDto>> GetAllPacketsAsync()
        {
            var packets = await _context.Packets.Where(x => !x.IsDeleted)
            .Select(x => new PacketListDto
            {
                Id = x.Id,
                Title = x.Title,
                Coin = x.Coin,
                Value = x.Value
            })
            .AsNoTracking()
            .ToListAsync();

            return packets;
        }

        public async Task<Packet> GetPacketByIdAsync(string packetId)
        {
            var packet = await _context.Packets.FirstOrDefaultAsync(x=> x.Id == Guid.Parse(packetId) && !x.IsDeleted)
                ?? throw new NotFoundException<Packet>();

            return packet;  
        }
    }
}
