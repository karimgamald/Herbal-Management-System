using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveMedicalHistoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MedicalHistory_MedicalHistoryId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicalHistoryId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "MedicalHistoryId",
                table: "Patients");

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "MedicalHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistory_PatientId",
                table: "MedicalHistory",
                column: "PatientId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalHistory_Patients_PatientId",
                table: "MedicalHistory",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalHistory_Patients_PatientId",
                table: "MedicalHistory");

            migrationBuilder.DropIndex(
                name: "IX_MedicalHistory_PatientId",
                table: "MedicalHistory");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "MedicalHistory");

            migrationBuilder.AddColumn<int>(
                name: "MedicalHistoryId",
                table: "Patients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicalHistoryId",
                table: "Patients",
                column: "MedicalHistoryId",
                unique: true,
                filter: "[MedicalHistoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MedicalHistory_MedicalHistoryId",
                table: "Patients",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistory",
                principalColumn: "MedicalHistoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
