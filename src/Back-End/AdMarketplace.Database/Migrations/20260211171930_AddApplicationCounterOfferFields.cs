using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationCounterOfferFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "CounterAdFormat",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CounterMessage",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CounterPostingTime",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CounterPriceTon",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                type: "numeric(18,9)",
                precision: 18,
                scale: 9,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CounterPriceType",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastCounterByUserId",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CounterAdFormat",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CounterMessage",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CounterPostingTime",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CounterPriceTon",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "numeric(18,9)",
                precision: 18,
                scale: 9,
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "CounterPriceType",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastCounterByUserId",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CounterAdFormat",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropColumn(
                name: "CounterMessage",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropColumn(
                name: "CounterPostingTime",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropColumn(
                name: "CounterPriceTon",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropColumn(
                name: "CounterPriceType",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropColumn(
                name: "LastCounterByUserId",
                schema: "AdMarketplace",
                table: "ChannelApplications");

            migrationBuilder.DropColumn(
                name: "CounterAdFormat",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.DropColumn(
                name: "CounterMessage",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.DropColumn(
                name: "CounterPostingTime",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.DropColumn(
                name: "CounterPriceTon",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.DropColumn(
                name: "CounterPriceType",
                schema: "AdMarketplace",
                table: "CampaignApplications");

            migrationBuilder.DropColumn(
                name: "LastCounterByUserId",
                schema: "AdMarketplace",
                table: "CampaignApplications");
        }
    }
}
