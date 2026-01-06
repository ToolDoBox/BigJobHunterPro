using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260119090000_AddApplicationsTable")]
    public partial class AddApplicationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Applications]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [Applications] (
                        [Id] uniqueidentifier NOT NULL,
                        [UserId] nvarchar(450) NOT NULL,
                        [CompanyName] nvarchar(200) NOT NULL,
                        [RoleTitle] nvarchar(200) NOT NULL,
                        [SourceName] nvarchar(100) NOT NULL,
                        [SourceUrl] nvarchar(500) NULL,
                        [Status] int NOT NULL,
                        [WorkMode] int NULL,
                        [Location] nvarchar(200) NULL,
                        [SalaryMin] int NULL,
                        [SalaryMax] int NULL,
                        [JobDescription] nvarchar(4000) NULL,
                        [RequiredSkills] nvarchar(max) NOT NULL CONSTRAINT [DF_Applications_RequiredSkills] DEFAULT N'[]',
                        [NiceToHaveSkills] nvarchar(max) NOT NULL CONSTRAINT [DF_Applications_NiceToHaveSkills] DEFAULT N'[]',
                        [ParsedByAI] bit NOT NULL CONSTRAINT [DF_Applications_ParsedByAI] DEFAULT 0,
                        [AiParsingStatus] int NOT NULL CONSTRAINT [DF_Applications_AiParsingStatus] DEFAULT 0,
                        [LastAIParsedDate] datetime2 NULL,
                        [RawPageContent] nvarchar(max) NULL,
                        [Points] int NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [UpdatedDate] datetime2 NOT NULL,
                        CONSTRAINT [PK_Applications] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_Applications_AspNetUsers_UserId] FOREIGN KEY ([UserId])
                            REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );
                END
                ELSE
                BEGIN
                    IF COL_LENGTH('Applications', 'WorkMode') IS NULL
                        ALTER TABLE [Applications] ADD [WorkMode] int NULL;
                    IF COL_LENGTH('Applications', 'Location') IS NULL
                        ALTER TABLE [Applications] ADD [Location] nvarchar(200) NULL;
                    IF COL_LENGTH('Applications', 'SalaryMin') IS NULL
                        ALTER TABLE [Applications] ADD [SalaryMin] int NULL;
                    IF COL_LENGTH('Applications', 'SalaryMax') IS NULL
                        ALTER TABLE [Applications] ADD [SalaryMax] int NULL;
                    IF COL_LENGTH('Applications', 'JobDescription') IS NULL
                        ALTER TABLE [Applications] ADD [JobDescription] nvarchar(4000) NULL;
                    IF COL_LENGTH('Applications', 'RequiredSkills') IS NULL
                        ALTER TABLE [Applications] ADD [RequiredSkills] nvarchar(max) NOT NULL CONSTRAINT [DF_Applications_RequiredSkills] DEFAULT N'[]';
                    IF COL_LENGTH('Applications', 'NiceToHaveSkills') IS NULL
                        ALTER TABLE [Applications] ADD [NiceToHaveSkills] nvarchar(max) NOT NULL CONSTRAINT [DF_Applications_NiceToHaveSkills] DEFAULT N'[]';
                    IF COL_LENGTH('Applications', 'ParsedByAI') IS NULL
                        ALTER TABLE [Applications] ADD [ParsedByAI] bit NOT NULL CONSTRAINT [DF_Applications_ParsedByAI] DEFAULT 0;
                    IF COL_LENGTH('Applications', 'AiParsingStatus') IS NULL
                        ALTER TABLE [Applications] ADD [AiParsingStatus] int NOT NULL CONSTRAINT [DF_Applications_AiParsingStatus] DEFAULT 0;
                    IF COL_LENGTH('Applications', 'LastAIParsedDate') IS NULL
                        ALTER TABLE [Applications] ADD [LastAIParsedDate] datetime2 NULL;
                    IF COL_LENGTH('Applications', 'RawPageContent') IS NULL
                        ALTER TABLE [Applications] ADD [RawPageContent] nvarchar(max) NULL;
                END

                IF COL_LENGTH('AspNetUsers', 'TotalPoints') IS NULL
                    ALTER TABLE [AspNetUsers] ADD [TotalPoints] int NOT NULL
                    CONSTRAINT [DF_AspNetUsers_TotalPoints] DEFAULT 0;

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'IX_Applications_CreatedDate'
                      AND object_id = OBJECT_ID(N'[Applications]')
                )
                    CREATE INDEX [IX_Applications_CreatedDate] ON [Applications] ([CreatedDate]);

                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'IX_Applications_UserId'
                      AND object_id = OBJECT_ID(N'[Applications]')
                )
                    CREATE INDEX [IX_Applications_UserId] ON [Applications] ([UserId]);
                """
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Applications]', N'U') IS NOT NULL
                    DROP TABLE [Applications];
                """
            );
        }
    }
}
