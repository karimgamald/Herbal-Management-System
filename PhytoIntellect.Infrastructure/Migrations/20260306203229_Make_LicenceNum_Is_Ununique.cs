using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Make_LicenceNum_Is_Ununique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Herbalist_LicenseNumber",
                table: "Herbalist");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Herbalist",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Herbalist",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Herbalist_LicenseNumber",
                table: "Herbalist",
                column: "LicenseNumber",
                unique: true);
        }
    }
}
