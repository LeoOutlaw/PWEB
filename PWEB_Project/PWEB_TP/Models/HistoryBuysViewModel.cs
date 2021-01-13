using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PWEB_TP.Models
{
    public class HistoryBuysViewModel
    {

        public int IdCompra { get; set; }
        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Status of the buy")]
        public string State { get; set; }
        [Display(Name = "Date of Buy")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime BuyDate { get; set; }
        [Display(Name = "Date of Delivery")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime DeliveryDate { get; set; }
        [Display(Name = "Type of Delivery")]
        public string DeliveryType { get; set; }
    }
}