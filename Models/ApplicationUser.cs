using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SecondHandPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Display(Name = "Phone Number")]
        public override string? PhoneNumber { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; }
    }
}