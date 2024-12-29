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
                name: "inbox_messages",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inbox_messages", x => x.Id);
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
                name: "Travellers",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    VisitDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        name: "FK_Visits_Travellers_TravellerId",
                        column: x => x.TravellerId,
                        principalSchema: "travellers",
                        principalTable: "Travellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                schema: "travellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Visits_VisitId",
                        column: x => x.VisitId,
                        principalSchema: "travellers",
                        principalTable: "Visits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_VisitId",
                schema: "travellers",
                table: "Review",
                column: "VisitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_PlaceId",
                schema: "travellers",
                table: "Visits",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_TravellerId_PlaceId",
                schema: "travellers",
                table: "Visits",
                columns: new[] { "TravellerId", "PlaceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Review",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Visits",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Places",
                schema: "travellers");

            migrationBuilder.DropTable(
                name: "Travellers",
                schema: "travellers");
        }
    }
}
