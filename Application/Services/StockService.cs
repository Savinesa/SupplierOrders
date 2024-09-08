using System.Threading.Tasks;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Interfaces;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Core.Entities;
using SupplierOrdersModule.Core.Interfaces;

namespace SupplierOrdersModule.Application.Services
{
    public class StockService
    {
        private readonly IRepository<Stock> _stockRepository;

        public StockService(IRepository<Stock> stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<StockDTO> GetStockByProductIdAsync(int productId)
        {
            var stock = await _stockRepository.GetByIdAsync(productId);
            if (stock == null) return null;

            return new StockDTO
            {
                ProductId = stock.ProductId,
                Quantity = stock.Quantity
            };
        }
    }
}
