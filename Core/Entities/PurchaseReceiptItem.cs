namespace SupplierOrdersModule.Core.Entities
{
    public class PurchaseReceiptItem
    {
            public int Id { get; set; }
            public int PurchaseReceiptId { get; set; }
            public PurchaseReceipt PurchaseReceipt { get; set; }  
            public int ProductId { get; set; }
            public Product Product { get; set; }  
            public int ReceivedQuantity { get; set; }  // Quantity received

    }
}
