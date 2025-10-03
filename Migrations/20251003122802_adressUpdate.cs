using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IlisanCommerce.Migrations
{
    /// <inheritdoc />
    public partial class adressUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Body",
                value: "\n                        <h2>Sayın {CustomerName},</h2>\n                        <p>Siparişiniz başarıyla alınmıştır.</p>\n                        <p><strong>Sipariş No:</strong> {OrderNumber}</p>\n                        <p><strong>Sipariş Tarihi:</strong> {OrderDate}</p>\n                        <p><strong>Toplam Tutar:</strong> {TotalAmount} TL</p>\n                        <p>Siparişiniz en kısa sürede hazırlanacaktır.</p>\n                        <p>Teşekkür ederiz.</p>\n                        <p><strong>ILISAN Savunma Sanayi</strong></p>\n                    ");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Body",
                value: "\n                        <h2>Yeni Sipariş Alındı</h2>\n                        <p><strong>Sipariş No:</strong> {OrderNumber}</p>\n                        <p><strong>Müşteri:</strong> {CustomerName}</p>\n                        <p><strong>Email:</strong> {CustomerEmail}</p>\n                        <p><strong>Toplam Tutar:</strong> {TotalAmount} TL</p>\n                        <p><strong>Sipariş Tarihi:</strong> {OrderDate}</p>\n                        <p>Lütfen siparişi kontrol edip onaylayınız.</p>\n                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 1,
                column: "Body",
                value: "\r\n                        <h2>Sayın {CustomerName},</h2>\r\n                        <p>Siparişiniz başarıyla alınmıştır.</p>\r\n                        <p><strong>Sipariş No:</strong> {OrderNumber}</p>\r\n                        <p><strong>Sipariş Tarihi:</strong> {OrderDate}</p>\r\n                        <p><strong>Toplam Tutar:</strong> {TotalAmount} TL</p>\r\n                        <p>Siparişiniz en kısa sürede hazırlanacaktır.</p>\r\n                        <p>Teşekkür ederiz.</p>\r\n                        <p><strong>ILISAN Savunma Sanayi</strong></p>\r\n                    ");

            migrationBuilder.UpdateData(
                table: "EmailTemplates",
                keyColumn: "Id",
                keyValue: 2,
                column: "Body",
                value: "\r\n                        <h2>Yeni Sipariş Alındı</h2>\r\n                        <p><strong>Sipariş No:</strong> {OrderNumber}</p>\r\n                        <p><strong>Müşteri:</strong> {CustomerName}</p>\r\n                        <p><strong>Email:</strong> {CustomerEmail}</p>\r\n                        <p><strong>Toplam Tutar:</strong> {TotalAmount} TL</p>\r\n                        <p><strong>Sipariş Tarihi:</strong> {OrderDate}</p>\r\n                        <p>Lütfen siparişi kontrol edip onaylayınız.</p>\r\n                    ");
        }
    }
}
