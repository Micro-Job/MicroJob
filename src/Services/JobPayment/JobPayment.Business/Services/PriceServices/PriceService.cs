using JobPayment.Business.Dtos.PriceDtos;
using JobPayment.Core.Entities;
using JobPayment.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPayment.Business.Services.PriceServices
{
    public class PriceService(PaymentDbContext _context) : IPriceService
    {
        public async Task<List<PriceListDto>> GetAllPricesAsync()
        {
            var prices = await _context.Services
            .Select(x => new PriceListDto
            {
                Id = x.Id,
                Coin = x.Coin,
                InformationType = x.InformationType,
                OldPrices = x.OldPrices.Select(y=> new OldPriceDto
                { 
                    Id = y.Id,
                    MainPriceId = y.PriceId,
                    OldCoin = y.OldCoin
                }).ToList()
            })
            .ToListAsync();

            return prices;
        }

        public async Task<Service> GetPriceByInformationTypeAsync(InformationType type)
        {
            var price = await _context.Services.FirstOrDefaultAsync(x=> x.InformationType == type) 
                                        ?? throw new NotFoundException<Service>();

            return price;
        }

        public async Task UpdatePriceAsync(UpdatePriceDto dto)
        {
            var price = await _context.Services.FirstOrDefaultAsync(x=> x.Id == Guid.Parse(dto.Id))
                                                ?? throw new NotFoundException<Service>();

            var oldPrice = new OldService
            {
                PriceId = price.Id,
                OldCoin = price.Coin,
            };

            price.Coin = dto.Coin;

            await _context.OldServices.AddAsync(oldPrice);
            await _context.SaveChangesAsync();
        }
    }
}
