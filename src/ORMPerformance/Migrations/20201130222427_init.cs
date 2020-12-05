using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ORMPerformance.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: true, computedColumnSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(nullable: true, computedColumnSql: "getutcdate()"),
                    TableName = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true),
                    CreateUserId = table.Column<int>(nullable: false),
                    UpdateUserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    ContactPhone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: true, computedColumnSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(nullable: true, computedColumnSql: "getutcdate()"),
                    AuditLogId = table.Column<int>(nullable: false),
                    ValueFrom = table.Column<string>(nullable: true),
                    ValueTo = table.Column<string>(nullable: true),
                    FieldName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogDetail_AuditLog_AuditLogId",
                        column: x => x.AuditLogId,
                        principalTable: "AuditLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    CustomerId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    CCV = table.Column<int>(nullable: false),
                    Exp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Card_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    UpdateDate = table.Column<DateTime>(nullable: true, defaultValueSql: "getutcdate()"),
                    DeliveryDetailId = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    Currency = table.Column<int>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_OrderStatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalTable: "OrderStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_Action",
                table: "AuditLog",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_TableName",
                table: "AuditLog",
                column: "TableName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogDetail_AuditLogId",
                table: "AuditLogDetail",
                column: "AuditLogId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogDetail_FieldName",
                table: "AuditLogDetail",
                column: "FieldName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogDetail_ValueFrom",
                table: "AuditLogDetail",
                column: "ValueFrom");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogDetail_ValueTo",
                table: "AuditLogDetail",
                column: "ValueTo");

            migrationBuilder.CreateIndex(
                name: "IX_Card_CustomerId",
                table: "Card",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ContactName",
                table: "Customer",
                column: "ContactName");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_ContactPhone",
                table: "Customer",
                column: "ContactPhone");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Email",
                table: "Customer",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customer",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Currency",
                table: "Order",
                column: "Currency");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CustomerId",
                table: "Order",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusId",
                table: "Order",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Price",
                table: "Order",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_Name",
                table: "OrderStatus",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogDetail");

            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "OrderStatus");
        }
    }
}
