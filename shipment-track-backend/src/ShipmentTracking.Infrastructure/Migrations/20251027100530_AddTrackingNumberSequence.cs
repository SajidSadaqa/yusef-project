using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<long>(
                name: "tracking_number_seq",
                startValue: 1000L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "tracking_number_seq");
        }
    }
}
