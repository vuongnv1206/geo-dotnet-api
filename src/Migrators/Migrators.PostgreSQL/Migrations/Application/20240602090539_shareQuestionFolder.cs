using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class shareQuestionFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanShare",
                schema: "Question",
                table: "QuestionFolderPermissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanShare",
                schema: "Question",
                table: "QuestionFolderPermissions");
        }
    }
}
