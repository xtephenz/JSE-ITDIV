using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JSE.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    pool_city = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    pool_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    pool_phone = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoolBranch", x => x.pool_city);
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    admin_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    admin_username = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    pool_city = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    admin_password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK_Admin_PoolBranch_pool_city",
                        column: x => x.pool_city,
                        principalTable: "PoolBranch",
                        principalColumn: "pool_city",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Delivery",
                columns: table => new
                {
                    tracking_number = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    sender_city = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    receiver_city = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                        name: "FK_Delivery_PoolBranch_receiver_city",
                        column: x => x.receiver_city,
                        principalTable: "PoolBranch",
                        principalColumn: "pool_city",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Delivery_PoolBranch_sender_city",
                        column: x => x.sender_city,
                        principalTable: "PoolBranch",
                        principalColumn: "pool_city",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_pool_city",
                table: "Admin",
                column: "pool_city");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_courier_id",
                table: "Delivery",
                column: "courier_id");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_receiver_city",
                table: "Delivery",
                column: "receiver_city");

            migrationBuilder.CreateIndex(
                name: "IX_Delivery_sender_city",
                table: "Delivery",
                column: "sender_city");
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
