using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_meditrack.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecurringIntervalHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecurringIntervalHours",
                table: "Prescriptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecurringIntervalHours",
                table: "Prescriptions",
                type: "int",
                nullable: true);
        }
    }
}
