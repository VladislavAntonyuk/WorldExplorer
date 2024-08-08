using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                column: "Id",
                value: "19d3b2c7-8714-4851-ac73-95aeecfba3a6");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "19d3b2c7-8714-4851-ac73-95aeecfba3a6");
        }
    }
}
