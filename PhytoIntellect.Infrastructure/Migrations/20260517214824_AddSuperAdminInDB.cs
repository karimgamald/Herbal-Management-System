using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhytoIntellect.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminInDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "City", "CreatedAt", "Email", "EmailConfirmationToken", "EmailConfirmationTokenExpiry", "FullName", "Governorate", "IsEmailConfirmed", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "Phone", "Role", "Street", "UserName" },
                values: new object[] { 999, "Menofia", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "herbal.ai200@gmail.com", null, null, "Super Admin", "Menofia", true, "$2a$12$uiBZ/NOR7RYt6xd.NBX.J./x.IlrlVx3IDp8GZQ3pstkzOckPTtLK", null, null, "01000000000", "Admin", "Menofia", "super_admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 999);
        }
    }
}
