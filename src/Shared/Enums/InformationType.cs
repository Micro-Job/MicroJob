using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Enums
{
    public enum InformationType : byte
    {
        /// <summary>
        /// Gundelik vakansiya ucun odenis edilerse
        /// </summary>
        Vacancy = 1,
        /// <summary>
        /// Anonim resumelere baxmaq ucun odenis edilerse
        /// </summary>
        AnonymResume = 2,
        PacketPayment = 3
    }
}
