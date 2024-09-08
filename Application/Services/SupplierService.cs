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
    public class SupplierService
    {
        private readonly IRepository<Supplier> _supplierRepository;

        public SupplierService(IRepository<Supplier> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<SupplierDTO>> GetAllSuppliersAsync()
        {
            // Here I used GetAllAsync() from the interface
            var suppliers = await _supplierRepository.GetAllAsync();
            return suppliers.Select(s => new SupplierDTO
            {
                Id = s.Id,
                Name = s.Name,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber
            });
        }

        public async Task<SupplierDTO> GetSupplierByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return null; // this will be handled in api to display correct error

            return new SupplierDTO
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber
            };
        }

        public async Task AddSupplierAsync(SupplierDTO supplierDto)
        {
            // Here am checking if a supplier with the same email or phone number already exists
            var existingSupplier = await _supplierRepository
                .GetWhereAsync(s => s.Email == supplierDto.Email || s.PhoneNumber == supplierDto.PhoneNumber);

            if (existingSupplier.Any())
            {
                throw new Exception("A supplier with the same email or phone number already exists.");
            }

            // Add logic if there is no duplicate
            var supplier = new Supplier
            {
                Name = supplierDto.Name,
                Email = supplierDto.Email,
                PhoneNumber = supplierDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            await _supplierRepository.AddAsync(supplier);
        }



        public async Task UpdateSupplierAsync(int id, SupplierDTO supplierDto)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return; // this will be handled in api to display correct error

            supplier.Name = supplierDto.Name;
            supplier.Email = supplierDto.Email;
            supplier.PhoneNumber = supplierDto.PhoneNumber;
            await _supplierRepository.UpdateAsync(supplier);
            
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return; // this will be handled in api to display correct error
            
            await _supplierRepository.DeleteAsync(supplier);
        }
    }
}
