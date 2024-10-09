using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Model
{
    public class Category
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        // tuỳ chỉnh hiển thị dl
        [Display(Name = "Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100")]
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
