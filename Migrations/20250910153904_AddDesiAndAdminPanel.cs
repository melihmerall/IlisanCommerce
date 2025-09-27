using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IlisanCommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddDesiAndAdminPanel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Desi",
                table: "ProductVariants",
                type: "decimal(8,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Desi",
                table: "Products",
                type: "decimal(8,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentToken",
                table: "Orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTransactionId",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MinDesi = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    MaxDesi = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FreeShippingThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedDeliveryDays = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminActivityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminUserId = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    OldValues = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminActivityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminActivityLogs_AdminUsers_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "AdminUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AdminUsers",
                columns: new[] { "Id", "CreatedDate", "Email", "FirstName", "IsActive", "IsEmailConfirmed", "LastLoginDate", "LastName", "PasswordHash", "PhoneNumber", "Role", "UpdatedDate" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@ilisan.com.tr", "Admin", true, true, null, "User", "$2a$11$K2iBUNCldhfj8DJwlnGuXeM5KQjH.vFYx.XgJzwJzQGvU8pq8Xv7W", null, 1, null });

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 1,
                column: "Desi",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 2,
                column: "Desi",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 3,
                column: "Desi",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: 4,
                column: "Desi",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Desi",
                value: 2.5m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Desi",
                value: 2.0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Desi",
                value: 1.5m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "Desi",
                value: 1.0m);

            migrationBuilder.InsertData(
                table: "ShippingRates",
                columns: new[] { "Id", "CreatedDate", "Description", "EstimatedDeliveryDays", "FreeShippingThreshold", "IsActive", "IsDefault", "MaxDesi", "MinDesi", "Name", "ShippingCost", "SortOrder", "UpdatedDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "1-5 desi arası ürünler için standart kargo", "1-2 gün", 500.00m, true, true, 5m, 0m, "Standart Kargo", 20.00m, 1, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "5-10 desi arası ürünler için ağır kargo", "2-3 gün", 1000.00m, true, false, 10m, 5m, "Ağır Kargo", 40.00m, 2, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "10 desi üzeri ürünler için özel kargo", "3-5 gün", 2000.00m, true, false, null, 10m, "Özel Kargo", 80.00m, 3, null }
                });

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

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "IsActive", "Key", "Type", "UpdatedDate", "Value" },
                values: new object[,]
                {
                    { 13, "Company", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8681), "Şirket adresi", true, "CompanyAddress", 1, null, "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş" },
                    { 14, "Company", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8682), "Çalışma saatleri", true, "CompanyWorkingHours", 1, null, "Pazartesi - Cuma: 08:00 - 18:00" },
                    { 15, "SocialMedia", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8683), "Facebook hesabı", true, "SocialMediaFacebook", 7, null, "#" },
                    { 16, "SocialMedia", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8684), "Instagram hesabı", true, "SocialMediaInstagram", 7, null, "https://instagram.com/ilisansavunma" },
                    { 17, "SocialMedia", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8686), "LinkedIn hesabı", true, "SocialMediaLinkedIn", 7, null, "#" },
                    { 18, "Company", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8687), "Kuruluş yılı", true, "CompanyFoundedYear", 1, null, "2018" },
                    { 19, "Company", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8688), "Vizyon", true, "CompanyVision", 3, null, "Savunma sanayinde dünya çapında tanınan, AR-GE odaklı bir lider firma olmak." },
                    { 20, "Company", new DateTime(2025, 9, 10, 18, 39, 3, 340, DateTimeKind.Local).AddTicks(8690), "Misyon", true, "CompanyMission", 3, null, "Milli savunma sanayinin geliştirilmesi ve tam bağımsızlığa katkı sağlamak." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivityLogs_AdminUserId",
                table: "AdminActivityLogs",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_Email",
                table: "AdminUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_IsDefault",
                table: "ShippingRates",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_MinDesi_MaxDesi",
                table: "ShippingRates",
                columns: new[] { "MinDesi", "MaxDesi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminActivityLogs");

            migrationBuilder.DropTable(
                name: "ShippingRates");

            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DropColumn(
                name: "Desi",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Desi",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentToken",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentTransactionId",
                table: "Orders");

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
        }
    }
}
