using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelPricings",
                schema: "AdMarketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdFormat = table.Column<byte>(type: "smallint", nullable: false),
                    PriceType = table.Column<byte>(type: "smallint", nullable: false),
                    PriceTon = table.Column<decimal>(type: "numeric(18,9)", precision: 18, scale: 9, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelPricings_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelPricings_ChannelId",
                schema: "AdMarketplace",
                table: "ChannelPricings",
                column: "ChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelPricings",
                schema: "AdMarketplace");
        }
    }
}
