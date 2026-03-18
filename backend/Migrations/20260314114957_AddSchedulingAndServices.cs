using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddSchedulingAndServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProviderServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    PriceEur = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderServices_Users_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProviderWorkingHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    IsWorking = table.Column<bool>(type: "boolean", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderWorkingHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProviderWorkingHours_Users_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProviderServiceId = table.Column<int>(type: "integer", nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    StartUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_ProviderServices_ProviderServiceId",
                        column: x => x.ProviderServiceId,
                        principalTable: "ProviderServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientId",
                table: "Appointments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ProviderServiceId",
                table: "Appointments",
                column: "ProviderServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderServices_ProviderId",
                table: "ProviderServices",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProviderWorkingHours_ProviderId_DayOfWeek",
                table: "ProviderWorkingHours",
                columns: new[] { "ProviderId", "DayOfWeek" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "ProviderWorkingHours");

            migrationBuilder.DropTable(
                name: "ProviderServices");
        }
    }
}
