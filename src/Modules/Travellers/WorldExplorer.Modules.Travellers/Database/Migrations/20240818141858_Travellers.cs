using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldExplorer.Modules.Travellers.Database.Migrations
{
    /// <inheritdoc />
    public partial class Travellers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "travellers");

            migrationBuilder.CreateTable(
                name: "inbox_message_consumers",
                schema: "travellers",
                columns: table => new
                {
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_message_consumers", x => new { x.InboxMessageId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_message_consumers",
                schema: "travellers",
                columns: table => new
                {
                    OutboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_message_consumers", x => new { x.OutboxMessageId, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Travellers",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travellers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravellerRoute",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Locations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravellerRoute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravellerRoute_Travellers_TravellerId",
                        column: x => x.TravellerId,
                        principalSchema: "travellers",
                        principalTable: "Travellers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TravellerRoute_TravellerId",
                schema: "travellers",
                table: "TravellerRoute",
                column: "TravellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox_message_consumers",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "outbox_message_consumers",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "TravellerRoute",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Travellers",
                schema: "travellers");
        }
    }
}
