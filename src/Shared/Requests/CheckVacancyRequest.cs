﻿namespace SharedLibrary.Requests;

public class CheckVacancyRequest
{
    public Guid VacancyId { get; set; }
    public Guid UserId { get; set; }
}
