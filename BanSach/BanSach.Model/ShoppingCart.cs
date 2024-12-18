﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Model
{
    public class ShoppingCart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product? Product { get; set; }
        [Range(1,1000)]
        public int Count { get; set; }
        [ForeignKey("ApplicationUserId")]
        public string? ApplicationUserId { get; set; }
        [ValidateNever]
        public ApplicationUser? ApplicationUser { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}
