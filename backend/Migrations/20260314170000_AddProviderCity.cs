using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using backend.Data;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260314170000_AddProviderCity")]
    public partial class AddProviderCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");
        }
    }
}
