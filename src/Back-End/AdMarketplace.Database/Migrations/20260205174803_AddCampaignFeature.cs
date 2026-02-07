using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campaigns",
                schema: "AdMarketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvertiserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Brief = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    BudgetTon = table.Column<decimal>(type: "numeric(18,9)", precision: 18, scale: 9, nullable: false),
                    MaxPricePerPlacement = table.Column<decimal>(type: "numeric(18,9)", precision: 18, scale: 9, nullable: true),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ApplicationDeadline = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreativeJson = table.Column<string>(type: "jsonb", nullable: true),
                    TargetingJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Campaigns_Users_AdvertiserId",
                        column: x => x.AdvertiserId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CampaignApplications",
                schema: "AdMarketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProposedAdFormat = table.Column<byte>(type: "smallint", nullable: false),
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
                    table.PrimaryKey("PK_CampaignApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignApplications_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignApplications_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deals",
                schema: "AdMarketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
                    AdvertiserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AmountTon = table.Column<decimal>(type: "numeric(18,9)", precision: 18, scale: 9, nullable: false),
                    AdFormat = table.Column<byte>(type: "smallint", nullable: false),
                    EscrowWalletAddress = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    TransactionHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Status = table.Column<byte>(type: "smallint", nullable: false),
                    ScheduledPostTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActualPostTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PostedMessageId = table.Column<long>(type: "bigint", nullable: true),
                    DraftMessageId = table.Column<long>(type: "bigint", nullable: true),
                    DraftStatus = table.Column<byte>(type: "smallint", nullable: false),
                    AdvertiserFeedback = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    LastActivityAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AutoCancelAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FundsReleasedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deals_CampaignApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalSchema: "AdMarketplace",
                        principalTable: "CampaignApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Deals_Users_AdvertiserId",
                        column: x => x.AdvertiserId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CampaignApplications_CampaignId_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                columns: new[] { "CampaignId", "ChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignApplications_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignApplications",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_AdvertiserId",
                schema: "AdMarketplace",
                table: "Campaigns",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_CategoryId",
                schema: "AdMarketplace",
                table: "Campaigns",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_AdvertiserId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "AdvertiserId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_ApplicationId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_AutoCancelAt",
                schema: "AdMarketplace",
                table: "Deals",
                column: "AutoCancelAt");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_CampaignId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_ChannelId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_Status",
                schema: "AdMarketplace",
                table: "Deals",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Deals",
                schema: "AdMarketplace");

            migrationBuilder.DropTable(
                name: "CampaignApplications",
                schema: "AdMarketplace");

            migrationBuilder.DropTable(
                name: "Campaigns",
                schema: "AdMarketplace");
        }
    }
}
