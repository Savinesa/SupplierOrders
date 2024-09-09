using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupplierOrdersModule.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "Index_SupplierId",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_StockId",
                table: "Stocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_PurchaseReceiptId",
                table: "PurchaseReceipts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_PurchaseOrderId",
                table: "PurchaseOrders",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_PurchaseOrderItemId",
                table: "PurchaseOrderItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_ProductId",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "Index_ItemLedgerEntryId",
                table: "ItemLedgerEntries",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "Index_SupplierId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "Index_StockId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "Index_PurchaseReceiptId",
                table: "PurchaseReceipts");

            migrationBuilder.DropIndex(
                name: "Index_PurchaseOrderId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "Index_PurchaseOrderItemId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "Index_ProductId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "Index_ItemLedgerEntryId",
                table: "ItemLedgerEntries");
        }
    }
}
