using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;

namespace Job.Business.Extensions;

public static class TranslationExtensions
{
    public static IQueryable<T> IncludeTranslations<T>(this IQueryable<T> query)
        where T : class
    {
        return query.Include("Translations");
    }

    //    public static List<TranslationsGetDto> GetAllTranslationForEntity<T>(this T entity)
    //where T : class
    //    {
    //        var propertyInfo = typeof(T).GetProperty("Translations");

    //        if (propertyInfo == null) return new List<TranslationsGetDto>();

    //        var translations = propertyInfo.GetValue(entity) as IEnumerable<dynamic>;

    //        return translations?
    //            .Select(t => new TranslationsGetDto
    //            {
    //                Id = t.Id,
    //                Name = t.Name,
    //                Language = t.Language
    //            })
    //            .ToList() ?? new List<TranslationsGetDto>();
    //    }

    public static string GetTranslation<T>(this T entity, LanguageCode languageCode, string propName)
        where T : class
    {
        var propertyInfo = typeof(T).GetProperty("Translations");

        if (propertyInfo == null) return null;

        var translations = propertyInfo.GetValue(entity) as IEnumerable<dynamic>;
        // TODO : optimallasdirma 
        return translations?
    .Where(t => t.Language == languageCode)
    .Select(t => (string)t.GetType().GetProperty(propName)?.GetValue(t, null))
            .FirstOrDefault();
    }

}