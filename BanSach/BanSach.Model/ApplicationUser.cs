﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanSach.Model
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string? Name { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public string? State {  get; set; }
        public string? PostalCode { get; set; }

        [ForeignKey("CompanyId")]
        public int? CompanyId { get; set; }
        public Company? company { get; set; }
    }
}
