using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class b : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassesId",
                schema: "Classes",
                table: "UserStudent",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserStudent_ClassesId",
                schema: "Classes",
                table: "UserStudent",
                column: "ClassesId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStudent_Classes_ClassesId",
                schema: "Classes",
                table: "UserStudent",
                column: "ClassesId",
                principalSchema: "Classes",
                principalTable: "Classes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStudent_Classes_ClassesId",
                schema: "Classes",
                table: "UserStudent");

            migrationBuilder.DropIndex(
                name: "IX_UserStudent_ClassesId",
                schema: "Classes",
                table: "UserStudent");

            migrationBuilder.DropColumn(
                name: "ClassesId",
                schema: "Classes",
                table: "UserStudent");
        }
    }
}
