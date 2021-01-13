using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using PWEB_TP.Models;
using PWEB_TP.Models.ViewModels;

namespace PWEB_TP.Controllers
{
    [Authorize(Roles = "Client, Admin, Employee")]
    public class BuysController : Controller
    {
        private Pweb_DBEntities db = new Pweb_DBEntities();

        public Clients GetClient()
        {
            //vai buscar o ID do utilizador atual
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userIdValue = userIdClaim.Value;
            var client = db.Clients.Where(m => m.IdentityId == userIdValue).FirstOrDefault();

            return client;
        }

        public ActionResult ShoppingCart()
        {
            return RedirectToAction("ShoppingCart", "Home");
        }

        // GET: Buys
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var buys = db.Buy.OrderByDescending(x => x.BuyDate).ToList();
                return View(buys);
            }
            else if (User.IsInRole("Client"))
            {
                var client = GetClient();
                var buys = db.Buy.Where(x => x.Client_Id == client.IdClient).OrderByDescending(x => x.BuyDate).ToList();
                return View(buys);
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("EmployeeBuys");
            }
            else
            {
                return View();
            }
        }

        public ActionResult EmployeeBuys()
        {
            var buys = db.Buy.Where(x => x.State == "Pending").OrderByDescending(x => x.BuyDate).ToList();
            return View(buys);
        }

        public ActionResult Delivered(int? id)
        {
            var buys = db.Buy.Find(id);
            if (ModelState.IsValid)
            {
                buys.State = "Delivered";
                db.Entry(buys).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("EmployeeBuys");
            }
            return RedirectToAction("EmployeeBuys");
        }

        // GET: Buys/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Buy buy = db.Buy.Find(id);
            if (buy == null)
            {
                return HttpNotFound();
            }
            return View(buy);
        }

        // GET: Buys/Create
        public ActionResult Create()
        {
            var compra = new Buy();
            CartDTO cart = Session["cartItems"] as CartDTO;
            foreach (var item in cart.ListProducts)
            {
                compra.Product.Add(db.Product.Find(item.ProdcutID));
            }
            compra.Price = cart.Price;
            return View(compra);
        }

        // POST: Buys/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Buy buy)   //Fazer uma compra
        {
            var client = GetClient();
            CartDTO cart = Session["cartItems"] as CartDTO;
            List<Product> compras = new List<Product>();
            foreach (var item in cart.ListProducts)
            {
                compras.Add(db.Product.Find(item.ProdcutID));
            }
            if (ModelState.IsValid)
            {
                Buy compra = new Buy()
                {
                    Price = cart.Price,
                    State = "Pending",
                    BuyDate = DateTime.Now,
                    DeliveryType = buy.DeliveryType,
                    Product = compras,
                    Client_Id = client.IdClient,
                    Clients = client
                };
                // Caso o stock esteja a zero nao permitir a compra
                db.Buy.Add(compra);
                db.SaveChanges();
                foreach (var item in compra.Product)
                {
                    DecreseStock(item.IdProduct);
                }
                return RedirectToAction("ClientHome","Home");
            }

            return View(buy);
        }

        public void DecreseStock(int id)
        {
            var product = db.Product.Find(id);
            product.Stock--;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: Buys/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Buy buy = db.Buy.Find(id);
            if (buy == null)
            {
                return HttpNotFound();
            }
            return View(buy);
        }

        // POST: Buys/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdCompra,Price,State")] Buy buy)
        {
            if (ModelState.IsValid)
            {
                db.Entry(buy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(buy);
        }

        // GET: Buys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Buy buy = db.Buy.Find(id);
            if (buy == null)
            {
                return HttpNotFound();
            }
            return View(buy);
        }

        // POST: Buys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Buy buy = db.Buy.Find(id);
            db.Buy.Remove(buy);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
