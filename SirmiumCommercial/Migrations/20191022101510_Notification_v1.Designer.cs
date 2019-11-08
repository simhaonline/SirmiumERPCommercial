﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20191022101510_Notification_v1")]
    partial class Notification_v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Chat", b =>
                {
                    b.Property<int>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("User1Checkpoint");

                    b.Property<string>("User1Id");

                    b.Property<DateTime>("User2Checkpoint");

                    b.Property<string>("User2Id");

                    b.HasKey("ChatId");

                    b.ToTable("Chats");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.ChatMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ChatId");

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("MessageContent");

                    b.Property<string>("MessageType");

                    b.Property<bool>("Seen");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("ChatId");

                    b.ToTable("ChatMessages");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CommentId");

                    b.Property<string>("Content");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateAdded");

                    b.Property<int>("Dislikes");

                    b.Property<string>("For");

                    b.Property<int>("ForId");

                    b.Property<int>("Likes");

                    b.HasKey("Id");

                    b.ToTable("Comments");
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

                    b.Property<string>("Description");

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.Property<int>("VideoId");

                    b.HasKey("CourseId");

                    b.HasIndex("CreatedById");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.CourseUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AppUserId");

                    b.Property<int>("CourseId");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("CourseId");

                    b.ToTable("CourseUsers");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Dislikes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("For");

                    b.Property<int>("ForId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Dislikes");
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

            modelBuilder.Entity("SirmiumCommercial.Models.GroupChat", b =>
                {
                    b.Property<int>("ChatId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChatPhotoPath");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Title");

                    b.HasKey("ChatId");

                    b.ToTable("GroupChats");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupChatMessage", b =>
                {
                    b.Property<int>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateAdded");

                    b.Property<int?>("GroupChatChatId");

                    b.Property<string>("MessageContent");

                    b.Property<string>("MessageType");

                    b.Property<bool>("Seen");

                    b.Property<string>("UserId");

                    b.HasKey("MessageId");

                    b.HasIndex("GroupChatChatId");

                    b.ToTable("GroupChatMessages");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupChatUsers", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GroupChatChatId");

                    b.Property<string>("UserId");

                    b.HasKey("id");

                    b.HasIndex("GroupChatChatId");

                    b.ToTable("GroupChatUsers");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupMessageView", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("GroupChatMessageMessageId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("GroupChatMessageMessageId");

                    b.ToTable("GroupMessageViews");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AppUserId");

                    b.Property<int>("GroupId");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupUsers");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Likes", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("For");

                    b.Property<int>("ForId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("For");

                    b.Property<int>("ForId");

                    b.Property<string>("Msg");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
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

                    b.Property<int>("Part");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.Property<int>("VideoId");

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

                    b.Property<int>("Rating");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.Property<int>("VideoId");

                    b.HasKey("RepresentationId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("PresentationId");

                    b.ToTable("Representations");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("For");

                    b.Property<int>("ForId");

                    b.Property<string>("Status");

                    b.Property<string>("Title");

                    b.Property<string>("VideoPath");

                    b.Property<int>("Views");

                    b.HasKey("Id");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.AppUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("CompanyName");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("ProfilePhotoUrl");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<string>("Status");

                    b.ToTable("AppUser");

                    b.HasDiscriminator().HasValue("AppUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SirmiumCommercial.Models.ChatMessage", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.Chat")
                        .WithMany("Messages")
                        .HasForeignKey("ChatId");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Course", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.CourseUsers", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser")
                        .WithMany("Courses")
                        .HasForeignKey("AppUserId");

                    b.HasOne("SirmiumCommercial.Models.Course")
                        .WithMany("Users")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SirmiumCommercial.Models.Group", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupChatMessage", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.GroupChat")
                        .WithMany("Messages")
                        .HasForeignKey("GroupChatChatId");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupChatUsers", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.GroupChat")
                        .WithMany("Users")
                        .HasForeignKey("GroupChatChatId");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupMessageView", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.GroupChatMessage")
                        .WithMany("Views")
                        .HasForeignKey("GroupChatMessageMessageId");
                });

            modelBuilder.Entity("SirmiumCommercial.Models.GroupUsers", b =>
                {
                    b.HasOne("SirmiumCommercial.Models.AppUser")
                        .WithMany("Groups")
                        .HasForeignKey("AppUserId");

                    b.HasOne("SirmiumCommercial.Models.Group")
                        .WithMany("Users")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
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
#pragma warning restore 612, 618
        }
    }
}
