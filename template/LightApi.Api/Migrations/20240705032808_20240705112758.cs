using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LightApi.Api.Migrations
{
    /// <inheritdoc />
    public partial class _20240705112758 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Test1",
                table: "Student",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Test2",
                table: "Student",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Test3",
                table: "Student",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Test4",
                table: "Student",
                type: "TEXT",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Test1",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Test2",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Test3",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Test4",
                table: "Student");
        }
    }
}
