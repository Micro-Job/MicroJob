﻿using Job.Core.Enums;

namespace Job.Business.Dtos.LanguageDtos
{
    public class LanguageGetByIdDto
    {
        public Guid Id { get; set; }
        public Language LanguageName { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
    }
}