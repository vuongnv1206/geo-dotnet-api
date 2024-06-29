using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class base_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsReactions_Post_PostId",
                schema: "Classroom",
                table: "NewsReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClasses_UserStudent_UserStudentId",
                schema: "Classroom",
                table: "UserClasses");

            migrationBuilder.DropTable(
                name: "UserStudent",
                schema: "Classroom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClasses",
                schema: "Classroom",
                table: "UserClasses");

            migrationBuilder.DropIndex(
                name: "IX_UserClasses_ClassesId",
                schema: "Classroom",
                table: "UserClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_NewsReactions",
                schema: "Classroom",
                table: "NewsReactions");

            migrationBuilder.RenameTable(
                name: "NewsReactions",
                schema: "Classroom",
                newName: "PostLike",
                newSchema: "Classroom");

            migrationBuilder.RenameColumn(
                name: "UserStudentId",
                schema: "Classroom",
                table: "UserClasses",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsReactions_PostId",
                schema: "Classroom",
                table: "PostLike",
                newName: "IX_PostLike_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClasses",
                schema: "Classroom",
                table: "UserClasses",
                columns: new[] { "ClassesId", "StudentId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_PostLike",
                schema: "Classroom",
                table: "PostLike",
                columns: new[] { "UserId", "PostId" });

            migrationBuilder.CreateTable(
                name: "Student",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    StudentCode = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<bool>(type: "boolean", nullable: true),
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Student", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClasses_StudentId",
                schema: "Classroom",
                table: "UserClasses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Student_ClassesId",
                schema: "Classroom",
                table: "Student",
                column: "ClassesId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLike_Post_PostId",
                schema: "Classroom",
                table: "PostLike",
                column: "PostId",
                principalSchema: "Classroom",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClasses_Student_StudentId",
                schema: "Classroom",
                table: "UserClasses",
                column: "StudentId",
                principalSchema: "Classroom",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostLike_Post_PostId",
                schema: "Classroom",
                table: "PostLike");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClasses_Student_StudentId",
                schema: "Classroom",
                table: "UserClasses");

            migrationBuilder.DropTable(
                name: "Student",
                schema: "Classroom");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClasses",
                schema: "Classroom",
                table: "UserClasses");

            migrationBuilder.DropIndex(
                name: "IX_UserClasses_StudentId",
                schema: "Classroom",
                table: "UserClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PostLike",
                schema: "Classroom",
                table: "PostLike");

            migrationBuilder.RenameTable(
                name: "PostLike",
                schema: "Classroom",
                newName: "NewsReactions",
                newSchema: "Classroom");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                schema: "Classroom",
                table: "UserClasses",
                newName: "UserStudentId");

            migrationBuilder.RenameIndex(
                name: "IX_PostLike_PostId",
                schema: "Classroom",
                table: "NewsReactions",
                newName: "IX_NewsReactions_PostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClasses",
                schema: "Classroom",
                table: "UserClasses",
                columns: new[] { "UserStudentId", "ClassesId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_NewsReactions",
                schema: "Classroom",
                table: "NewsReactions",
                columns: new[] { "UserId", "PostId" });

            migrationBuilder.CreateTable(
                name: "UserStudent",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<bool>(type: "boolean", nullable: true),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    StudentCode = table.Column<string>(type: "text", nullable: true),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserStudent_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserClasses_ClassesId",
                schema: "Classroom",
                table: "UserClasses",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_UserStudent_ClassesId",
                schema: "Classroom",
                table: "UserStudent",
                column: "ClassesId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsReactions_Post_PostId",
                schema: "Classroom",
                table: "NewsReactions",
                column: "PostId",
                principalSchema: "Classroom",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClasses_UserStudent_UserStudentId",
                schema: "Classroom",
                table: "UserClasses",
                column: "UserStudentId",
                principalSchema: "Classroom",
                principalTable: "UserStudent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
