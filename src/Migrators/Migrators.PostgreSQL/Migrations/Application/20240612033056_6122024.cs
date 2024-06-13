using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class _6122024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmitPaperDetails",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "Question",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "NumberOfQuestion",
                schema: "Examination",
                table: "Papers");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Mark",
                schema: "Examination",
                table: "SubmitPaperDetails",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RawIndex",
                schema: "Examination",
                table: "PaperQuestions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmitPaperDetails",
                schema: "Examination",
                table: "SubmitPaperDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitPaperDetails_SubmitPaperId",
                schema: "Examination",
                table: "SubmitPaperDetails",
                column: "SubmitPaperId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmitPaperDetails",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropIndex(
                name: "IX_SubmitPaperDetails_SubmitPaperId",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "Mark",
                schema: "Examination",
                table: "SubmitPaperDetails");

            migrationBuilder.DropColumn(
                name: "RawIndex",
                schema: "Examination",
                table: "PaperQuestions");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                schema: "Question",
                table: "Questions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfQuestion",
                schema: "Examination",
                table: "Papers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmitPaperDetails",
                schema: "Examination",
                table: "SubmitPaperDetails",
                columns: new[] { "SubmitPaperId", "QuestionId" });
        }
    }
}
