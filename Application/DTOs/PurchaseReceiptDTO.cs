namespace SupplierOrdersModule.Application.DTOs
{
    public class PurchaseReceiptDTO
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public DateTime ReceiptDate { get; set; }
        public List<PurchaseReceiptItemDTO> Items { get; set; } 

    }
}
