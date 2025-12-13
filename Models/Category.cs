using System.ComponentModel.DataAnnotations;

namespace SecondHandPlatform.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}