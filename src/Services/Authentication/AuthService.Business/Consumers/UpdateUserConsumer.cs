using AuthService.Core.Entities;
using AuthService.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Consumers
{
    public class UpdateUserConsumer(AppDbContext _context) : IConsumer<UpdateUserEvent>
    {
        //TODO : normalda burada email ve phoneNumber tekrarlanmasina gore checklenmelidir ve ona uygun proses davam etmelidir yeni ki: exception atilmali deyil sadece olaraq proses geri alinmalidir
        //yeni burada eslinde transaction-dan ve saga ve ya outbox kimi patternlerden istifade edilmelidir. Amma hele ki, ilkin versiya olaraq bu sekilde istifade edilir
        public async Task Consume(ConsumeContext<UpdateUserEvent> context)
        {
            UpdateUserEvent message = context.Message;

            User user = await _context.Users.FirstOrDefaultAsync(x=> x.Id == message.UserId);

            user.FirstName = message.FirstName;
            user.LastName = message.LastName;
            user.Email = message.Email;
            user.MainPhoneNumber = message.MainPhoneNumber;

            await _context.SaveChangesAsync();
        }
    }
}
