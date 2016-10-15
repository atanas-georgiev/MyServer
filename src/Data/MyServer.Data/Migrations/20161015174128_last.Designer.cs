using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyServer.Data;

namespace MyServer.Data.Migrations
{
    [DbContext(typeof(MyServerDbContext))]
    [Migration("20161015174128_last")]
    partial class last
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MyServer.Data.Models.Album", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Access");

                    b.Property<string>("AddedById");

                    b.Property<Guid?>("CoverId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("DescriptionBg")
                        .HasAnnotation("MaxLength", 3000);

                    b.Property<string>("DescriptionEn")
                        .HasAnnotation("MaxLength", 3000);

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedOn");

                    b.Property<string>("TitleBg")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("TitleEn")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.HasKey("Id");

                    b.HasIndex("AddedById");

                    b.HasIndex("CoverId");

                    b.ToTable("Albums");
                });

            modelBuilder.Entity("MyServer.Data.Models.Comment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AlbumId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 1000);

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<Guid?>("ImageId");

                    b.Property<bool>("IsDeleted");

                    b.Property<DateTime?>("ModifiedOn");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ImageId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("MyServer.Data.Models.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddedById");

                    b.Property<Guid?>("AlbumId");

                    b.Property<string>("Aperture")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("CameraMaker")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("CameraModel")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DateTaken");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("ExposureBiasStep")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("FocusLen")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("Height");

                    b.Property<Guid?>("ImageGpsDataId");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Iso")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("LowHeight");

                    b.Property<int>("LowWidth");

                    b.Property<int>("MidHeight");

                    b.Property<int>("MidWidth");

                    b.Property<DateTime?>("ModifiedOn");

                    b.Property<string>("OriginalFileName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("ShutterSpeed")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Title")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<int>("Width");

                    b.HasKey("Id");

                    b.HasIndex("AddedById");

                    b.HasIndex("AlbumId");

                    b.HasIndex("ImageGpsDataId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("MyServer.Data.Models.ImageGpsData", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<bool>("IsDeleted");

                    b.Property<double?>("Latitude")
                        .IsRequired();

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<double?>("Longitude")
                        .IsRequired();

                    b.Property<DateTime?>("ModifiedOn");

                    b.HasKey("Id");

                    b.ToTable("ImageGpsDatas");
                });

            modelBuilder.Entity("MyServer.Data.Models.User", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("DeletedOn");

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTime?>("ModifiedOn");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<int>("NotificationMask");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MyServer.Data.Models.User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MyServer.Data.Models.User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyServer.Data.Models.User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyServer.Data.Models.Album", b =>
                {
                    b.HasOne("MyServer.Data.Models.User", "AddedBy")
                        .WithMany("Albums")
                        .HasForeignKey("AddedById");

                    b.HasOne("MyServer.Data.Models.Image", "Cover")
                        .WithMany("Covers")
                        .HasForeignKey("CoverId");
                });

            modelBuilder.Entity("MyServer.Data.Models.Comment", b =>
                {
                    b.HasOne("MyServer.Data.Models.Album")
                        .WithMany("Comments")
                        .HasForeignKey("AlbumId");

                    b.HasOne("MyServer.Data.Models.Image")
                        .WithMany("Comments")
                        .HasForeignKey("ImageId");

                    b.HasOne("MyServer.Data.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyServer.Data.Models.Image", b =>
                {
                    b.HasOne("MyServer.Data.Models.User", "AddedBy")
                        .WithMany("Images")
                        .HasForeignKey("AddedById");

                    b.HasOne("MyServer.Data.Models.Album", "Album")
                        .WithMany("Images")
                        .HasForeignKey("AlbumId");

                    b.HasOne("MyServer.Data.Models.ImageGpsData", "ImageGpsData")
                        .WithMany()
                        .HasForeignKey("ImageGpsDataId");
                });
        }
    }
}
