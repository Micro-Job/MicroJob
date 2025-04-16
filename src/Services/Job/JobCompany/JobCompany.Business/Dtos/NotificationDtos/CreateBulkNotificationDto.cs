using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.NotificationDtos
{
    public class CreateBulkNotificationDto
    {
        public NotificationType NotificationType { get; set; }
        public Guid? SenderId { get; set; }
        public List<Guid> ReceiverIds { get; set; }
        public Guid InformationId { get; set; }
        public string InformationName { get; set; }
    }
}
