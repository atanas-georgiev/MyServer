namespace MyServer.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class last : Migration
    {
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Albums_AspNetUsers_AddedById", table: "Albums");

            migrationBuilder.DropForeignKey(name: "FK_Images_AspNetUsers_AddedById", table: "Images");

            migrationBuilder.DropForeignKey(name: "FK_Albums_Images_CoverId", table: "Albums");

            migrationBuilder.DropTable(name: "AspNetRoleClaims");

            migrationBuilder.DropTable(name: "AspNetUserClaims");

            migrationBuilder.DropTable(name: "AspNetUserLogins");

            migrationBuilder.DropTable(name: "AspNetUserRoles");

            migrationBuilder.DropTable(name: "AspNetUserTokens");

            migrationBuilder.DropTable(name: "Comments");

            migrationBuilder.DropTable(name: "AspNetRoles");

            migrationBuilder.DropTable(name: "AspNetUsers");

            migrationBuilder.DropTable(name: "Images");

            migrationBuilder.DropTable(name: "Albums");

            migrationBuilder.DropTable(name: "ImageGpsDatas");
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns:
                table =>
                    new
                        {
                            Id = table.Column<string>(nullable: false),
                            ConcurrencyStamp = table.Column<string>(nullable: true),
                            Name = table.Column<string>(maxLength: 256, nullable: true),
                            NormalizedName = table.Column<string>(maxLength: 256, nullable: true)
                        },
                constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns:
                table =>
                    new
                        {
                            UserId = table.Column<string>(nullable: false),
                            LoginProvider = table.Column<string>(nullable: false),
                            Name = table.Column<string>(nullable: false),
                            Value = table.Column<string>(nullable: true)
                        },
                constraints:
                table => { table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name }); });

            migrationBuilder.CreateTable(
                name: "ImageGpsDatas",
                columns:
                table =>
                    new
                        {
                            Id = table.Column<Guid>(nullable: false),
                            CreatedOn = table.Column<DateTime>(nullable: false),
                            DeletedOn = table.Column<DateTime>(nullable: true),
                            IsDeleted = table.Column<bool>(nullable: false),
                            Latitude = table.Column<double>(nullable: false),
                            LocationName = table.Column<string>(maxLength: 200, nullable: false),
                            Longitude = table.Column<double>(nullable: false),
                            ModifiedOn = table.Column<DateTime>(nullable: true)
                        },
                constraints: table => { table.PrimaryKey("PK_ImageGpsDatas", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns:
                table =>
                    new
                        {
                            Id = table.Column<string>(nullable: false),
                            AccessFailedCount = table.Column<int>(nullable: false),
                            ConcurrencyStamp = table.Column<string>(nullable: true),
                            CreatedOn = table.Column<DateTime>(nullable: false),
                            DeletedOn = table.Column<DateTime>(nullable: true),
                            Email = table.Column<string>(maxLength: 256, nullable: true),
                            EmailConfirmed = table.Column<bool>(nullable: false),
                            FirstName = table.Column<string>(maxLength: 50, nullable: false),
                            IsDeleted = table.Column<bool>(nullable: false),
                            LastName = table.Column<string>(maxLength: 50, nullable: false),
                            LockoutEnabled = table.Column<bool>(nullable: false),
                            LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                            ModifiedOn = table.Column<DateTime>(nullable: true),
                            NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                            NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                            NotificationMask = table.Column<int>(nullable: false),
                            PasswordHash = table.Column<string>(nullable: true),
                            PhoneNumber = table.Column<string>(nullable: true),
                            PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                            SecurityStamp = table.Column<string>(nullable: true),
                            TwoFactorEnabled = table.Column<bool>(nullable: false),
                            UserName = table.Column<string>(maxLength: 256, nullable: true)
                        },
                constraints: table => { table.PrimaryKey("PK_AspNetUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns:
                table =>
                    new
                        {
                            Id =
                            table.Column<int>(nullable: false)
                                .Annotation(
                                    "SqlServer:ValueGenerationStrategy",
                                    SqlServerValueGenerationStrategy.IdentityColumn),
                            ClaimType = table.Column<string>(nullable: true),
                            ClaimValue = table.Column<string>(nullable: true),
                            RoleId = table.Column<string>(nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                            column: x => x.RoleId,
                            principalTable: "AspNetRoles",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns:
                table =>
                    new
                        {
                            Id =
                            table.Column<int>(nullable: false)
                                .Annotation(
                                    "SqlServer:ValueGenerationStrategy",
                                    SqlServerValueGenerationStrategy.IdentityColumn),
                            ClaimType = table.Column<string>(nullable: true),
                            ClaimValue = table.Column<string>(nullable: true),
                            UserId = table.Column<string>(nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                        table.ForeignKey(
                            name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns:
                table =>
                    new
                        {
                            LoginProvider = table.Column<string>(nullable: false),
                            ProviderKey = table.Column<string>(nullable: false),
                            ProviderDisplayName = table.Column<string>(nullable: true),
                            UserId = table.Column<string>(nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                        table.ForeignKey(
                            name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns:
                table =>
                    new
                        {
                            UserId = table.Column<string>(nullable: false),
                            RoleId = table.Column<string>(nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                        table.ForeignKey(
                            name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                            column: x => x.RoleId,
                            principalTable: "AspNetRoles",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                        table.ForeignKey(
                            name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns:
                table =>
                    new
                        {
                            Id = table.Column<Guid>(nullable: false),
                            AlbumId = table.Column<Guid>(nullable: true),
                            CreatedOn = table.Column<DateTime>(nullable: false),
                            Data = table.Column<string>(maxLength: 1000, nullable: false),
                            DeletedOn = table.Column<DateTime>(nullable: true),
                            ImageId = table.Column<Guid>(nullable: true),
                            IsDeleted = table.Column<bool>(nullable: false),
                            ModifiedOn = table.Column<DateTime>(nullable: true),
                            UserId = table.Column<string>(nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_Comments", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Comments_AspNetUsers_UserId",
                            column: x => x.UserId,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                    });

            migrationBuilder.CreateTable(
                name: "Images",
                columns:
                table =>
                    new
                        {
                            Id = table.Column<Guid>(nullable: false),
                            AddedById = table.Column<string>(nullable: true),
                            AlbumId = table.Column<Guid>(nullable: true),
                            Aperture = table.Column<string>(maxLength: 50, nullable: true),
                            CameraMaker = table.Column<string>(maxLength: 50, nullable: true),
                            CameraModel = table.Column<string>(maxLength: 50, nullable: true),
                            CreatedOn = table.Column<DateTime>(nullable: false),
                            DateTaken = table.Column<DateTime>(nullable: true),
                            DeletedOn = table.Column<DateTime>(nullable: true),
                            ExposureBiasStep = table.Column<string>(maxLength: 50, nullable: true),
                            FileName = table.Column<string>(maxLength: 200, nullable: false),
                            FocusLen = table.Column<string>(maxLength: 50, nullable: true),
                            Height = table.Column<int>(nullable: false),
                            ImageGpsDataId = table.Column<Guid>(nullable: true),
                            IsDeleted = table.Column<bool>(nullable: false),
                            Iso = table.Column<string>(maxLength: 50, nullable: true),
                            LowHeight = table.Column<int>(nullable: false),
                            LowWidth = table.Column<int>(nullable: false),
                            MidHeight = table.Column<int>(nullable: false),
                            MidWidth = table.Column<int>(nullable: false),
                            ModifiedOn = table.Column<DateTime>(nullable: true),
                            OriginalFileName = table.Column<string>(maxLength: 200, nullable: false),
                            ShutterSpeed = table.Column<string>(maxLength: 50, nullable: true),
                            Title = table.Column<string>(maxLength: 200, nullable: true),
                            Width = table.Column<int>(nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_Images", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Images_AspNetUsers_AddedById",
                            column: x => x.AddedById,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                        table.ForeignKey(
                            name: "FK_Images_ImageGpsDatas_ImageGpsDataId",
                            column: x => x.ImageGpsDataId,
                            principalTable: "ImageGpsDatas",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                    });

            migrationBuilder.CreateTable(
                name: "Albums",
                columns:
                table =>
                    new
                        {
                            Id = table.Column<Guid>(nullable: false),
                            Access = table.Column<int>(nullable: false),
                            AddedById = table.Column<string>(nullable: true),
                            CoverId = table.Column<Guid>(nullable: true),
                            CreatedOn = table.Column<DateTime>(nullable: false),
                            DeletedOn = table.Column<DateTime>(nullable: true),
                            Description = table.Column<string>(maxLength: 3000, nullable: true),
                            IsDeleted = table.Column<bool>(nullable: false),
                            ModifiedOn = table.Column<DateTime>(nullable: true),
                            Title = table.Column<string>(maxLength: 200, nullable: false)
                        },
                constraints: table =>
                    {
                        table.PrimaryKey("PK_Albums", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Albums_AspNetUsers_AddedById",
                            column: x => x.AddedById,
                            principalTable: "AspNetUsers",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                        table.ForeignKey(
                            name: "FK_Albums_Images_CoverId",
                            column: x => x.CoverId,
                            principalTable: "Images",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Restrict);
                    });

            migrationBuilder.CreateIndex(name: "RoleNameIndex", table: "AspNetRoles", column: "NormalizedName");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(name: "IX_AspNetUserRoles_RoleId", table: "AspNetUserRoles", column: "RoleId");

            migrationBuilder.CreateIndex(name: "IX_AspNetUserRoles_UserId", table: "AspNetUserRoles", column: "UserId");

            migrationBuilder.CreateIndex(name: "IX_Albums_AddedById", table: "Albums", column: "AddedById");

            migrationBuilder.CreateIndex(name: "IX_Albums_CoverId", table: "Albums", column: "CoverId");

            migrationBuilder.CreateIndex(name: "IX_Comments_AlbumId", table: "Comments", column: "AlbumId");

            migrationBuilder.CreateIndex(name: "IX_Comments_ImageId", table: "Comments", column: "ImageId");

            migrationBuilder.CreateIndex(name: "IX_Comments_UserId", table: "Comments", column: "UserId");

            migrationBuilder.CreateIndex(name: "IX_Images_AddedById", table: "Images", column: "AddedById");

            migrationBuilder.CreateIndex(name: "IX_Images_AlbumId", table: "Images", column: "AlbumId");

            migrationBuilder.CreateIndex(name: "IX_Images_ImageGpsDataId", table: "Images", column: "ImageGpsDataId");

            migrationBuilder.CreateIndex(name: "EmailIndex", table: "AspNetUsers", column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Images_ImageId",
                table: "Comments",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Albums_AlbumId",
                table: "Comments",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Albums_AlbumId",
                table: "Images",
                column: "AlbumId",
                principalTable: "Albums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}