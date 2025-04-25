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
    /// <summary>
    /// Vakansiyanin silinmesi bildirisi
    /// </summary>
    VacancyDeleted = 5,
    /// <summary>
    /// Vakansiyanin silinmesi bildirisi
    /// </summary>
    VacancyUpdate = 6,
    /// <summary>
    /// Vakansiyanin qebul edilmesi bildirisi
    /// </summary>
    VacancyAccept = 7,
    /// <summary>
    /// Vakansiyanin legv edilme bildirisi
    /// </summary>
    VacancyReject = 8,
    /// <summary>
    /// Vakansiya qebul edilib amma sirket balansinda yeterli coin yoxdur
    /// </summary>
    VacancyPendingActive = 9,
    /// <summary>
    /// Vakansiya elaninin gundelik pul cixmasi 
    /// </summary>
    VacancySuccessDailyPayment = 10,
    VacancyFailedDailyPayment = 11
}

