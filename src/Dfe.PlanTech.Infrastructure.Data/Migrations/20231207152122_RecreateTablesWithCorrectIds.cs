﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dfe.PlanTech.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RecreateTablesWithCorrectIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RichTextDataDbEntity",
                schema: "Contentful",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RichTextDataDbEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RichTextContents",
                schema: "Contentful",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NodeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataId = table.Column<long>(type: "bigint", nullable: true),
                    RichTextContentDbEntityId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RichTextContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RichTextContents_RichTextContents_RichTextContentDbEntityId",
                        column: x => x.RichTextContentDbEntityId,
                        principalSchema: "Contentful",
                        principalTable: "RichTextContents",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RichTextContents_RichTextDataDbEntity_DataId",
                        column: x => x.DataId,
                        principalSchema: "Contentful",
                        principalTable: "RichTextDataDbEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ComponentDropDowns",
                schema: "Contentful",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RichTextContentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentDropDowns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentDropDowns_RichTextContents_RichTextContentId",
                        column: x => x.RichTextContentId,
                        principalSchema: "Contentful",
                        principalTable: "RichTextContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RichTextMarkDbEntity",
                schema: "Contentful",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RichTextContentDbEntityId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RichTextMarkDbEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RichTextMarkDbEntity_RichTextContents_RichTextContentDbEntityId",
                        column: x => x.RichTextContentDbEntityId,
                        principalSchema: "Contentful",
                        principalTable: "RichTextContents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TextBodies",
                schema: "Contentful",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RichTextId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextBodies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextBodies_ContentComponents_Id",
                        column: x => x.Id,
                        principalSchema: "Contentful",
                        principalTable: "ContentComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TextBodies_RichTextContents_RichTextId",
                        column: x => x.RichTextId,
                        principalSchema: "Contentful",
                        principalTable: "RichTextContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Warnings",
                schema: "Contentful",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TextId = table.Column<string>(type: "nvarchar(30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warnings_ContentComponents_Id",
                        column: x => x.Id,
                        principalSchema: "Contentful",
                        principalTable: "ContentComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Warnings_TextBodies_TextId",
                        column: x => x.TextId,
                        principalSchema: "Contentful",
                        principalTable: "TextBodies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentDropDowns_RichTextContentId",
                schema: "Contentful",
                table: "ComponentDropDowns",
                column: "RichTextContentId");

            migrationBuilder.CreateIndex(
                name: "IX_RichTextContents_DataId",
                schema: "Contentful",
                table: "RichTextContents",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_RichTextContents_RichTextContentDbEntityId",
                schema: "Contentful",
                table: "RichTextContents",
                column: "RichTextContentDbEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_RichTextMarkDbEntity_RichTextContentDbEntityId",
                schema: "Contentful",
                table: "RichTextMarkDbEntity",
                column: "RichTextContentDbEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_TextBodies_RichTextId",
                schema: "Contentful",
                table: "TextBodies",
                column: "RichTextId");

            migrationBuilder.CreateIndex(
                name: "IX_Warnings_TextId",
                schema: "Contentful",
                table: "Warnings",
                column: "TextId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentDropDowns",
                schema: "Contentful");

            migrationBuilder.DropTable(
                name: "RichTextMarkDbEntity",
                schema: "Contentful");

            migrationBuilder.DropTable(
                name: "Warnings",
                schema: "Contentful");

            migrationBuilder.DropTable(
                name: "TextBodies",
                schema: "Contentful");

            migrationBuilder.DropTable(
                name: "RichTextContents",
                schema: "Contentful");

            migrationBuilder.DropTable(
                name: "RichTextDataDbEntity",
                schema: "Contentful");
        }
    }
}