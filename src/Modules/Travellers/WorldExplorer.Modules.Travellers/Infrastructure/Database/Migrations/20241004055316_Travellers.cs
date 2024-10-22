using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldExplorer.Modules.Travellers.Infrastructure.Database.Migrations
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
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Places",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Places", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
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
                name: "Visits",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TravellerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visits_Places_PlaceId",
                        column: x => x.PlaceId,
                        principalSchema: "travellers",
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visits_Review_ReviewId",
                        column: x => x.ReviewId,
                        principalSchema: "travellers",
                        principalTable: "Review",
                        principalColumn: "Id");
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

            migrationBuilder.InsertData(
                schema: "travellers",
                table: "Travellers",
                column: "Id",
                value: new Guid("19d3b2c7-8714-4851-ac73-95aeecfba3a6"));

            migrationBuilder.CreateIndex(
                name: "IX_TravellerRoute_TravellerId",
                schema: "travellers",
                table: "TravellerRoute",
                column: "TravellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PlaceId",
                schema: "travellers",
                table: "Visits",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ReviewId",
                schema: "travellers",
                table: "Visits",
                column: "ReviewId");
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
                name: "Visits",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Travellers",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Places",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Review",
                schema: "travellers");
        }
    }
}
