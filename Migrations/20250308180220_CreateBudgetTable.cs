using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetBackend.Migrations
{
    /// <inheritdoc />
    public partial class CreateBudgetTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "BudgetRecords");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "BudgetRecords",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Cost",
                table: "BudgetRecords",
                newName: "Utilities");

            migrationBuilder.AddColumn<decimal>(
                name: "DebtBalance",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DebtToIncomeRatio",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Entertainment",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Groceries",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Insurance",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InvestmentBalance",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Investments",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LoanPayments",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NetWorth",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RentMortgage",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SavingsBalance",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SavingsRate",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Subscriptions",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalExpenses",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalIncome",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Transportation",
                table: "BudgetRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtBalance",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "DebtToIncomeRatio",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Entertainment",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Groceries",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Insurance",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "InvestmentBalance",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Investments",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "LoanPayments",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "NetWorth",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "RentMortgage",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "SavingsBalance",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "SavingsRate",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Subscriptions",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "TotalExpenses",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "TotalIncome",
                table: "BudgetRecords");

            migrationBuilder.DropColumn(
                name: "Transportation",
                table: "BudgetRecords");

            migrationBuilder.RenameColumn(
                name: "Utilities",
                table: "BudgetRecords",
                newName: "Cost");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BudgetRecords",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "BudgetRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
