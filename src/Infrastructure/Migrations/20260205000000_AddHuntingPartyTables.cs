using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260205000000_AddHuntingPartyTables")]
    public partial class AddHuntingPartyTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                -- Create HuntingParties table
                IF OBJECT_ID(N'[HuntingParties]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [HuntingParties] (
                        [Id] uniqueidentifier NOT NULL,
                        [Name] nvarchar(100) NOT NULL,
                        [InviteCode] nvarchar(8) NOT NULL,
                        [CreatorId] nvarchar(450) NOT NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        CONSTRAINT [PK_HuntingParties] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_HuntingParties_AspNetUsers_CreatorId] FOREIGN KEY ([CreatorId])
                            REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
                    );

                    CREATE UNIQUE INDEX [IX_HuntingParties_InviteCode] ON [HuntingParties] ([InviteCode]);
                END

                -- Create HuntingPartyMemberships table
                IF OBJECT_ID(N'[HuntingPartyMemberships]', N'U') IS NULL
                BEGIN
                    CREATE TABLE [HuntingPartyMemberships] (
                        [Id] uniqueidentifier NOT NULL,
                        [HuntingPartyId] uniqueidentifier NOT NULL,
                        [UserId] nvarchar(450) NOT NULL,
                        [Role] int NOT NULL,
                        [JoinedDate] datetime2 NOT NULL,
                        [IsActive] bit NOT NULL CONSTRAINT [DF_HuntingPartyMemberships_IsActive] DEFAULT 1,
                        CONSTRAINT [PK_HuntingPartyMemberships] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_HuntingPartyMemberships_HuntingParties_HuntingPartyId] FOREIGN KEY ([HuntingPartyId])
                            REFERENCES [HuntingParties] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_HuntingPartyMemberships_AspNetUsers_UserId] FOREIGN KEY ([UserId])
                            REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
                    );

                    CREATE UNIQUE INDEX [IX_HuntingPartyMemberships_HuntingPartyId_UserId]
                        ON [HuntingPartyMemberships] ([HuntingPartyId], [UserId]);
                END
                """
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[HuntingPartyMemberships]', N'U') IS NOT NULL
                    DROP TABLE [HuntingPartyMemberships];

                IF OBJECT_ID(N'[HuntingParties]', N'U') IS NOT NULL
                    DROP TABLE [HuntingParties];
                """
            );
        }
    }
}
