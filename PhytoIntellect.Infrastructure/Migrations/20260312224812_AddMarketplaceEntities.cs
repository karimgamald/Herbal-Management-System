using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Herb",
                columns: table => new
                {
                    HerbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HerbName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ScientificName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Benefits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Warnings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AddedByHerbalistId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Herb", x => x.HerbId);
                    table.ForeignKey(
                        name: "FK_Herb_Herbalist_AddedByHerbalistId",
                        column: x => x.AddedByHerbalistId,
                        principalTable: "Herbalist",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Recipe",
                columns: table => new
                {
                    RecipeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HerbalistId = table.Column<int>(type: "int", nullable: true),
                    CreatedByAI = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Instructions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AverageRating = table.Column<float>(type: "real", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipe", x => x.RecipeId);
                    table.ForeignKey(
                        name: "FK_Recipe_Herbalist_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalist",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HerbalistHerb",
                columns: table => new
                {
                    HerbalistId = table.Column<int>(type: "int", nullable: false),
                    HerbId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerbalistHerb", x => new { x.HerbalistId, x.HerbId });
                    table.ForeignKey(
                        name: "FK_HerbalistHerb_Herb_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herb",
                        principalColumn: "HerbId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HerbalistHerb_Herbalist_HerbalistId",
                        column: x => x.HerbalistId,
                        principalTable: "Herbalist",
                        principalColumn: "HerbalistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecipeHerb",
                columns: table => new
                {
                    RecipeHerbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    HerbId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeHerb", x => x.RecipeHerbId);
                    table.ForeignKey(
                        name: "FK_RecipeHerb_Herb_HerbId",
                        column: x => x.HerbId,
                        principalTable: "Herb",
                        principalColumn: "HerbId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecipeHerb_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "RecipeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Herb_AddedByHerbalistId",
                table: "Herb",
                column: "AddedByHerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_HerbalistHerb_HerbId",
                table: "HerbalistHerb",
                column: "HerbId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipe_HerbalistId",
                table: "Recipe",
                column: "HerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHerb_HerbId",
                table: "RecipeHerb",
                column: "HerbId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeHerb_RecipeId",
                table: "RecipeHerb",
                column: "RecipeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HerbalistHerb");

            migrationBuilder.DropTable(
                name: "RecipeHerb");

            migrationBuilder.DropTable(
                name: "Herb");

            migrationBuilder.DropTable(
                name: "Recipe");
        }
    }
}
