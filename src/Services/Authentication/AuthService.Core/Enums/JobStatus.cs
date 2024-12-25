namespace AuthService.Core.Enums;

public enum JobStatus : byte
{
    NotLookingForAJob = 1,       // İş axtarmıram
    OpenToOffers = 2,            // Təklifləri nəzərdən keçirirəm
    ActivelySeekingJob = 3       // Aktiv iş axtarıram
}
