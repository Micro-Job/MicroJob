using JobCompany.Core.Entites.Base;
using SharedLibrary.Enums;

namespace JobCompany.Core.Entites;

/// <summary>
/// Sistemdə olan mövcud müraciət mərhələlərini (statuslarını) təyin edir
/// </summary>
public class ApplicationStep : BaseEntity
{
    public StatusEnum StatusEnum { get; set; }
    public string StatusColor { get; set; }
    public bool IsVisible { get; set; }
    public byte Order { get; set; }
}
