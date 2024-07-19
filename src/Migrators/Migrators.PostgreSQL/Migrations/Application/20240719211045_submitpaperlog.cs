using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class submitpaperlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubmitPaperLogs",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmitPaperId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: true),
                    DeviceName = table.Column<string>(type: "text", nullable: true),
                    DeviceType = table.Column<string>(type: "text", nullable: true),
                    PublicIp = table.Column<string>(type: "text", nullable: true),
                    LocalIp = table.Column<string>(type: "text", nullable: true),
                    ProcessLog = table.Column<string>(type: "text", nullable: true),
                    MouseLog = table.Column<string>(type: "text", nullable: true),
                    KeyboardLog = table.Column<string>(type: "text", nullable: true),
                    NetworkLog = table.Column<string>(type: "text", nullable: true),
                    IsSuspicious = table.Column<bool>(type: "boolean", nullable: true),
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
                    table.PrimaryKey("PK_SubmitPaperLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitPaperLogs_SubmitPapers_SubmitPaperId",
                        column: x => x.SubmitPaperId,
                        principalSchema: "Examination",
                        principalTable: "SubmitPapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmitPaperLogs_SubmitPaperId",
                schema: "Examination",
                table: "SubmitPaperLogs",
                column: "SubmitPaperId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmitPaperLogs",
                schema: "Examination");
        }
    }
}
