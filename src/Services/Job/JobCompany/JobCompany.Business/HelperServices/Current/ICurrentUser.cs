using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.HelperServices.Current
{
    public interface ICurrentUser
    {
        public string? UserId { get; }
        public Guid? UserGuid { get; }
        public string? UserName { get; }
        public string? BaseUrl { get; }
    }
}
