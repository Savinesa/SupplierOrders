using Microsoft.AspNetCore.Mvc;
using SupplierOrdersModule.Application.DTOs;
using SupplierOrdersModule.Application.Services;
using System.Threading.Tasks;

namespace SupplierOrdersModule.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierService _supplierService;

        public SupplierController(SupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                //Return 500 internal server error if something unexpected happens
                return StatusCode(500, $"An error occurred while retrieving suppliers: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            // Check if id is not 0
            if (id <= 0)
                return BadRequest("Invalid supplier ID.");

            try
            {
                var supplier = await _supplierService.GetSupplierByIdAsync(id);
                if (supplier == null) // service returns null if supplierid wasn't found
                    return NotFound($"Supplier with ID {id} not found!");

                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving the supplier: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSupplier(SupplierDTO supplierDto)
        {
            //check if model is valid - mainly for data types errors
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                //check if supplierdto.email is an email
                await _supplierService.AddSupplierAsync(supplierDto);
                return CreatedAtAction(nameof(GetSupplierById), new { id = supplierDto.Id }, supplierDto); //this line returns the ceated supplier in the response body
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while adding the supplier: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, SupplierDTO supplierDto)
        {
            if (id <= 0)
                return BadRequest("Invalid supplier ID.");

            //check if id exists
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null) 
                return NotFound($"Supplier with ID {id} not found!");

            supplierDto.Id = id;

            try
            {
                await _supplierService.UpdateSupplierAsync(id, supplierDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while updating the supplier: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid supplier ID.");

            //check if id exists
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound($"Supplier with ID {id} not found!");

            try
            {
                await _supplierService.DeleteSupplierAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the supplier: {ex.Message}");
            }
        }
    }
}
