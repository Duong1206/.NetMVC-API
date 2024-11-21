using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Model
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        [ForeignKey("ApplicationUserId")]
        public string? ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        [MaxLength(300, ErrorMessage = "Comment cannot exceed 300 characters.")]
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
