using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IlisanCommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddIyzicoPaymentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "AltText", "CreatedDate", "ImagePath", "IsMainImage", "ProductId", "SortOrder" },
                values: new object[,]
                {
                    { 5, "ILISAN BV-01 Yan Görünüm", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "/images/demo/demo_370x530.png", false, 1, 2 },
                    { 6, "ILISAN BV-02 Detay", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "/images/demo/demo_370x530.png", false, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "Id", "Color", "ColorCode", "CreatedDate", "Dimensions", "IsActive", "IsDefault", "Material", "MinStockLevel", "PriceAdjustment", "ProductId", "SKU", "Size", "SortOrder", "StockQuantity", "UpdatedDate", "VariantName", "Weight" },
                values: new object[,]
                {
                    { 1, "Siyah", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, true, "Bor + Polietilen", 2, 0.00m, 1, "BV01-S-BLK", "S", 1, 15, null, "Small", null },
                    { 2, "Siyah", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Bor + Polietilen", 3, 0.00m, 1, "BV01-M-BLK", "M", 2, 20, null, "Medium", null },
                    { 3, "Siyah", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, false, "Bor + Polietilen", 2, 100.00m, 1, "BV01-L-BLK", "L", 3, 15, null, "Large", null },
                    { 4, "Kamuflaj", null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, true, true, "Aramid + Seramik", 2, 0.00m, 2, "BV02-M-CAM", "M", 1, 12, null, "Kamuflaj - Medium", null }
                });

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8142));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8797));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8801));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8802));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8803));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8804));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8805));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8806));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8807));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8808));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8809));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 5, 9, 57, 45, 611, DateTimeKind.Local).AddTicks(8810));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CompanyName", "CreatedDate", "Email", "EmailConfirmationToken", "FirstName", "IsActive", "IsEmailConfirmed", "LastLoginDate", "LastName", "PasswordHash", "PasswordResetToken", "PasswordResetTokenExpiry", "Phone", "Role", "TaxNumber" },
                values: new object[,]
                {
                    { 2, null, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "ahmet@test.com", null, "Ahmet", true, true, null, "Yılmaz", "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", null, null, null, 1, null },
                    { 3, null, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "ayse@test.com", null, "Ayşe", true, true, null, "Demir", "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", null, null, null, 1, null },
                    { 4, null, new DateTime(2024, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "mehmet@test.com", null, "Mehmet", true, true, null, "Kaya", "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", null, null, null, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 786, DateTimeKind.Local).AddTicks(5634));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 786, DateTimeKind.Local).AddTicks(5721));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 786, DateTimeKind.Local).AddTicks(5723));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 786, DateTimeKind.Local).AddTicks(5724));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(4825));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5294));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5297));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5298));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5299));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5300));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5301));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5303));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5304));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5305));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5306));

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedDate",
                value: new DateTime(2025, 9, 3, 9, 0, 1, 785, DateTimeKind.Local).AddTicks(5307));
        }
    }
}
