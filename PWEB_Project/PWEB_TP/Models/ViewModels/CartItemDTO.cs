using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWEB_TP.Models.ViewModels
{
    public class CartItemDTO
    {
        public int ProdcutID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsInCart { get; set; }
    }
}