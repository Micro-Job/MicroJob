﻿// <auto-generated />
using System;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JobCompany.DAL.Migrations
{
    [DbContext(typeof(JobCompanyDbContext))]
    partial class JobCompanyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("JobCompany.Core.Entites.Application", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("VacancyId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.HasIndex("VacancyId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CityName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Company", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CompanyInformation")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("CompanyLocation")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("CompanyLogo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EmployeeCount")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("WebLink")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CityId");

                    b.HasIndex("CountryId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.CompanyNumber", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Number")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<Guid?>("VacancyId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.HasIndex("VacancyId");

                    b.ToTable("CompanyNumbers");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CountryName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Status", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit");

                    b.Property<int>("Order")
                        .HasColumnType("int");

                    b.Property<string>("StatusColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusName")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Statuses");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Vacancy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Citizenship")
                        .HasColumnType("tinyint");

                    b.Property<Guid?>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("CompanyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CompanyLogo")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid?>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<byte>("Driver")
                        .HasColumnType("tinyint");

                    b.Property<string>("Email")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<byte>("Family")
                        .HasColumnType("tinyint");

                    b.Property<byte>("Gender")
                        .HasColumnType("tinyint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVip")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<decimal?>("MainSalary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("MaxSalary")
                        .HasColumnType("decimal(18,2)");

                    b.Property<byte>("Military")
                        .HasColumnType("tinyint");

                    b.Property<string>("Requirement")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("nvarchar(1024)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<Guid?>("VacancyTestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("ViewCount")
                        .HasColumnType("int");

                    b.Property<byte?>("WorkType")
                        .HasColumnType("tinyint");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CityId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("CountryId");

                    b.HasIndex("VacancyTestId");

                    b.ToTable("Vacancies");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.VacancyTest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("VacancyTests");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Application", b =>
                {
                    b.HasOne("JobCompany.Core.Entites.Status", "Status")
                        .WithMany("Applications")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("JobCompany.Core.Entites.Vacancy", "Vacancy")
                        .WithMany("Applications")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Status");

                    b.Navigation("Vacancy");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.City", b =>
                {
                    b.HasOne("JobCompany.Core.Entites.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Company", b =>
                {
                    b.HasOne("JobCompany.Core.Entites.Category", "Category")
                        .WithMany("Companies")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("JobCompany.Core.Entites.City", "City")
                        .WithMany("Companies")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("JobCompany.Core.Entites.Country", "Country")
                        .WithMany("Companies")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Category");

                    b.Navigation("City");

                    b.Navigation("Country");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.CompanyNumber", b =>
                {
                    b.HasOne("JobCompany.Core.Entites.Company", "Company")
                        .WithMany("CompanyNumbers")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JobCompany.Core.Entites.Vacancy", "Vacancy")
                        .WithMany("CompanyNumbers")
                        .HasForeignKey("VacancyId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Company");

                    b.Navigation("Vacancy");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Status", b =>
                {
                    b.HasOne("JobCompany.Core.Entites.Company", "Company")
                        .WithMany("Statuses")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Vacancy", b =>
                {
                    b.HasOne("JobCompany.Core.Entites.Category", "Category")
                        .WithMany("Vacancies")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("JobCompany.Core.Entites.City", "City")
                        .WithMany("Vacancies")
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("JobCompany.Core.Entites.Company", "Company")
                        .WithMany("Vacancies")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("JobCompany.Core.Entites.Country", "Country")
                        .WithMany("Vacancies")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("JobCompany.Core.Entites.VacancyTest", "VacancyTest")
                        .WithMany("Vacancies")
                        .HasForeignKey("VacancyTestId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Category");

                    b.Navigation("City");

                    b.Navigation("Company");

                    b.Navigation("Country");

                    b.Navigation("VacancyTest");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Category", b =>
                {
                    b.Navigation("Companies");

                    b.Navigation("Vacancies");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.City", b =>
                {
                    b.Navigation("Companies");

                    b.Navigation("Vacancies");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Company", b =>
                {
                    b.Navigation("CompanyNumbers");

                    b.Navigation("Statuses");

                    b.Navigation("Vacancies");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Country", b =>
                {
                    b.Navigation("Cities");

                    b.Navigation("Companies");

                    b.Navigation("Vacancies");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Status", b =>
                {
                    b.Navigation("Applications");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.Vacancy", b =>
                {
                    b.Navigation("Applications");

                    b.Navigation("CompanyNumbers");
                });

            modelBuilder.Entity("JobCompany.Core.Entites.VacancyTest", b =>
                {
                    b.Navigation("Vacancies");
                });
#pragma warning restore 612, 618
        }
    }
}
