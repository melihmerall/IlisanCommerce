using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IlisanCommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetToAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "AdminUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetTokenExpiry",
                table: "AdminUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordResetToken", "PasswordResetTokenExpiry" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(1845));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2540));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2542));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2544));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2545));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2546));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2547));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2548));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2549));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2551));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2552));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2553));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2554));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2555));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2557));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2558));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2559));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2560));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2561));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 11, 20, 52, 1, 549, DateTimeKind.Local).AddTicks(2563));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "AdminUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetTokenExpiry",
                table: "AdminUsers");

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8157));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8667));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8669));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8671));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8672));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8673));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8674));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8675));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8677));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8678));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8679));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8680));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8681));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8682));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8683));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8684));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8686));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8687));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8688));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8690));
        }
    }
}
