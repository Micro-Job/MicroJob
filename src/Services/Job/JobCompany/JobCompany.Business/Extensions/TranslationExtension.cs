using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;

namespace JobCompany.Business.Extensions;

public static class TranslationExtensions
{
    public static IQueryable<T> IncludeTranslations<T>(this IQueryable<T> query)
        where T : class
    {
        return query.Include("Translations");
    }

    public static string GetTranslation<T>(this T entity, LanguageCode languageCode)
        where T : class
    {
        var propertyInfo = typeof(T).GetProperty("Translations");

        if (propertyInfo == null) return null;

        var translations = propertyInfo.GetValue(entity) as IEnumerable<dynamic>;

        return translations?
            .Where(t => t.Language == languageCode)
            .Select(t => t.Name)
            .FirstOrDefault();
    }

    public static string GetTranslationForComment<T>(this T entity, LanguageCode languageCode)
where T : class
    {
        var propertyInfo = typeof(T).GetProperty("Translations");

        if (propertyInfo == null) return null;

        var translations = propertyInfo.GetValue(entity) as IEnumerable<dynamic>;

        return translations?
            .Where(t => t.Language == languageCode)
            .Select(t => t.Comment)
            .FirstOrDefault();
    }
}