using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PWEB_TP.Models.ViewModels
{
    public class PromotionDTO
    {
        public SelectList PromoDropDown { get; set; }

        public Product product { get; set; }
        [Required]
        public int Promotion { get; set; }
        
    }
}