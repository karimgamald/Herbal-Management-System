using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatientGenderToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MedicalHistory_MedicalHistoryId",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MedicalHistory_MedicalHistoryId",
                table: "Patients",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistory",
                principalColumn: "MedicalHistoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MedicalHistory_MedicalHistoryId",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Patients",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MedicalHistory_MedicalHistoryId",
                table: "Patients",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistory",
                principalColumn: "MedicalHistoryId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
