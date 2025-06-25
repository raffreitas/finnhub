using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinnHub.PortfolioManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class v0102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_positions_portfolios_portfolio_id",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_portfolios_portfolio_id",
                table: "transactions");

            migrationBuilder.AddForeignKey(
                name: "fk_positions_portfolios_portfolio_id",
                table: "positions",
                column: "portfolio_id",
                principalTable: "portfolios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_portfolios_portfolio_id",
                table: "transactions",
                column: "portfolio_id",
                principalTable: "portfolios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_positions_portfolios_portfolio_id",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "fk_transactions_portfolios_portfolio_id",
                table: "transactions");

            migrationBuilder.AddForeignKey(
                name: "fk_positions_portfolios_portfolio_id",
                table: "positions",
                column: "portfolio_id",
                principalTable: "portfolios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_portfolios_portfolio_id",
                table: "transactions",
                column: "portfolio_id",
                principalTable: "portfolios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
