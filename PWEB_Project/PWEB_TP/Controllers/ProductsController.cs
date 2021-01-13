using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using PWEB_TP.Models;
using PWEB_TP.Models.ViewModels;

namespace PWEB_TP.Controllers
{
    [Authorize(Roles = "Company, Employee, Admin, Client")]
    public class ProductsController : Controller
    {
        private Pweb_DBEntities db = new Pweb_DBEntities();

        // GET: Products
        public ActionResult Index()
        {
            if (User.IsInRole("Company"))
            {
                var company = GetCompany().IdCompany;
                var products = db.Product.Where(x => x.Company.IdCompany == company);
                return View(products);
            }
            return View(db.Product.ToList());
        }

        public ActionResult ShoppingCart()
        {
            return RedirectToAction("ShoppingCart", "Home");
        }

        public Company GetCompany()
        {
            //vai buscar o ID do utilizador atual
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userIdValue = userIdClaim.Value;
            var company = db.Company.Where(m => m.IdentityId == userIdValue).FirstOrDefault();

            return company;
        }

        public Employee GetEmployee()
        {
            //vai buscar o ID do utilizador atual
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            var userIdValue = userIdClaim.Value;
            var employee = db.Employee.Where(m => m.IdentityId == userIdValue).FirstOrDefault();

            return employee;
        }

        public string GetStatus(int stock)
        {
            string status;
            if (stock != 0)
            {
                status = "Available";
            }
            else
            {
                status = "Not Available";
            }
            return status;
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var dto = new ProductDTO();
            var categories = db.Category.ToList();
            dto.CategoryDropDown = new SelectList(categories, "IdCategory", "Name");
            return View(dto);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductDTO product)
        {
            var company = GetCompany();
            string status = GetStatus(product.Stock);
            var new_product = new Product()
            {
                Name = product.Name,
                Price = product.Price,
                OriginalPrice = product.Price,
                Stock = product.Stock,
                Category = db.Category.Find(product.Category),
                State = status,
                Company_Id = company.IdCompany,
                Company = company
            };

            try
            {
                db.Product.Add(new_product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

            return View(product);
        }

        //GET
        public ActionResult EditPromo(int? id)
        {
            var dto = new PromotionDTO();
            var product = db.Product.Find(id);
            dto.product = product;
            var promotions = db.Promotion.ToList();
            dto.PromoDropDown = new SelectList(promotions, "IdPromo", "Name");
            return View(dto);
        }

        [HttpPost]
        public ActionResult EditPromo(PromotionDTO promo)
        {
            var employee = GetEmployee();
            var product = db.Product.Find(promo.product.IdProduct);
            var promotion = db.Promotion.Find(promo.Promotion);
            promotion.Product.Add(product);
            var desconto = (decimal)product.OriginalPrice * promotion.Percent/100;
            product.Price = product.OriginalPrice - desconto;
            promotion.Employee = employee;
            product.Promotion.Add(promotion);
            try
            {
                db.Entry(promotion).State = EntityState.Modified;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

            return View(product);
        }
        public ActionResult RemovePromo(int? id)
        {
            var product = db.Product.Find(id);
            return View(product);
        }
        [HttpPost]
        public ActionResult RemovePromo(int id)
        {
            var product = db.Product.Find(id);
            product.Promotion.Clear();
            product.Price = product.OriginalPrice;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdProduct,Name,Price,Stock,State")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Product.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Product.Find(id);
            db.Product.Remove(product);
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
