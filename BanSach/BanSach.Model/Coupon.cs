using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Model
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime DateStarted { get; set; } = DateTime.Now;
        public DateTime DateExpired { get; set; } = DateTime.Now.AddDays(5);
        [Required]
        public int Quantity { get; set; }
        public int Status { get; set; } 

    }



}
