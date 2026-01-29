using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserChannelConnectionRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChannelLinks_Users_UserId",
                schema: "AdMarketplace",
                table: "UserChannelLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChannelLinks",
                schema: "AdMarketplace",
                table: "UserChannelLinks");

            migrationBuilder.RenameTable(
                name: "UserChannelLinks",
                schema: "AdMarketplace",
                newName: "UserChannelConnections",
                newSchema: "AdMarketplace");

            migrationBuilder.RenameIndex(
                name: "IX_UserChannelLinks_UserId_ChatId",
                schema: "AdMarketplace",
                table: "UserChannelConnections",
                newName: "IX_UserChannelConnections_UserId_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChannelConnections",
                schema: "AdMarketplace",
                table: "UserChannelConnections",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChannelConnections_Users_UserId",
                schema: "AdMarketplace",
                table: "UserChannelConnections",
                column: "UserId",
                principalSchema: "AdMarketplace",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChannelConnections_Users_UserId",
                schema: "AdMarketplace",
                table: "UserChannelConnections");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserChannelConnections",
                schema: "AdMarketplace",
                table: "UserChannelConnections");

            migrationBuilder.RenameTable(
                name: "UserChannelConnections",
                schema: "AdMarketplace",
                newName: "UserChannelLinks",
                newSchema: "AdMarketplace");

            migrationBuilder.RenameIndex(
                name: "IX_UserChannelConnections_UserId_ChatId",
                schema: "AdMarketplace",
                table: "UserChannelLinks",
                newName: "IX_UserChannelLinks_UserId_ChatId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserChannelLinks",
                schema: "AdMarketplace",
                table: "UserChannelLinks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChannelLinks_Users_UserId",
                schema: "AdMarketplace",
                table: "UserChannelLinks",
                column: "UserId",
                principalSchema: "AdMarketplace",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
