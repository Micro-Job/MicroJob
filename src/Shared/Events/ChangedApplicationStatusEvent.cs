using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Events
{
    /// <summary>
    /// Status dəyişildiyi zaman mailin getmesi ucun autha publish edilir bu message(Niye auth cunki emailService oradadir kod tekrari olmasin deye normalda EmailService ayrica bir proyekt olmali idi)
    /// </summary>
    public class ChangedApplicationStatusEvent
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
