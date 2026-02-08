using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostedTextHash",
                schema: "AdMarketplace",
                table: "Deals",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostedTextHash",
                schema: "AdMarketplace",
                table: "Deals");
        }
    }
}
