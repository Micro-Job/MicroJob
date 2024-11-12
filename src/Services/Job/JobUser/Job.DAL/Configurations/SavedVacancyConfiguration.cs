﻿using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.DAL.Configurations
{
    internal class SavedVacancyConfiguration : IEntityTypeConfiguration<SavedVacancy>
    {
        public void Configure(EntityTypeBuilder<SavedVacancy> builder)
        {
            builder.HasOne(sv => sv.User)
              .WithMany(u => u.SavedVacancies)
              .HasForeignKey(sv => sv.UserId);

            builder.Property(sv => sv.VacancyId)
                   .IsRequired();
        }
    }
}
