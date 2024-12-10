using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Model
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int ISBN { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        [Range(1, 10000)]
        public int Quantity { get; set; }
        [Required]
        [Range(1, 100000000)]
        public double Price50 { get; set; }

        [Required]
        [Range(0, 100000000)]
        public double Price100 { get; set; }
        [ValidateNever]
        [Required]
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ValidateNever]

        [ForeignKey("CategoryId")]

        public Category? Category { get; set; }

        [Required]
        public int CoverTypeId { get; set; }

        [ForeignKey("CoverTypeId")]
        [ValidateNever]
        public CoverType? coverType { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}