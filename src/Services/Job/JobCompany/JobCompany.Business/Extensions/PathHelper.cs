namespace JobCompany.Business.Extensions;

static class PathHelper
{
    /// <summary>
    /// File path-dəki backslash (“\”) işarəsini forward slash (“/”) ilə əvəz edir və sondakı slash işarələrini silir.
    /// </summary>
    public static string NormalizeSlashes(this string path) =>
        path?.Replace('\\', '/').TrimEnd('/') ?? string.Empty;
}
