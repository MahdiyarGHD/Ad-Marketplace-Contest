using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentIdToChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AgentId",
                schema: "AdMarketplace",
                table: "Channels",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_AgentId",
                schema: "AdMarketplace",
                table: "Channels",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Agents_AgentId",
                schema: "AdMarketplace",
                table: "Channels",
                column: "AgentId",
                principalSchema: "AdMarketplace",
                principalTable: "Agents",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Agents_AgentId",
                schema: "AdMarketplace",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_AgentId",
                schema: "AdMarketplace",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "AgentId",
                schema: "AdMarketplace",
                table: "Channels");
        }
    }
}
