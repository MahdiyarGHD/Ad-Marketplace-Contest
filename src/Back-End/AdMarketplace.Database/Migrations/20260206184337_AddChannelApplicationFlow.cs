using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelApplicationFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CampaignId",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ChannelApplicationId",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChannelApplications",
                schema: "AdMarketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvertiserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposedAdFormat = table.Column<byte>(type: "smallint", nullable: false),
                    ProposedPriceType = table.Column<byte>(type: "smallint", nullable: false),
                    ProposedPriceTon = table.Column<decimal>(type: "numeric(18,9)", precision: 18, scale: 9, nullable: false),
                    ProposedPostingTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Message = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    RejectionReason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelApplications_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelApplications_Users_AdvertiserId",
                        column: x => x.AdvertiserId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_ChannelApplicationId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "ChannelApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChannelApplications_AdvertiserId",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelApplications_ChannelId_AdvertiserId",
                schema: "AdMarketplace",
                table: "ChannelApplications",
                columns: new[] { "ChannelId", "AdvertiserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_ChannelApplications_ChannelApplicationId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "ChannelApplicationId",
                principalSchema: "AdMarketplace",
                principalTable: "ChannelApplications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_ChannelApplications_ChannelApplicationId",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropTable(
                name: "ChannelApplications",
                schema: "AdMarketplace");

            migrationBuilder.DropIndex(
                name: "IX_Deals_ChannelApplicationId",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ChannelApplicationId",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.AlterColumn<Guid>(
                name: "CampaignId",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
