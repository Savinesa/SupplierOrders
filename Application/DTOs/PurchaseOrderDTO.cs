namespace SupplierOrdersModule.Application.DTOs
{
    public class PurchaseOrderDTO
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<PurchaseOrderItemDTO> Items { get; set; } 

    }
}
