using System.ComponentModel.DataAnnotations;

namespace SupplierOrdersModule.Application.DTOs
{
    public class SupplierDTO
    {
        public int Id { get; set; } //Should be Removed because user shouldnt be able to add or edit id, in case user wanted to get id we can create 2 seperate dtos for supplier

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        public string PhoneNumber { get; set; }
    }

}

