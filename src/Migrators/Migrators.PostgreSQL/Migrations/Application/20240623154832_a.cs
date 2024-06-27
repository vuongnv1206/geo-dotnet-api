using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class a : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Classes",
                table: "UserClasses");

            migrationBuilder.DropColumn(
                name: "IsGender",
                schema: "Classes",
                table: "UserClasses");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                schema: "Classes",
                table: "UserClasses");

            migrationBuilder.DropColumn(
                name: "StudentCode",
                schema: "Classes",
                table: "UserClasses");

            migrationBuilder.EnsureSchema(
                name: "Notification");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "Classes",
                table: "UserClasses",
                newName: "UserStudentId");

            migrationBuilder.AddColumn<bool>(
                name: "CanShare",
                schema: "Examination",
                table: "PaperFolderPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "Notification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Label = table.Column<string>(type: "varchar(50)", nullable: false),
                    Message = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Url = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaperPermissions",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaperId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupTeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    CanView = table.Column<bool>(type: "boolean", nullable: false),
                    CanAdd = table.Column<bool>(type: "boolean", nullable: false),
                    CanUpdate = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CanShare = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaperPermissions_GroupTeachers_GroupTeacherId",
                        column: x => x.GroupTeacherId,
                        principalSchema: "GroupTeacher",
                        principalTable: "GroupTeachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaperPermissions_Papers_PaperId",
                        column: x => x.PaperId,
                        principalSchema: "Examination",
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserStudent",
                schema: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    StudentCode = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<bool>(type: "boolean", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStudent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaperPermissions_GroupTeacherId",
                schema: "Examination",
                table: "PaperPermissions",
                column: "GroupTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperPermissions_PaperId",
                schema: "Examination",
                table: "PaperPermissions",
                column: "PaperId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClasses_UserStudent_UserStudentId",
                schema: "Classes",
                table: "UserClasses",
                column: "UserStudentId",
                principalSchema: "Classes",
                principalTable: "UserStudent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClasses_UserStudent_UserStudentId",
                schema: "Classes",
                table: "UserClasses");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "Notification");

            migrationBuilder.DropTable(
                name: "PaperPermissions",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "UserStudent",
                schema: "Classes");

            migrationBuilder.DropColumn(
                name: "CanShare",
                schema: "Examination",
                table: "PaperFolderPermissions");

            migrationBuilder.RenameColumn(
                name: "UserStudentId",
                schema: "Classes",
                table: "UserClasses",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Classes",
                table: "UserClasses",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsGender",
                schema: "Classes",
                table: "UserClasses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                schema: "Classes",
                table: "UserClasses",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                schema: "Classes",
                table: "UserClasses",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
