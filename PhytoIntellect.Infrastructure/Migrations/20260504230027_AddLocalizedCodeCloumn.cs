using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocalizedCodeCloumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "ReviewRecipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "MedicalHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Herbs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "Diseases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LanguageCode",
                table: "AiRecipes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 1,
                column: "LanguageCode",
                value: "en");

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 2,
                column: "LanguageCode",
                value: "en");

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 3,
                column: "LanguageCode",
                value: "en");

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 4,
                column: "LanguageCode",
                value: "en");

            migrationBuilder.UpdateData(
                table: "Diseases",
                keyColumn: "DiseaseId",
                keyValue: 5,
                column: "LanguageCode",
                value: "en");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "ReviewRecipes");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "MedicalHistories");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Herbs");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "Diseases");

            migrationBuilder.DropColumn(
                name: "LanguageCode",
                table: "AiRecipes");
        }
    }
}
