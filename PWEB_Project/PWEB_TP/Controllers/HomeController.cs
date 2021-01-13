using PWEB_TP.Models;
using PWEB_TP.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PWEB_TP.Controllers
{
    public class HomeController : Controller
    {
        private Pweb_DBEntities db = new Pweb_DBEntities();
        private CartDTO cart = new CartDTO();
        public ActionResult Index()
        {
            return View();
        }

        //GET
        public ActionResult ClientHome()
        {
            var products = db.Product.ToList();
            return View(products);
        }

        [HttpPost]
        public JsonResult AddToCart(string ItemId)
        {
            var cartItem = new CartItemDTO();
            var product = db.Product.Single(x => x.IdProduct.ToString() == ItemId);
            if (Session["cartItems"] != null)
            {
                cart = Session["cartItems"] as CartDTO;
            }
            if (cart.ListProducts.Any(m => m.ProdcutID.ToString() == ItemId))
            {
                cartItem = cart.ListProducts.Single(x => x.ProdcutID.ToString() == ItemId);
                cartItem.Quantity = cartItem.Quantity + 1;
                cartItem.TotalPrice = cartItem.Price * cartItem.Quantity;
                cart.Price += cartItem.Price;
            }
            else
            {
                cartItem.ProdcutID = product.IdProduct;
                cartItem.Name = product.Name;
                cartItem.Price = product.Price;
                cartItem.Quantity = 1;
                cartItem.TotalPrice = cartItem.Price;
                cartItem.IsInCart = true;
                cart.ListProducts.Add(cartItem);
                cart.Price += cartItem.Price;
            }
            
            Session["cartCounter"] = cart.ListProducts.Count;
            Session["cartItems"] = cart;

            return Json(new { Success = true, Counter = cart.ListProducts.Count }, JsonRequestBehavior.AllowGet);
        }

        //Get
        public ActionResult ShoppingCart()
        {
            cart = Session["cartItems"] as CartDTO;

            return View(cart);
        }

        [HttpPost]
        public JsonResult RemoveFromCart(string ItemId)
        {
            var cartItem = new CartItemDTO();
            if (Session["cartItems"] != null)
            {
                cart = Session["cartItems"] as CartDTO;
                cartItem = cart.ListProducts.Single(x => x.ProdcutID.ToString() == ItemId);
                cart.ListProducts.Remove(cartItem);
                cart.Price -= cartItem.Price*cartItem.Quantity;

                Session["cartCounter"] = cart.ListProducts.Count;
                Session["cartItems"] = cart;
                return Json(new { Success = true, Counter = cart.ListProducts.Count }, JsonRequestBehavior.AllowGet);
            }
            
            return Json(new { Success = false, Counter = 0 }, JsonRequestBehavior.AllowGet);
        }

        // GET: Categories/SearchCategory/5
        public ActionResult SearchCategory(int? id)
        {
            var products = db.Product.Where(x => x.Category_Id == id).ToList();
            return View(products);
        }

        public ActionResult Menu()
        {
            IEnumerable<Category> Menu = null;

            if (Session["_Menu"] != null)
            {
                Menu = (IEnumerable<Category>)Session["_Menu"];
            }
            else
            {
                Menu = db.Category.ToList();
                Session["_Menu"] = Menu;
            }
            return PartialView("_Menu", Menu);
        }
    }
}