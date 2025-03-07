﻿using Job.Core.Enums;

namespace Job.Core.Entities
{
    public class Resume : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string FatherName { get; set; }
        public string Position { get; set; }
        public string? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public bool IsPublic { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public string? ResumeEmail { get; set; }
        public ICollection<Number> PhoneNumbers { get; set; }
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience>? Experiences { get; set; }
        public ICollection<Language>? Languages { get; set; }
        public ICollection<Certificate>? Certificates { get; set; }
        public ICollection<ResumeSkill> ResumeSkills { get; set; }
    }
}