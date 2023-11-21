using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JSE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    admin_username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    admin_password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.admin_id);
                });

            migrationBuilder.CreateTable(
                name: "Courier",
                columns: table => new
                {
                    courier_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    courier_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    courier_phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courier", x => x.courier_id);
                });

            migrationBuilder.CreateTable(
                name: "PoolBranch",
                columns: table => new
                {
                    pool_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    pool_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    pool_phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolBranch", x => x.pool_id);
                });

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    tracking_number = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    service_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sending_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    arrival_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    package_weight = table.Column<int>(type: "int", nullable: false),
                    delivery_price = table.Column<int>(type: "int", nullable: false),
                    delivery_status = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sender_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sender_phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    sender_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    receiver_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    receiver_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    receiver_phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    pool_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    courier_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Delivery", x => x.tracking_number);
                    table.ForeignKey(
                        name: "FK_Delivery_Courier_courier_id",
                        column: x => x.courier_id,
                        principalTable: "Courier",
                        principalColumn: "courier_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Delivery_PoolBranch_pool_id",
                        column: x => x.pool_id,
                        principalTable: "PoolBranch",
                        principalColumn: "pool_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_courier_id",
                table: "Delivery",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_pool_id",
                table: "Delivery",
                column: "pool_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Delivery");

            migrationBuilder.DropTable(
                name: "Courier");

            migrationBuilder.DropTable(
                name: "PoolBranch");
        }
    }
}
