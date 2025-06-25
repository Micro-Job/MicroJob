using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;

namespace Job.Business.Extensions;

public static class TranslationExtensions
{
    public static string? GetTranslation<T>(this T entityTranslationProp, LanguageCode languageCode, string propName) where T : class
    {
        var translations = entityTranslationProp as IEnumerable<dynamic>;

        return translations?
                .Where(t => t.Language == languageCode)
                .Select(t => t.GetType().GetProperty(propName)?.GetValue(t, null))
                        .FirstOrDefault();
    }

}