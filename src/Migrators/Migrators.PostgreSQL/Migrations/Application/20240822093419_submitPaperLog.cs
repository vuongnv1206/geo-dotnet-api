using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class submitPaperLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommonLog",
                schema: "Examination",
                table: "SubmitPaperLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReassignLog",
                schema: "Examination",
                table: "SubmitPaperLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Duration",
                schema: "Examination",
                table: "Papers",
                type: "real",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommonLog",
                schema: "Examination",
                table: "SubmitPaperLogs");

            migrationBuilder.DropColumn(
                name: "ReassignLog",
                schema: "Examination",
                table: "SubmitPaperLogs");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                schema: "Examination",
                table: "Papers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);
        }
    }
}
