using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace PWEB_TP.Models.ViewModels
{
    public class ProductDTO
    {
        public SelectList CategoryDropDown { get; set; }

        [Required]
        public string Name { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:c}")]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        public int Category { get; set; }
    }
}