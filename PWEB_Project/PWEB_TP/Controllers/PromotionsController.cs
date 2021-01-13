using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using PWEB_TP.Models;

namespace PWEB_TP.Controllers
{
    [Authorize(Roles = "Employee, Company")]
    public class PromotionsController : Controller
    {
        private Pweb_DBEntities db = new Pweb_DBEntities();

        // GET: Promotions
        public ActionResult Index()
        {
            return View(db.Promotion.ToList());
        }

        public ActionResult ShoppingCart()
        {
            return RedirectToAction("ShoppingCart", "Home");
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


        // GET: Promotions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotion.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // GET: Promotions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Promotions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPromo,Name,Percent")] Promotion promotion)
        {
            var employee = GetEmployee();
            if (ModelState.IsValid)
            {
                promotion.Employee = employee;
                db.Promotion.Add(promotion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotion.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPromo,Name,Percent")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(promotion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Promotion promotion = db.Promotion.Find(id);
            if (promotion == null)
            {
                return HttpNotFound();
            }
            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            List<Product> list = new List<Product>();
            Promotion promotion = db.Promotion.Find(id);
            foreach (var item in promotion.Product)
            {
                list.Add(db.Product.Find(item.IdProduct));
            }

            for (int i = 0; i < list.Count(); i++)
            {
                list.ElementAt(i).Price = list.ElementAt(i).OriginalPrice;
                list.ElementAt(i).Promotion.Clear();
                db.Entry(list.ElementAt(i)).State = EntityState.Modified;
            }
            
            db.Promotion.Remove(promotion);
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
