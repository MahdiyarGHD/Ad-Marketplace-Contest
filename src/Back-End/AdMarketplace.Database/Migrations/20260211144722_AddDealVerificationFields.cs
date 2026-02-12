using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDealVerificationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChannelUnitPrice",
                schema: "AdMarketplace",
                table: "Deals",
                type: "numeric(18,9)",
                precision: 18,
                scale: 9,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PostVerifyAt",
                schema: "AdMarketplace",
                table: "Deals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RequiredPostDurationHours",
                schema: "AdMarketplace",
                table: "Deals",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequiredViewCount",
                schema: "AdMarketplace",
                table: "Deals",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelUnitPrice",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "PostVerifyAt",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "RequiredPostDurationHours",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "RequiredViewCount",
                schema: "AdMarketplace",
                table: "Deals");
        }
    }
}
