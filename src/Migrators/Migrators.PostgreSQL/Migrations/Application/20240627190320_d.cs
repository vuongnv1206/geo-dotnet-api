using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class d : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_News_NewsId",
                schema: "Classes",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsReactions_News_NewsId",
                schema: "Classes",
                table: "NewsReactions");

            migrationBuilder.DropTable(
                name: "News",
                schema: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Comments_NewsId",
                schema: "Classes",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "NewsId",
                schema: "Classes",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "NewsId",
                schema: "Classes",
                table: "NewsReactions",
                newName: "PostId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsReactions_NewsId",
                schema: "Classes",
                table: "NewsReactions",
                newName: "IX_NewsReactions_PostId");

            migrationBuilder.CreateTable(
                name: "Post",
                schema: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsLockComment = table.Column<bool>(type: "boolean", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classes",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                schema: "Classes",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ClassesId",
                schema: "Classes",
                table: "Post",
                column: "ClassesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Post_PostId",
                schema: "Classes",
                table: "Comments",
                column: "PostId",
                principalSchema: "Classes",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsReactions_Post_PostId",
                schema: "Classes",
                table: "NewsReactions",
                column: "PostId",
                principalSchema: "Classes",
                principalTable: "Post",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Post_PostId",
                schema: "Classes",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsReactions_Post_PostId",
                schema: "Classes",
                table: "NewsReactions");

            migrationBuilder.DropTable(
                name: "Post",
                schema: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostId",
                schema: "Classes",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "PostId",
                schema: "Classes",
                table: "NewsReactions",
                newName: "NewsId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsReactions_PostId",
                schema: "Classes",
                table: "NewsReactions",
                newName: "IX_NewsReactions_NewsId");

            migrationBuilder.AddColumn<Guid>(
                name: "NewsId",
                schema: "Classes",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "News",
                schema: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsLockComment = table.Column<bool>(type: "boolean", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classes",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_NewsId",
                schema: "Classes",
                table: "Comments",
                column: "NewsId");

            migrationBuilder.CreateIndex(
                name: "IX_News_ClassesId",
                schema: "Classes",
                table: "News",
                column: "ClassesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_News_NewsId",
                schema: "Classes",
                table: "Comments",
                column: "NewsId",
                principalSchema: "Classes",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsReactions_News_NewsId",
                schema: "Classes",
                table: "NewsReactions",
                column: "NewsId",
                principalSchema: "Classes",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
