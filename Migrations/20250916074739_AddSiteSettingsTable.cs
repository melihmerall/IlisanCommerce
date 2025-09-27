using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IlisanCommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 12);

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
                name: "CreatedDate",
                table: "SiteSettings");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "SiteSettings",
                newName: "SortOrder");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SiteSettings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "SiteSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SiteSettings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SiteSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "General",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "SiteSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SiteSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "SiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SiteSettings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataType",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SiteSettings");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "SiteSettings",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SiteSettings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "SiteSettings",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SiteSettings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "SiteSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "General");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "SiteSettings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "SiteSettings",
                columns: new[] { "Id", "Category", "CreatedDate", "Description", "IsActive", "Key", "Type", "UpdatedDate", "Value" },
                values: new object[,]
                {
                    { 1, "General", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(2655), "Site adı", true, "SiteName", 1, null, "ILISAN Savunma Sanayi" },
                    { 2, "General", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3410), "Site açıklaması", true, "SiteDescription", 1, null, "Tam bağımsız Türkiye için hiç durmadan çalışmaya devam edeceğiz." },
                    { 3, "General", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3413), "Site logosu", true, "Logo", 2, null, "/images/logo/logo_2.png" },
                    { 4, "Contact", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3435), "Telefon numarası", true, "Phone", 1, null, "+90 (850) 532 5237" },
                    { 5, "Contact", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3437), "Email adresi", true, "Email", 6, null, "info@ilisan.com.tr" },
                    { 6, "Contact", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3438), "Adres", true, "Address", 1, null, "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş" },
                    { 7, "Contact", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3439), "Çalışma saatleri", true, "WorkingHours", 1, null, "24/7 Destek" },
                    { 8, "Social", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3440), "Facebook sayfası", true, "FacebookUrl", 7, null, "#" },
                    { 9, "Social", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3442), "Instagram sayfası", true, "InstagramUrl", 7, null, "https://instagram.com/ilisansavunma" },
                    { 10, "Social", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3443), "LinkedIn sayfası", true, "LinkedInUrl", 7, null, "#" },
                    { 11, "HomePage", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3444), "Hakkımızda başlığı", true, "AboutUsTitle", 1, null, "Biz Kimiz ?" },
                    { 12, "HomePage", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3445), "Hakkımızda metni", true, "AboutUsText", 3, null, "2018'de AR-GE çalışmalarına başladığımız süreçten bu yana patentli ürünlerimizi sektörde rakipsiz hale getirdik." },
                    { 13, "Company", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3446), "Şirket adresi", true, "CompanyAddress", 1, null, "Üngüt Mah. 71093.Sk 14/C Onikişubat / Kahramanmaraş" },
                    { 14, "Company", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3447), "Çalışma saatleri", true, "CompanyWorkingHours", 1, null, "Pazartesi - Cuma: 08:00 - 18:00" },
                    { 15, "SocialMedia", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3449), "Facebook hesabı", true, "SocialMediaFacebook", 7, null, "#" },
                    { 16, "SocialMedia", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3450), "Instagram hesabı", true, "SocialMediaInstagram", 7, null, "https://instagram.com/ilisansavunma" },
                    { 17, "SocialMedia", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3451), "LinkedIn hesabı", true, "SocialMediaLinkedIn", 7, null, "#" },
                    { 18, "Company", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3452), "Kuruluş yılı", true, "CompanyFoundedYear", 1, null, "2018" },
                    { 19, "Company", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3453), "Vizyon", true, "CompanyVision", 3, null, "Savunma sanayinde dünya çapında tanınan, AR-GE odaklı bir lider firma olmak." },
                    { 20, "Company", new DateTime(2025, 9, 15, 20, 2, 25, 595, DateTimeKind.Local).AddTicks(3454), "Misyon", true, "CompanyMission", 3, null, "Milli savunma sanayinin geliştirilmesi ve tam bağımsızlığa katkı sağlamak." }
                });
        }
    }
}
