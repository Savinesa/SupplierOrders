using Microsoft.AspNetCore.Mvc;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Application.Services;
using System.Threading.Tasks;

namespace SupplierOrdersModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseReceiptController : ControllerBase
    {
        private readonly PurchaseReceiptService _purchaseReceiptService;

        public PurchaseReceiptController(PurchaseReceiptService purchaseReceiptService)
        {
            _purchaseReceiptService = purchaseReceiptService;
        }

        // POST: api/purchasereceipt
        [HttpPost]
        public async Task<IActionResult> AddPurchaseReceipt(PurchaseReceiptDTO receiptDto)
        {
            // Validate the model
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _purchaseReceiptService.AddPurchaseReceiptAsync(receiptDto);
                return CreatedAtAction(nameof(AddPurchaseReceipt), null);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the purchase receipt: {ex.Message}");
            }
        }

        // GET: api/purchasereceipt/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceiptById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid receipt ID.");

            try
            {
                var receipt = await _purchaseReceiptService.GetReceiptByIdAsync(id);

                if (receipt == null)
                    return NotFound($"Receipt with ID {id} not found.");

                return Ok(receipt);
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error for unexpected exceptions
                return StatusCode(500, $"An error occurred while retrieving the purchase receipt: {ex.Message}");
            }
        }
    }
}
