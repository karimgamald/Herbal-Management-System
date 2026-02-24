using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Version_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Herbalists_Users_UserId",
                table: "Herbalists");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_MedicalHistories_MedicalHistoryId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_MedicalHistoryId",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Herbalists",
                table: "Herbalists");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "Patient");

            migrationBuilder.RenameTable(
                name: "MedicalHistories",
                newName: "MedicalHistory");

            migrationBuilder.RenameTable(
                name: "Herbalists",
                newName: "Herbalist");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_UserId",
                table: "Patient",
                newName: "IX_Patient_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Herbalists_UserId",
                table: "Herbalist",
                newName: "IX_Herbalist_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patient",
                table: "Patient",
                column: "PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistory",
                table: "MedicalHistory",
                column: "MedicalHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Herbalist",
                table: "Herbalist",
                column: "HerbalistId");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TokenHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_MedicalHistoryId",
                table: "Patient",
                column: "MedicalHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Herbalist_Users_UserId",
                table: "Herbalist",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_MedicalHistory_MedicalHistoryId",
                table: "Patient",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistory",
                principalColumn: "MedicalHistoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patient_Users_UserId",
                table: "Patient",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Herbalist_Users_UserId",
                table: "Herbalist");

            migrationBuilder.DropForeignKey(
                name: "FK_Patient_MedicalHistory_MedicalHistoryId",
                table: "Patient");

            migrationBuilder.DropForeignKey(
                name: "FK_Patient_Users_UserId",
                table: "Patient");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patient",
                table: "Patient");

            migrationBuilder.DropIndex(
                name: "IX_Patient_MedicalHistoryId",
                table: "Patient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MedicalHistory",
                table: "MedicalHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Herbalist",
                table: "Herbalist");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Patient",
                newName: "Patients");

            migrationBuilder.RenameTable(
                name: "MedicalHistory",
                newName: "MedicalHistories");

            migrationBuilder.RenameTable(
                name: "Herbalist",
                newName: "Herbalists");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Patient_UserId",
                table: "Patients",
                newName: "IX_Patients_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Herbalist_UserId",
                table: "Herbalists",
                newName: "IX_Herbalists_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Users",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MedicalHistories",
                table: "MedicalHistories",
                column: "MedicalHistoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Herbalists",
                table: "Herbalists",
                column: "HerbalistId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_MedicalHistoryId",
                table: "Patients",
                column: "MedicalHistoryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Herbalists_Users_UserId",
                table: "Herbalists",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_MedicalHistories_MedicalHistoryId",
                table: "Patients",
                column: "MedicalHistoryId",
                principalTable: "MedicalHistories",
                principalColumn: "MedicalHistoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Users_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
