using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJATK_APBD_PROJ_s33611.Migrations
{
    /// <inheritdoc />
    public partial class addPaymentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ContractPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContractPayments");
        }
    }
}
