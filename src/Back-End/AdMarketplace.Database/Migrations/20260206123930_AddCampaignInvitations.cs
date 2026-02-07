using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddCampaignInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationId",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "InvitationId",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InvitationId1",
                schema: "AdMarketplace",
                table: "Deals",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CampaignInvitations",
                schema: "AdMarketplace",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_CampaignInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CampaignInvitations_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CampaignInvitations_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalSchema: "AdMarketplace",
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_InvitationId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "InvitationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deals_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals",
                column: "InvitationId1");

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitations_CampaignId_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignInvitations",
                columns: new[] { "CampaignId", "ChannelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CampaignInvitations_ChannelId",
                schema: "AdMarketplace",
                table: "CampaignInvitations",
                column: "ChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_CampaignInvitations_InvitationId",
                schema: "AdMarketplace",
                table: "Deals",
                column: "InvitationId",
                principalSchema: "AdMarketplace",
                principalTable: "CampaignInvitations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deals_CampaignInvitations_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals",
                column: "InvitationId1",
                principalSchema: "AdMarketplace",
                principalTable: "CampaignInvitations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deals_CampaignInvitations_InvitationId",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropForeignKey(
                name: "FK_Deals_CampaignInvitations_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropTable(
                name: "CampaignInvitations",
                schema: "AdMarketplace");

            migrationBuilder.DropIndex(
                name: "IX_Deals_InvitationId",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_InvitationId1",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "InvitationId",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "InvitationId1",
                schema: "AdMarketplace",
                table: "Deals");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicationId",
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
