using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.LanguageDtos;

namespace Job.Business.Services.Language
{
    public interface ILanguageService
    {
        Task CreateLanguageAsync(LanguageCreateDto dto);
        Task UpdateLanguageAsync(string id, LanguageUpdateDto dto);
    }
}