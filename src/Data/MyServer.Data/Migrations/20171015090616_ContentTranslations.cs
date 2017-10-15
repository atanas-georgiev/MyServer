using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MyServer.Data.Migrations
{
    public partial class ContentTranslations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentValue",
                table: "StaticContents");

            migrationBuilder.AddColumn<string>(
                name: "ContentValueBg",
                table: "StaticContents",
                type: "nvarchar(max)",
                maxLength: 30000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentValueEn",
                table: "StaticContents",
                type: "nvarchar(max)",
                maxLength: 30000,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentValueBg",
                table: "StaticContents");

            migrationBuilder.DropColumn(
                name: "ContentValueEn",
                table: "StaticContents");

            migrationBuilder.AddColumn<string>(
                name: "ContentValue",
                table: "StaticContents",
                maxLength: 30000,
                nullable: false,
                defaultValue: "");
        }
    }
}
