using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdMarketplace.Database.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBalanceDefaultValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                schema: "AdMarketplace",
                table: "Users",
                type: "numeric(32,8)",
                precision: 32,
                scale: 8,
                nullable: false,
                defaultValue: 100m,
                oldClrType: typeof(decimal),
                oldType: "numeric(32,8)",
                oldPrecision: 32,
                oldScale: 8,
                oldDefaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                schema: "AdMarketplace",
                table: "Users",
                type: "numeric(32,8)",
                precision: 32,
                scale: 8,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(32,8)",
                oldPrecision: 32,
                oldScale: 8,
                oldDefaultValue: 100m);
        }
    }
}
