namespace SupplierOrdersModule.Core.Entities
{
    public class PurchaseReceipt
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
        public DateTime ReceiptDate { get; set; }
        public ICollection<PurchaseReceiptItem> PurchaseReceiptItems { get; set; }

    }
}
