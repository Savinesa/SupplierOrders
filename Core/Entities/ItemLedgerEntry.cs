namespace SupplierOrdersModule.Core.Entities
{
    public class ItemLedgerEntry
    {
        public int Id { get; set; }  // Primary Key

        public int ProductId { get; set; }  // Foreign key to Product
        public Product Product { get; set; }  

        public int Quantity { get; set; }  // Positive for stock additions, negative for stock deductions

        public string TransactionType { get; set; }  // "Purchase", "Receipt"

        public DateTime TransactionDate { get; set; }  // Date of the transaction

        public string Remarks { get; set; }  // Optional: Additional information or reference IDs (e.g., "Purchase Order #123")
    }

}
