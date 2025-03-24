using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Enums
{
    public enum JobStatus : byte
    {
        NotLookingForAJob = 1,       // İş axtarmıram
        OpenToOffers = 2,            // Təklifləri nəzərdən keçirirəm
        ActivelySeekingJob = 3       // Aktiv iş axtarıram
    }
}
