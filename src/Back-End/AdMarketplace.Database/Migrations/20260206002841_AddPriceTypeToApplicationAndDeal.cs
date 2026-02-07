using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceTypeToApplicationAndDeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "PriceType",
                schema: "AdMarketplace",
                table: "Deals",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "ProposedPriceType",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "smallint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceType",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ProposedPriceType",
                schema: "AdMarketplace",
                table: "CampaignApplications");
        }
    }
}
