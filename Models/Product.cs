using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecondHandPlatform.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between $0.01 and $1,000,000")]
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Image")]
        public string? ImagePath { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Foreign keys
        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}