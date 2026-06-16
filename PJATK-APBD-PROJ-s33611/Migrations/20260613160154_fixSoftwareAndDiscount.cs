using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJATK_APBD_PROJ_s33611.Migrations
{
    /// <inheritdoc />
    public partial class fixSoftwareAndDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_Software_SoftwareId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_SoftwareId",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountTarget",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "SoftwareId",
                table: "Discounts");

            migrationBuilder.AddColumn<string>(
                name: "Offer",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Offer",
                table: "Discounts");

            migrationBuilder.AddColumn<int>(
                name: "DiscountTarget",
                table: "Discounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoftwareId",
                table: "Discounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_SoftwareId",
                table: "Discounts",
                column: "SoftwareId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_Software_SoftwareId",
                table: "Discounts",
                column: "SoftwareId",
                principalTable: "Software",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
