using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JSE.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Courier_courier_id",
                table: "Delivery");

            migrationBuilder.AlterColumn<Guid>(
                name: "courier_id",
                table: "Delivery",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Courier_courier_id",
                table: "Delivery",
                column: "courier_id",
                principalTable: "Courier",
                principalColumn: "courier_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Delivery_Courier_courier_id",
                table: "Delivery");

            migrationBuilder.AlterColumn<Guid>(
                name: "courier_id",
                table: "Delivery",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Delivery_Courier_courier_id",
                table: "Delivery",
                column: "courier_id",
                principalTable: "Courier",
                principalColumn: "courier_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
