﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Migrations.AppDetailsDB
{
    [DbContext(typeof(AppDetailsDBContext))]
    partial class AppDetailsDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SirmiumCommercial.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("CompanyName");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<int?>("CourseId");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<int?>("GroupId");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Status");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("GroupId");

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Course", b =>
                {
                    b.Property<int>("CourseId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AwardIcon");

                    b.Property<string>("CreatedById");

                    b.Property<DateTime>("DateAdded");

                    b.Property<DateTime>("DateModified");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Title");

                    b.HasKey("CourseId");

                    b.HasIndex("CreatedById");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CompanyName");

                    b.Property<string>("CreatedById");

                    b.Property<string>("Name");

                    b.HasKey("GroupId");

                    b.HasIndex("CreatedById");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Presentation", b =>
                {
                    b.Property<int>("PresentationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CourseId");

                    b.Property<string>("CreatedById");

                    b.Property<DateTime>("DateAdded");

                    b.Property<DateTime>("DateModified");

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.HasKey("PresentationId");

                    b.HasIndex("CourseId");

                    b.HasIndex("CreatedById");

                    b.ToTable("Presentations");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Representation", b =>
                {
                    b.Property<int>("RepresentationId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedById");

                    b.Property<DateTime>("DateAdded");

                    b.Property<int?>("PresentationId");

                    b.Property<double>("Score");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.HasKey("RepresentationId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("PresentationId");

                    b.ToTable("Representations");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.UserDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ProfilePhoto");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UsersDetails");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.AppUser", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.Course")
                        .WithMany("Users")
                        .HasForeignKey("CourseId");

                    b.HasOne("SirmiumCommercial.Models.Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Course", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Group", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Presentation", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.Course")
                        .WithMany("Presentations")
                        .HasForeignKey("CourseId");

                    b.HasOne("SirmiumCommercial.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Representation", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("SirmiumCommercial.Models.Presentation")
                        .WithMany("Representations")
                        .HasForeignKey("PresentationId");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.UserDetails", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });
#pragma warning restore 612, 618
        }
    }
}
