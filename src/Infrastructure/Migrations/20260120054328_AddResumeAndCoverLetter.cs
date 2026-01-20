using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeAndCoverLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResumeText",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResumeUpdatedAt",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CoverLetterGeneratedAt",
                table: "Applications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverLetterHtml",
                table: "Applications",
                type: "character varying(15000)",
                maxLength: 15000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResumeText",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResumeUpdatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CoverLetterGeneratedAt",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "CoverLetterHtml",
                table: "Applications");
        }
    }
}
