using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class updatequestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                schema: "Examination",
                table: "SubmitPapers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceName",
                schema: "Examination",
                table: "SubmitPapers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                schema: "Examination",
                table: "SubmitPapers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocalIp",
                schema: "Examination",
                table: "SubmitPapers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicIp",
                schema: "Examination",
                table: "SubmitPapers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LocalIpAllowed",
                schema: "Examination",
                table: "Papers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicIpAllowed",
                schema: "Examination",
                table: "Papers",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                schema: "Examination",
                table: "SubmitPapers");

            migrationBuilder.DropColumn(
                name: "DeviceName",
                schema: "Examination",
                table: "SubmitPapers");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                schema: "Examination",
                table: "SubmitPapers");

            migrationBuilder.DropColumn(
                name: "LocalIp",
                schema: "Examination",
                table: "SubmitPapers");

            migrationBuilder.DropColumn(
                name: "PublicIp",
                schema: "Examination",
                table: "SubmitPapers");

            migrationBuilder.DropColumn(
                name: "LocalIpAllowed",
                schema: "Examination",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "PublicIpAllowed",
                schema: "Examination",
                table: "Papers");
        }
    }
}
