﻿using AuthService.Core.Entities.Base;
using SharedLibrary.Enums;

namespace AuthService.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string MainPhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime? RefreshTokenExpireDate { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RegistrationDate { get; set; }

        public DateTime? LockDownDate { get; set; }

        public string? Image { get; set; }

        public virtual ICollection<LoginLog> LoginLogs { get; set; } = new List<LoginLog>(); 

        public PasswordToken? PasswordToken { get; set; }

        public UserRole UserRole { get; set; }
        //TODO : JobStatus neye gore authdadir
        public JobStatus JobStatus { get; set; }
    }
}