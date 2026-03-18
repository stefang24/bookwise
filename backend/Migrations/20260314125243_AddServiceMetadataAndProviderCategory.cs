using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceMetadataAndProviderCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryCategory",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProviderServices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProviderServices",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrimaryCategory",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProviderServices");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProviderServices");
        }
    }
}
