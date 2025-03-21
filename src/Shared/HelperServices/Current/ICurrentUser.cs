﻿using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.HelperServices.Current
{
    public interface ICurrentUser
    {
        public string? UserId { get; }
        public Guid? UserGuid { get; }
        public string? UserName { get; }
        public string? BaseUrl { get; }
        public LanguageCode LanguageCode { get; }
    }
}
