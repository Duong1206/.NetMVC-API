﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BanSach.Model.ViewModel
{
    public class ProductVM
    {
       public  Product? product { get; set; }
        // lấy ra danhs sách Category, coverType dự vào Id trong product
        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem>? CoverTypeList { get; set; }

        

    }
}
