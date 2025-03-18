namespace SharedLibrary.Enums;

public enum NotificationType : byte
{
    /// <summary>
    /// Sistem bildirisleri
    /// </summary>
    System=1,
    /// <summary>
    /// Vacancy paylasim bildirisi
    /// </summary>
    Vacancy = 2,
    /// <summary>
    /// Vacancynin statusunun deyismesi
    /// </summary>
    StatusUpdate = 3,
    /// <summary>
    /// Sirkete muraciet apply bildirisi
    /// </summary>
    Application = 4,
}

