using System.Collections.Generic;
using System.Threading.Tasks;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Interfaces;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Interfaces;

namespace SupplierOrdersModule.Application.Services
{
    public class PurchaseReceiptService
    {
        private readonly IRepository<ItemLedgerEntry> _itemLedgerEntryRepository;
        private readonly IRepository<PurchaseReceipt> _purchaseReceiptRepository;
        private readonly IRepository<PurchaseReceiptItem> _purchaseReceiptItemRepository;
        private readonly IRepository<Stock> _stockRepository; 
        private readonly PurchaseOrderService _purchaseOrderService;


        public PurchaseReceiptService(
            IRepository<PurchaseReceipt> purchaseReceiptRepository,
            IRepository<PurchaseReceiptItem> purchaseReceiptItemRepository,
            IRepository<Stock> stockRepository,
            PurchaseOrderService purchaseOrderService,
            IRepository<ItemLedgerEntry> itemLedgerEntryRepository)
        {
            _purchaseReceiptRepository = purchaseReceiptRepository;
            _purchaseReceiptItemRepository = purchaseReceiptItemRepository;
            _stockRepository = stockRepository;
            _purchaseOrderService = purchaseOrderService;
            _itemLedgerEntryRepository = itemLedgerEntryRepository;
        }

        public async Task AddPurchaseReceiptAsync(PurchaseReceiptDTO receiptDto)
        {
            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(receiptDto.PurchaseOrderId);
            if (purchaseOrder == null)
            {
                throw new Exception("Purchase Order does not exist.");
            }

            if (purchaseOrder.Status != "Completed")
            {
                throw new Exception("Purchase Order is still not completed!");
            }

            // Check if received quantities exceed ordered quantities
            foreach (var receiptItem in receiptDto.Items)
            {
                var orderedItem = purchaseOrder.Items.FirstOrDefault(i => i.ProductId == receiptItem.ProductId);

                if (orderedItem == null)
                {
                    throw new Exception($"Product with ID {receiptItem.ProductId} is not part of the purchase order.");
                }

                if (receiptItem.ReceivedQuantity > orderedItem.Quantity)
                {
                    throw new Exception($"Received quantity for product with ID {receiptItem.ProductId} exceeds the ordered quantity.");
                }
            }

            var receipt = new PurchaseReceipt
            {
                PurchaseOrderId = receiptDto.PurchaseOrderId,
                ReceiptDate = receiptDto.ReceiptDate
            };

            await _purchaseReceiptRepository.AddAsync(receipt);

            foreach (var item in receiptDto.Items)
            {
                var receiptItem = new PurchaseReceiptItem
                {
                    PurchaseReceiptId = receipt.Id,
                    ProductId = item.ProductId,
                    ReceivedQuantity = item.ReceivedQuantity
                };
                await _purchaseReceiptItemRepository.AddAsync(receiptItem);

                //update stock
                var stock = await _stockRepository.GetByIdAsync(item.ProductId);
                if (stock != null)
                {
                    stock.Quantity += item.ReceivedQuantity;
                    await _stockRepository.UpdateAsync(stock);
                }
                else
                {
                    await _stockRepository.AddAsync(new Stock
                    {
                        ProductId = item.ProductId,
                        Quantity = item.ReceivedQuantity
                    });
                }

                //add the item ledger entry to track stock movement
                var ledgerEntry = new ItemLedgerEntry
                {
                    ProductId = item.ProductId,
                    Quantity = item.ReceivedQuantity,
                    TransactionType = "Purchase Receipt",
                    TransactionDate = DateTime.Now,
                    Remarks = $"Purchase Receipt for Order #{receiptDto.PurchaseOrderId} - Stock Added"
                };

                await _itemLedgerEntryRepository.AddAsync(ledgerEntry);
            }
        }

        public async Task<PurchaseReceiptDTO> GetReceiptByIdAsync(int id)
        {
            var receipt = await _purchaseReceiptRepository.GetByIdAsync(id);
            if (receipt == null) return null;

            var receiptItems = await _purchaseReceiptItemRepository.GetAllAsync();
            var items = receiptItems
                .Where(ri => ri.PurchaseReceiptId == receipt.Id)
                .Select(ri => new PurchaseReceiptItemDTO
                {
                    ProductId = ri.ProductId,
                    ReceivedQuantity = ri.ReceivedQuantity
                }).ToList();

            return new PurchaseReceiptDTO
            {
                Id = receipt.Id,
                PurchaseOrderId = receipt.PurchaseOrderId,
                ReceiptDate = receipt.ReceiptDate,
                Items = items
            };
        }
    }
}
