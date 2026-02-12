using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixApplicationUniqueIndexFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChannelApplications_ChannelId_AdvertiserId",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropIndex(
                name: "IX_CampaignApplications_CampaignId_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelApplications_ChannelId_AdvertiserId",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                columns: new[] { "ChannelId", "AdvertiserId" },
                unique: true,
                filter: "\"Status\" NOT IN (2, 3)");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignApplications_CampaignId_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                columns: new[] { "CampaignId", "ChannelId" },
                unique: true,
                filter: "\"Status\" NOT IN (2, 3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChannelApplications_ChannelId_AdvertiserId",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropIndex(
                name: "IX_CampaignApplications_CampaignId_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelApplications_ChannelId_AdvertiserId",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                columns: new[] { "ChannelId", "AdvertiserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignApplications_CampaignId_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                columns: new[] { "CampaignId", "ChannelId" },
                unique: true);
        }
    }
}
