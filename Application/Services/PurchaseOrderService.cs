using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Interfaces;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using SupplierOrdersModule.Infrastructure.Data;

namespace SupplierOrdersModule.Application.Services
{
    public class PurchaseOrderService
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IRepository<PurchaseOrderItem> _purchaseOrderItemRepository;
        private readonly SupplierService _supplierService;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ItemLedgerEntry> _itemLedgerEntry;
        private readonly ApplicationDbContext _context;

        public PurchaseOrderService(ApplicationDbContext context,
            IRepository<PurchaseOrder> purchaseOrderRepository,
            IRepository<PurchaseOrderItem> purchaseOrderItemRepository,
            IRepository<Product> productRepository,
            SupplierService supplierService,
            IRepository<ItemLedgerEntry> itemLedgerEntry)
        {
            _context = context;
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrderItemRepository = purchaseOrderItemRepository;
            _productRepository = productRepository; // Inject the product repository
            _supplierService = supplierService;
            _itemLedgerEntry = itemLedgerEntry;
        }

        public async Task<IEnumerable<PurchaseOrderDTO>> GetAllPurchaseOrdersAsync()
        {
            // Use the new method with eager loading
            var orders = await _purchaseOrderRepository.GetAllWithIncludesAsync(o => o.PurchaseOrderItems);

            return orders.Select(o => new PurchaseOrderDTO
            {
                Id = o.Id,
                SupplierId = o.SupplierId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Items = o.PurchaseOrderItems.Select(i => new PurchaseOrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });
        }



        // Added only to be used for validations for now
        public async Task<PurchaseOrderDTO> GetPurchaseOrderByIdAsync(int id)
        {
            //eagerly load the related PurchaseOrderItems
            var purchaseOrder = await _purchaseOrderRepository.GetByIdWithIncludesAsync(id, o => o.PurchaseOrderItems);

            if (purchaseOrder == null) return null;

            return new PurchaseOrderDTO
            {
                Id = purchaseOrder.Id,
                SupplierId = purchaseOrder.SupplierId,
                OrderDate = purchaseOrder.OrderDate,
                TotalAmount = purchaseOrder.TotalAmount,
                Status = purchaseOrder.Status,
                Items = purchaseOrder.PurchaseOrderItems.Select(i => new PurchaseOrderItemDTO
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }


        public async Task AddPurchaseOrderAsync(PurchaseOrderDTO purchaseOrderDto)
        {
            //will use a transaction cz user cannot create a purchase order if there is an error in the purchased items
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //check if the supplier exists
                var supplier = await _supplierService.GetSupplierByIdAsync(purchaseOrderDto.SupplierId);
                if (supplier == null)
                {
                    throw new Exception("Supplier does not exist.");
                }

                //validate all products before creating the order
                foreach (var item in purchaseOrderDto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new Exception($"Product with ID {item.ProductId} does not exist.");
                    }
                }

                //create the purchase order
                var order = new PurchaseOrder
                {
                    SupplierId = purchaseOrderDto.SupplierId,
                    OrderDate = purchaseOrderDto.OrderDate,
                    TotalAmount = purchaseOrderDto.TotalAmount,
                    Status = purchaseOrderDto.Status,
                    CreatedAt = DateTime.Now
                };

                await _purchaseOrderRepository.AddAsync(order);

                //This code was before adding the ledger logic 
                ////add order items now that the order is created
                //foreach (var item in purchaseOrderDto.Items)
                //{
                //    var orderItem = new PurchaseOrderItem
                //    {
                //        PurchaseOrderId = order.Id, 
                //        ProductId = item.ProductId,
                //        Quantity = item.Quantity,
                //        UnitPrice = item.UnitPrice
                //    };

                //    await _purchaseOrderItemRepository.AddAsync(orderItem);
                //}

                //add the order items to ledger table
                foreach (var item in purchaseOrderDto.Items)
                {
                    //add the order item
                    var orderItem = new PurchaseOrderItem
                    {
                        PurchaseOrderId = order.Id, 
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    await _purchaseOrderItemRepository.AddAsync(orderItem);

                    //add the item ledger entry to track stock movement
                    var ledgerEntry = new ItemLedgerEntry
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,  //positive because stock is being added
                        TransactionType = "Purchase Order", 
                        TransactionDate = DateTime.Now,
                        Remarks = $"Purchase Order #{order.Id} - Stock Added"
                    };

                    await _itemLedgerEntry.AddAsync(ledgerEntry);
                }

                //commit the transaction after all operations are successful
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _purchaseOrderRepository.GetByIdAsync(orderId);
            if (order == null) return;
            
            order.Status = status;
            await _purchaseOrderRepository.UpdateAsync(order);
        }
    }
}
