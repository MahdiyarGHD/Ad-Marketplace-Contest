using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixDealInvitationRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_CampaignInvitations_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "InvitationId1",
                schema: "AdMarketplace",
                table: "Deals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvitationId1",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals",
                column: "InvitationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_CampaignInvitations_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals",
                column: "InvitationId1",
                principalSchema: "AdMarketplace",
                principalTable: "CampaignInvitations",
                principalColumn: "Id");
        }
    }
}
