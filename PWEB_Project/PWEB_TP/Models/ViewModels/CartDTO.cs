using PWEB_TP.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PWEB_TP.Models.ViewModels
{
    public class CartDTO
    {
        public List<CartItemDTO> ListProducts { get; set; }
        public decimal Price { get; set; }

        public CartDTO()
        {
            ListProducts = new List<CartItemDTO>();
            Price = 0.00M;
        }
    }
}