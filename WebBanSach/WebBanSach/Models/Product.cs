using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1,10000000)]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 10000000)]
        public double Price100 { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("Category")]
        [Required]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [ForeignKey("CoverType")]
        [Required]
        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType coverType { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }

    }
}
