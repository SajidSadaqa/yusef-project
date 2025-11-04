using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipmentTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerReferenceAndReferenceNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "customer_reference",
                table: "shipments",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_shipments_reference_number",
                table: "shipments",
                column: "reference_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_shipments_reference_number",
                table: "shipments");

            migrationBuilder.DropColumn(
                name: "customer_reference",
                table: "shipments");
        }
    }
}
