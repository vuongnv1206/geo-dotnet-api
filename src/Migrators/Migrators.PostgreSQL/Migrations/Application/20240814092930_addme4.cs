﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class addme4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Question");

            migrationBuilder.EnsureSchema(
                name: "Assignment");

            migrationBuilder.EnsureSchema(
                name: "Auditing");

            migrationBuilder.EnsureSchema(
                name: "Classroom");

            migrationBuilder.EnsureSchema(
                name: "GroupTeacher");

            migrationBuilder.EnsureSchema(
                name: "Notification");

            migrationBuilder.EnsureSchema(
                name: "Payment");

            migrationBuilder.EnsureSchema(
                name: "Examination");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "Subject");

            migrationBuilder.CreateTable(
                name: "AuditTrails",
                schema: "Auditing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: true),
                    TableName = table.Column<string>(type: "text", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    AffectedColumns = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditTrails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupClasses",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
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
                    table.PrimaryKey("PK_GroupClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupTeachers",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    JoinLink = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_GroupTeachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InviteJoinTeacherTeam",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientEmail = table.Column<string>(type: "text", nullable: false),
                    SenderEmail = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_InviteJoinTeacherTeam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JoinTeacherTeamRequest",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdminTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    SenderEmail = table.Column<string>(type: "text", nullable: false),
                    InvitationId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_JoinTeacherTeamRequest", x => x.Id);
                });

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
                name: "PaperFolders",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_PaperFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaperFolders_PaperFolders_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Examination",
                        principalTable: "PaperFolders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaperLabels",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_PaperLabels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaperMatrices",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    TotalPoint = table.Column<float>(type: "real", nullable: false),
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
                    table.PrimaryKey("PK_PaperMatrices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionFolders",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_QuestionFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionFolders_QuestionFolders_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Question",
                        principalTable: "QuestionFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuestionLabels",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_QuestionLabels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                schema: "Subject",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Subject", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    Image = table.Column<string>(type: "text", nullable: false),
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
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeacherTeams",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeacherName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_TeacherTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                schema: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionID = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    Gender = table.Column<bool>(type: "boolean", nullable: true),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ObjectId = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    SchoolYear = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", maxLength: 256, nullable: false),
                    GroupClassId = table.Column<Guid>(type: "uuid", maxLength: 256, nullable: true),
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
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_GroupClasses_GroupClassId",
                        column: x => x.GroupClassId,
                        principalSchema: "Classroom",
                        principalTable: "GroupClasses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaperFolderPermissions",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_PaperFolderPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaperFolderPermissions_GroupTeachers_GroupTeacherId",
                        column: x => x.GroupTeacherId,
                        principalSchema: "GroupTeacher",
                        principalTable: "GroupTeachers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaperFolderPermissions_PaperFolders_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "Examination",
                        principalTable: "PaperFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionFolderPermissions",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupTeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionFolderId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_QuestionFolderPermissions", x => x.Id);
                    table.UniqueConstraint("AK_QuestionFolderPermissions_QuestionFolderId_UserId_GroupTeac~", x => new { x.QuestionFolderId, x.UserId, x.GroupTeacherId });
                    table.ForeignKey(
                        name: "FK_QuestionFolderPermissions_QuestionFolders_QuestionFolderId",
                        column: x => x.QuestionFolderId,
                        principalSchema: "Question",
                        principalTable: "QuestionFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Audio = table.Column<string>(type: "text", nullable: true),
                    QuestionFolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionType = table.Column<int>(type: "integer", nullable: true),
                    QuestionLableId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionStatus = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionFolders_QuestionFolderId",
                        column: x => x.QuestionFolderId,
                        principalSchema: "Question",
                        principalTable: "QuestionFolders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Questions_QuestionLabels_QuestionLableId",
                        column: x => x.QuestionLableId,
                        principalSchema: "Question",
                        principalTable: "QuestionLabels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Questions_Questions_QuestionParentId",
                        column: x => x.QuestionParentId,
                        principalSchema: "Question",
                        principalTable: "Questions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignment",
                schema: "Assignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Attachment = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CanViewResult = table.Column<bool>(type: "boolean", nullable: false),
                    RequireLoginToSubmit = table.Column<bool>(type: "boolean", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Assignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignment_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Subject",
                        principalTable: "Subject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Papers",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamName = table.Column<string>(type: "text", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsPublish = table.Column<bool>(type: "boolean", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Shuffle = table.Column<bool>(type: "boolean", nullable: false),
                    ShowMarkResult = table.Column<int>(type: "integer", nullable: false),
                    ShowQuestionAnswer = table.Column<int>(type: "integer", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: true),
                    ExamCode = table.Column<string>(type: "text", nullable: false),
                    NumberAttempt = table.Column<int>(type: "integer", nullable: true),
                    ShareType = table.Column<int>(type: "integer", nullable: false),
                    PublicIpAllowed = table.Column<string>(type: "text", nullable: true),
                    LocalIpAllowed = table.Column<string>(type: "text", nullable: true),
                    PaperLabelId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaperFolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Papers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Papers_PaperFolders_PaperFolderId",
                        column: x => x.PaperFolderId,
                        principalSchema: "Examination",
                        principalTable: "PaperFolders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Papers_PaperLabels_PaperLabelId",
                        column: x => x.PaperLabelId,
                        principalSchema: "Examination",
                        principalTable: "PaperLabels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Papers_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalSchema: "Subject",
                        principalTable: "Subject",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Total = table.Column<decimal>(type: "numeric", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsExpired = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "Payment",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JoinGroupTeacherRequest",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_JoinGroupTeacherRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinGroupTeacherRequest_GroupTeachers_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "GroupTeacher",
                        principalTable: "GroupTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JoinGroupTeacherRequest_TeacherTeams_TeacherId",
                        column: x => x.TeacherId,
                        principalSchema: "GroupTeacher",
                        principalTable: "TeacherTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherInGroups",
                schema: "GroupTeacher",
                columns: table => new
                {
                    TeacherTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupTeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherInGroups", x => new { x.TeacherTeamId, x.GroupTeacherId });
                    table.ForeignKey(
                        name: "FK_TeacherInGroups_GroupTeachers_GroupTeacherId",
                        column: x => x.GroupTeacherId,
                        principalSchema: "GroupTeacher",
                        principalTable: "GroupTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherInGroups_TeacherTeams_TeacherTeamId",
                        column: x => x.TeacherTeamId,
                        principalSchema: "GroupTeacher",
                        principalTable: "TeacherTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupPermissionInClasses",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupTeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_GroupPermissionInClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupPermissionInClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupPermissionInClasses_GroupTeachers_GroupTeacherId",
                        column: x => x.GroupTeacherId,
                        principalSchema: "GroupTeacher",
                        principalTable: "GroupTeachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    IsLockComment = table.Column<bool>(type: "boolean", nullable: false),
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
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StId = table.Column<Guid>(type: "uuid", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "TeacherPermissionInClasses",
                schema: "GroupTeacher",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherTeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_TeacherPermissionInClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherPermissionInClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherPermissionInClasses_TeacherTeams_TeacherTeamId",
                        column: x => x.TeacherTeamId,
                        principalSchema: "GroupTeacher",
                        principalTable: "TeacherTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Question",
                        principalTable: "Questions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuestionClones",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    Audio = table.Column<string>(type: "text", nullable: true),
                    QuestionFolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionType = table.Column<int>(type: "integer", nullable: true),
                    QuestionLabelId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuestionParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    OriginalQuestionId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_QuestionClones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionClones_QuestionClones_QuestionParentId",
                        column: x => x.QuestionParentId,
                        principalSchema: "Question",
                        principalTable: "QuestionClones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionClones_QuestionFolders_QuestionFolderId",
                        column: x => x.QuestionFolderId,
                        principalSchema: "Question",
                        principalTable: "QuestionFolders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionClones_QuestionLabels_QuestionLabelId",
                        column: x => x.QuestionLabelId,
                        principalSchema: "Question",
                        principalTable: "QuestionLabels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionClones_Questions_OriginalQuestionId",
                        column: x => x.OriginalQuestionId,
                        principalSchema: "Question",
                        principalTable: "Questions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssignmentClass",
                schema: "Assignment",
                columns: table => new
                {
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentClass", x => new { x.AssignmentId, x.ClassesId });
                    table.ForeignKey(
                        name: "FK_AssignmentClass_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "Assignment",
                        principalTable: "Assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentClass_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaperAccesses",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaperId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClassId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaperAccesses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaperAccesses_Papers_PaperId",
                        column: x => x.PaperId,
                        principalSchema: "Examination",
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaperPermissions",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
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
                name: "SubmitPapers",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaperId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalMark = table.Column<float>(type: "real", nullable: false),
                    DeviceId = table.Column<string>(type: "text", nullable: true),
                    DeviceName = table.Column<string>(type: "text", nullable: true),
                    DeviceType = table.Column<string>(type: "text", nullable: true),
                    PublicIp = table.Column<string>(type: "text", nullable: true),
                    LocalIp = table.Column<string>(type: "text", nullable: true),
                    canResume = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_SubmitPapers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitPapers_Papers_PaperId",
                        column: x => x.PaperId,
                        principalSchema: "Examination",
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "Classroom",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "Classroom",
                        principalTable: "Comments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Post_PostId",
                        column: x => x.PostId,
                        principalSchema: "Classroom",
                        principalTable: "Post",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PostLike",
                schema: "Classroom",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLike", x => new { x.UserId, x.PostId });
                    table.ForeignKey(
                        name: "FK_PostLike_Post_PostId",
                        column: x => x.PostId,
                        principalSchema: "Classroom",
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssignmentStudent",
                schema: "Assignment",
                columns: table => new
                {
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AnswerRaw = table.Column<string>(type: "text", nullable: true),
                    AttachmentPath = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Score = table.Column<float>(type: "real", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignmentStudent", x => new { x.AssignmentId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_AssignmentStudent_Assignment_AssignmentId",
                        column: x => x.AssignmentId,
                        principalSchema: "Assignment",
                        principalTable: "Assignment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssignmentStudent_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssignmentStudent_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Classroom",
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClasses",
                schema: "Classroom",
                columns: table => new
                {
                    ClassesId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClasses", x => new { x.ClassesId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_UserClasses_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalSchema: "Classroom",
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClasses_Student_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "Classroom",
                        principalTable: "Student",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnswerClones",
                schema: "Question",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    QuestionCloneId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_AnswerClones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerClones_QuestionClones_QuestionCloneId",
                        column: x => x.QuestionCloneId,
                        principalSchema: "Question",
                        principalTable: "QuestionClones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PaperQuestions",
                schema: "Examination",
                columns: table => new
                {
                    PaperId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Mark = table.Column<float>(type: "real", nullable: false),
                    RawIndex = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperQuestions", x => new { x.PaperId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_PaperQuestions_Papers_PaperId",
                        column: x => x.PaperId,
                        principalSchema: "Examination",
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaperQuestions_QuestionClones_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Question",
                        principalTable: "QuestionClones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmitPaperDetails",
                schema: "Examination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmitPaperId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerRaw = table.Column<string>(type: "text", nullable: true),
                    Mark = table.Column<float>(type: "real", nullable: true),
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
                    table.PrimaryKey("PK_SubmitPaperDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmitPaperDetails_QuestionClones_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Question",
                        principalTable: "QuestionClones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmitPaperDetails_SubmitPapers_SubmitPaperId",
                        column: x => x.SubmitPaperId,
                        principalSchema: "Examination",
                        principalTable: "SubmitPapers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "CommentLikes",
                schema: "Classroom",
                columns: table => new
                {
                    CommentId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentLikes", x => new { x.CommentId, x.UserId });
                    table.ForeignKey(
                        name: "FK_CommentLikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "Classroom",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerClones_QuestionCloneId",
                schema: "Question",
                table: "AnswerClones",
                column: "QuestionCloneId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId",
                schema: "Question",
                table: "Answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_SubjectId",
                schema: "Assignment",
                table: "Assignment",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentClass_ClassesId",
                schema: "Assignment",
                table: "AssignmentClass",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentStudent_ClassesId",
                schema: "Assignment",
                table: "AssignmentStudent",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentStudent_StudentId",
                schema: "Assignment",
                table: "AssignmentStudent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_GroupClassId",
                schema: "Classroom",
                table: "Classes",
                column: "GroupClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                schema: "Classroom",
                table: "Comments",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                schema: "Classroom",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissionInClasses_ClassId",
                schema: "GroupTeacher",
                table: "GroupPermissionInClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupPermissionInClasses_GroupTeacherId",
                schema: "GroupTeacher",
                table: "GroupPermissionInClasses",
                column: "GroupTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroupTeacherRequest_GroupId",
                schema: "GroupTeacher",
                table: "JoinGroupTeacherRequest",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroupTeacherRequest_TeacherId",
                schema: "GroupTeacher",
                table: "JoinGroupTeacherRequest",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SubscriptionId",
                schema: "Payment",
                table: "Orders",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperAccesses_ClassId",
                schema: "Examination",
                table: "PaperAccesses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperAccesses_PaperId",
                schema: "Examination",
                table: "PaperAccesses",
                column: "PaperId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperFolderPermissions_FolderId",
                schema: "Examination",
                table: "PaperFolderPermissions",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperFolderPermissions_GroupTeacherId",
                schema: "Examination",
                table: "PaperFolderPermissions",
                column: "GroupTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperFolders_ParentId",
                schema: "Examination",
                table: "PaperFolders",
                column: "ParentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PaperQuestions_QuestionId",
                schema: "Examination",
                table: "PaperQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Papers_PaperFolderId",
                schema: "Examination",
                table: "Papers",
                column: "PaperFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Papers_PaperLabelId",
                schema: "Examination",
                table: "Papers",
                column: "PaperLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_Papers_SubjectId",
                schema: "Examination",
                table: "Papers",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ClassesId",
                schema: "Classroom",
                table: "Post",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_PostLike_PostId",
                schema: "Classroom",
                table: "PostLike",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionClones_OriginalQuestionId",
                schema: "Question",
                table: "QuestionClones",
                column: "OriginalQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionClones_QuestionFolderId",
                schema: "Question",
                table: "QuestionClones",
                column: "QuestionFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionClones_QuestionLabelId",
                schema: "Question",
                table: "QuestionClones",
                column: "QuestionLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionClones_QuestionParentId",
                schema: "Question",
                table: "QuestionClones",
                column: "QuestionParentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionFolders_ParentId",
                schema: "Question",
                table: "QuestionFolders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionFolderId",
                schema: "Question",
                table: "Questions",
                column: "QuestionFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionLableId",
                schema: "Question",
                table: "Questions",
                column: "QuestionLableId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionParentId",
                schema: "Question",
                table: "Questions",
                column: "QuestionParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Identity",
                table: "Roles",
                columns: new[] { "NormalizedName", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_ClassesId",
                schema: "Classroom",
                table: "Student",
                column: "ClassesId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitPaperDetails_QuestionId",
                schema: "Examination",
                table: "SubmitPaperDetails",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitPaperDetails_SubmitPaperId",
                schema: "Examination",
                table: "SubmitPaperDetails",
                column: "SubmitPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitPaperLogs_SubmitPaperId",
                schema: "Examination",
                table: "SubmitPaperLogs",
                column: "SubmitPaperId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmitPapers_PaperId",
                schema: "Examination",
                table: "SubmitPapers",
                column: "PaperId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherInGroups_GroupTeacherId",
                schema: "GroupTeacher",
                table: "TeacherInGroups",
                column: "GroupTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPermissionInClasses_ClassId",
                schema: "GroupTeacher",
                table: "TeacherPermissionInClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPermissionInClasses_TeacherTeamId",
                schema: "GroupTeacher",
                table: "TeacherPermissionInClasses",
                column: "TeacherTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClasses_StudentId",
                schema: "Classroom",
                table: "UserClasses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_LoginProvider_ProviderKey_TenantId",
                schema: "Identity",
                table: "UserLogins",
                columns: new[] { "LoginProvider", "ProviderKey", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "Users",
                columns: new[] { "NormalizedUserName", "TenantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerClones",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "Answers",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "AssignmentClass",
                schema: "Assignment");

            migrationBuilder.DropTable(
                name: "AssignmentStudent",
                schema: "Assignment");

            migrationBuilder.DropTable(
                name: "AuditTrails",
                schema: "Auditing");

            migrationBuilder.DropTable(
                name: "CommentLikes",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "GroupPermissionInClasses",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "InviteJoinTeacherTeam",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "JoinGroupTeacherRequest",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "JoinTeacherTeamRequest",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "Notification");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Payment");

            migrationBuilder.DropTable(
                name: "PaperAccesses",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "PaperFolderPermissions",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "PaperMatrices",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "PaperPermissions",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "PaperQuestions",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "PostLike",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "QuestionFolderPermissions",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "SubmitPaperDetails",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "SubmitPaperLogs",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "TeacherInGroups",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "TeacherPermissionInClasses",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "Transactions",
                schema: "Payment");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserClasses",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Assignment",
                schema: "Assignment");

            migrationBuilder.DropTable(
                name: "Comments",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "Payment");

            migrationBuilder.DropTable(
                name: "QuestionClones",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "SubmitPapers",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "GroupTeachers",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "TeacherTeams",
                schema: "GroupTeacher");

            migrationBuilder.DropTable(
                name: "Student",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Post",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "Papers",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "Classes",
                schema: "Classroom");

            migrationBuilder.DropTable(
                name: "QuestionFolders",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "QuestionLabels",
                schema: "Question");

            migrationBuilder.DropTable(
                name: "PaperFolders",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "PaperLabels",
                schema: "Examination");

            migrationBuilder.DropTable(
                name: "Subject",
                schema: "Subject");

            migrationBuilder.DropTable(
                name: "GroupClasses",
                schema: "Classroom");
        }
    }
}
