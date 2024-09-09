using Microsoft.EntityFrameworkCore;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Entities;

namespace SupplierOrdersModule.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<PurchaseReceipt> PurchaseReceipts { get; set; }
        public DbSet<PurchaseReceiptItem> PurchaseReceiptItems { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<ItemLedgerEntry> ItemLedgerEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //add Indexes
            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.Id)
                .HasDatabaseName("Index_SupplierId");

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Id)
                .HasDatabaseName("Index_ProductId");

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(po => po.Id)
                .HasDatabaseName("Index_PurchaseOrderId");

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasIndex(poi => poi.Id)
                .HasDatabaseName("Index_PurchaseOrderItemId");

            modelBuilder.Entity<PurchaseReceipt>()
                .HasIndex(pr => pr.Id)
                .HasDatabaseName("Index_PurchaseReceiptId");

            modelBuilder.Entity<ItemLedgerEntry>()
                .HasIndex(ile => ile.Id)
                .HasDatabaseName("Index_ItemLedgerEntryId");

            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Id)
                .HasDatabaseName("Index_StockId");

            // Configure decimal properties for precision and scale
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 4); // Precision: 18, Scale: 4

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(18, 4);

            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.UnitPrice)
                .HasPrecision(18, 4);

            modelBuilder.Entity<ItemLedgerEntry>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<ItemLedgerEntry>()
                .Property(x => x.TransactionDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
