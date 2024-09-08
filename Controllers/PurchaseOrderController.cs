using Microsoft.AspNetCore.Mvc;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Application.Services;
using System.Threading.Tasks;

namespace SupplierOrdersModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly PurchaseOrderService _purchaseOrderService;

        public PurchaseOrderController(PurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        // GET: api/purchaseorder
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrders()
        {
            try
            {
                var orders = await _purchaseOrderService.GetAllPurchaseOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error for unexpected exceptions
                return StatusCode(500, $"An error occurred while retrieving purchase orders: {ex.Message}");
            }
        }

        // POST: api/purchaseorder
        [HttpPost]
        public async Task<IActionResult> AddPurchaseOrder(PurchaseOrderDTO orderDto)
        {
            // Check if the provided model is valid
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _purchaseOrderService.AddPurchaseOrderAsync(orderDto);
                return CreatedAtAction(nameof(GetPurchaseOrders), null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the purchase order: {ex.Message}");
            }
        }

        // PUT: api/purchaseorder/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            //validate the status string and ensure it's non-empty
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Status cannot be empty.");

            if (id <= 0)
                return BadRequest("Invalid purchase order ID.");

            var order = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Purchase Oder with ID {id} not found!");

            try
            {
                await _purchaseOrderService.UpdateOrderStatusAsync(id, status);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the purchase order status: {ex.Message}");
            }
        }
    }
}
