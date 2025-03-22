using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shoell.Autobody.Data.Migrations
{
    /// <inheritdoc />
    public partial class Version000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Target = table.Column<string>(type: "nvarchar(max)", nullable: false, computedColumnSql: "(LEFT([Name], CHARINDEX('.', [Name]) - 1))", stored: true),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: false, computedColumnSql: "(RIGHT([Name], LEN([Name]) - CHARINDEX('.', [Name])))", stored: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, computedColumnSql: "([UserName])", stored: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExternalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false, computedColumnSql: "(CAST (CASE WHEN [DateArchived] IS NULL THEN 0 ELSE 1 END AS BIT))", stored: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateArchived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchivedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Users_Users_ArchivedById",
                        column: x => x.ArchivedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Users_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_DeletedById",
                        column: x => x.DeletedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Role_Claims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Claims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false, computedColumnSql: "(CAST (CASE WHEN [DateArchived] IS NULL THEN 0 ELSE 1 END AS BIT))", stored: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateArchived = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArchivedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Groups_Users_ArchivedById",
                        column: x => x.ArchivedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Groups_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Groups_Users_DeletedById",
                        column: x => x.DeletedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Groups_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false, computedColumnSql: "([Type] + ' ' + [Action])", stored: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User_Claims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Claims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Logins",
                schema: "Identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_User_Logins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Roles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Roles", x => new { x.UserId, x.RoleId })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_User_Roles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Roles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Tokens",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_User_Tokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role_Groups",
                schema: "Identity",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role_Groups", x => new { x.RoleId, x.GroupId })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Role_Groups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Identity",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_Groups_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_Groups",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Groups", x => new { x.UserId, x.GroupId })
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_User_Groups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "Identity",
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Groups_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
DECLARE @username nvarchar(max) = 'spencer.shoell@wazitech.com';
DECLARE @externalId nvarchar(max) = 'dfaa0b46-21c0-4261-92d8-db63d97684b8';

INSERT INTO [Identity].[Users]
        ([Id],[ExternalId],[DateCreated],[DateModified],[DateArchived],[DateDeleted],[CreatedById],[ModifiedById],[ArchivedById],[DeletedById],[UserName],[NormalizedUserName],[Email],[NormalizedEmail],[EmailConfirmed],[PasswordHash],[SecurityStamp],[ConcurrencyStamp],[PhoneNumber],[PhoneNumberConfirmed],[TwoFactorEnabled],[LockoutEnd],[LockoutEnabled],[AccessFailedCount])
    VALUES
        ('00000000-0000-0000-0000-000000000000','',GETDATE(),GETDATE(),NULL,NULL,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',NULL,NULL,'administrator@app.local',UPPER('administrator@app.local'),'administrator@app.local',UPPER('administrator@app.local'),0,'',NEWID(),NEWID(),'',0,0,NULL,0,0),
        ('b95befc9-72ab-4a2e-a896-29c125dc31b9',@externalId,GETDATE(),GETDATE(),NULL,NULL,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',NULL,NULL,@username,UPPER(@username),@username,UPPER(@username),0,'',NEWID(),NEWID(),'',0,0,NULL,0,0);
");

            migrationBuilder.Sql(@"
INSERT INTO [Identity].[Roles]
        ([Id],[Name],[NormalizedName],[ConcurrencyStamp])
    VALUES
        (NEWID(),'Group.Create',UPPER('Group.Create'),NEWID()),
        (NEWID(),'Group.Read',UPPER('Group.Read'),NEWID()),
        (NEWID(),'Group.Update',UPPER('Group.Update'),NEWID()),
        (NEWID(),'Group.Delete',UPPER('Group.Delete'),NEWID()),
        (NEWID(),'Group.Recycle',UPPER('Group.Recycle'),NEWID()),
        (NEWID(),'Group.Recover',UPPER('Group.Recover'),NEWID()),
        (NEWID(),'Group.Purge',UPPER('Group.Purge'),NEWID()),
        (NEWID(),'Group.Archive',UPPER('Group.Archive'),NEWID()),
        (NEWID(),'Group.Restore',UPPER('Group.Restore'),NEWID()),

        (NEWID(),'Role.Read',UPPER('Role.Read'),NEWID()),

        (NEWID(),'User.Create',UPPER('User.Create'),NEWID()),
        (NEWID(),'User.Read',UPPER('User.Read'),NEWID()),
        (NEWID(),'User.Update',UPPER('User.Update'),NEWID()),
        (NEWID(),'User.Delete',UPPER('User.Delete'),NEWID()),
        (NEWID(),'User.Recycle',UPPER('User.Recycle'),NEWID()),
        (NEWID(),'User.Recover',UPPER('User.Recover'),NEWID()),
        (NEWID(),'User.Purge',UPPER('User.Purge'),NEWID()),
        (NEWID(),'User.Archive',UPPER('User.Archive'),NEWID()),
        (NEWID(),'User.Restore',UPPER('User.Restore'),NEWID())
");

            migrationBuilder.Sql(@"
INSERT INTO [Identity].[Groups]
            ([Id],[Name],[Description],[DateCreated],[DateModified],[DateArchived],[DateDeleted],[CreatedById],[ModifiedById],[ArchivedById],[DeletedById])
        VALUES
            ('720362b3-097b-4bb8-9321-37b899de6f4e','Admins','Adminsitrator Users',GETDATE(),GETDATE(),NULL,NULL,'00000000-0000-0000-0000-000000000000','00000000-0000-0000-0000-000000000000',NULL,NULL);
");

            migrationBuilder.Sql(@"
INSERT INTO [Identity].[Role_Groups] ([RoleId], [GroupId])
    SELECT
            [Id]
        ,'720362b3-097b-4bb8-9321-37b899de6f4e'
    FROM [Identity].[Roles]
");

            migrationBuilder.Sql(@"
INSERT INTO [Identity].[User_Groups]
            ([UserId],[GroupId])
        VALUES
            ('b95befc9-72ab-4a2e-a896-29c125dc31b9','720362b3-097b-4bb8-9321-37b899de6f4e'),
            ('00000000-0000-0000-0000-000000000000','720362b3-097b-4bb8-9321-37b899de6f4e');
            ");

            migrationBuilder.Sql(@"
CREATE PROCEDURE [Identity].[sp_ApplyPermissions]
    AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    TRUNCATE TABLE [Identity].[User_Roles];

    INSERT INTO [Identity].[User_Roles]
        ([UserId]
        ,[RoleId])
    SELECT DISTINCT  
            [User_Groups].[UserId]
        ,[Role_Groups].[RoleId]
    FROM [Identity].[User_Groups]
        INNER JOIN [Identity].[Role_Groups]
            ON [User_Groups].[GroupId] = [Role_Groups].[GroupId]
		INNER JOIN [Identity].[Groups]
			ON [Groups].[Id] = [User_Groups].[GroupId]
		INNER JOIN [Identity].[Users]
			ON [Users].[Id] = [User_Groups].[UserId]
	WHERE [Groups].[DateDeleted] IS NULL
		AND [Users].[DateDeleted] IS NULL

    COMMIT TRANSACTION
END
GO

EXEC [Identity].[sp_ApplyPermissions];
");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ArchivedById",
                schema: "Identity",
                table: "Groups",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CreatedById",
                schema: "Identity",
                table: "Groups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_DeletedById",
                schema: "Identity",
                table: "Groups",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ModifiedById",
                schema: "Identity",
                table: "Groups",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_RowId",
                schema: "Identity",
                table: "Groups",
                column: "RowId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                schema: "System",
                table: "Logs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Claims_RoleId",
                schema: "Identity",
                table: "Role_Claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Groups_GroupId",
                schema: "Identity",
                table: "Role_Groups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Groups_RowId",
                schema: "Identity",
                table: "Role_Groups",
                column: "RowId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RowId",
                schema: "Identity",
                table: "Roles",
                column: "RowId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_Claims_UserId",
                schema: "Identity",
                table: "User_Claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Groups_GroupId",
                schema: "Identity",
                table: "User_Groups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Groups_RowId",
                schema: "Identity",
                table: "User_Groups",
                column: "RowId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Logins_UserId",
                schema: "Identity",
                table: "User_Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Roles_RoleId",
                schema: "Identity",
                table: "User_Roles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Roles_RowId",
                schema: "Identity",
                table: "User_Roles",
                column: "RowId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ArchivedById",
                schema: "Identity",
                table: "Users",
                column: "ArchivedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                schema: "Identity",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedById",
                schema: "Identity",
                table: "Users",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ModifiedById",
                schema: "Identity",
                table: "Users",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RowId",
                schema: "Identity",
                table: "Users",
                column: "RowId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE [Identity].[sp_ApplyPermissions];");

            migrationBuilder.DropTable(
                name: "Logs",
                schema: "System");

            migrationBuilder.DropTable(
                name: "Role_Claims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Role_Groups",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "User_Claims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "User_Groups",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "User_Logins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "User_Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "User_Tokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Groups",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");
        }
    }
}
