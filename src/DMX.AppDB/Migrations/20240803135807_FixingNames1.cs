using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMX.AppDB.Migrations
{
    /// <inheritdoc />
    public partial class FixingNames1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PoxY",
                table: "shape",
                newName: "ZOrder");

            migrationBuilder.RenameColumn(
                name: "PosZ",
                table: "shape",
                newName: "PosY");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZOrder",
                table: "shape",
                newName: "PoxY");

            migrationBuilder.RenameColumn(
                name: "PosY",
                table: "shape",
                newName: "PosZ");
        }
    }
}
