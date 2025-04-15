namespace SharedLibrary.Enums;

public enum VacancyStatus
{
    /// <summary>
    /// Vakansiya yaradilan zaman default olaraq bu statusda olur
    /// </summary>
    Pending = 1,
    /// <summary>
    /// Vakansiya operator terefinden qebul edilerse ve sirketin balansinda yeteri qeder coin varsa
    /// </summary>
    Active = 2,
    Deactive = 3,
    /// <summary>
    /// Operator terefinden redd edilirse
    /// </summary>
    Reject = 4,
    Block = 5,
    /// <summary>
    /// Sirket terefinden vakansiya update edilerse vakansiya bu statusda olur
    /// </summary>
    Update = 6,
    Pause = 7,
    /// <summary>
    /// Operator terefinden qebul edilib amma sirketin yeteri qeder coini yoxdursa
    /// </summary>
    PendingActive = 8
}
