﻿using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Application : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
        public Guid? StatusId { get; set; }
        public Status? Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
