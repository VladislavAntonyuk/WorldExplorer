using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace WebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSrid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Places",
                type: "POINT",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "POINT")
                .Annotation("Sqlite:Srid", 4326);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "LocationInfoRequests",
                type: "POINT",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "POINT")
                .Annotation("Sqlite:Srid", 4326);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Places",
                type: "POINT",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "POINT")
                .OldAnnotation("Sqlite:Srid", 4326);

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "LocationInfoRequests",
                type: "POINT",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "POINT")
                .OldAnnotation("Sqlite:Srid", 4326);
        }
    }
}
