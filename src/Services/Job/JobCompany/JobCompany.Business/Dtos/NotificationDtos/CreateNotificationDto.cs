using Microsoft.AspNetCore.Server.HttpSys;
using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.NotificationDtos
{
    public class CreateNotificationDto
    {
        public NotificationType NotificationType { get; set; }
        public Guid? SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid InformationId { get; set; }
        public string InformationName { get; set; }
    }
}
