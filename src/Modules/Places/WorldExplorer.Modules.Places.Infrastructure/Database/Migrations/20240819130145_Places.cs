using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace WorldExplorer.Modules.Places.Infrastructure.Database.Migrations
{
	/// <inheritdoc />
	public partial class Places : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.EnsureSchema(
				name: "places");

			migrationBuilder.CreateTable(
				name: "inbox_message_consumers",
				schema: "places",
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
				schema: "places",
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
				schema: "places",
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
				schema: "places",
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
				name: "Places",
				schema: "places",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Location = table.Column<Point>(type: "geography", nullable: false),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Images = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Places", x => x.Id);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "inbox_message_consumers",
				schema: "places");

			migrationBuilder.DropTable(
				name: "inbox_messages",
				schema: "places");

			migrationBuilder.DropTable(
				name: "outbox_message_consumers",
				schema: "places");

			migrationBuilder.DropTable(
				name: "outbox_messages",
				schema: "places");

			migrationBuilder.DropTable(
				name: "Places",
				schema: "places");
		}
	}
}
