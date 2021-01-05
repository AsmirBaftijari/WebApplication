using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class EintraegerController : Controller
    {
        private DBModels2 db = new DBModels2();

        // GET: Eintraeges
        public ActionResult Index()
        {
            if (Session["isAdmin"] == null)
            {
                var Eintraeges = db.Eintraeges.Where(E => E.Autorisiert_ID != null);
                return View(db.Eintraeges.ToList());
            }
            else
            {
                var Eintraeges = db.Eintraeges.Include(E => E.Eintrag_Id);
                return View(db.Eintraeges.ToList());
            }
        }

        // GET: Eintraeges/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eintraege eintraege = db.Eintraeges.Find(id);
            if (eintraege == null)
            {
                return HttpNotFound();
            }
            return View(eintraege);
        }

        // GET: Eintraeges/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Eintraeges/Create
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Eintrag_Id,Vorname,Nachname,Datum,Kommentar")] Eintraege eintraege)
        {
            if (ModelState.IsValid)
            {
                eintraege.Eintrag_Id = Guid.NewGuid();
                db.Eintraeges.Add(eintraege);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(eintraege);
        }
        public ActionResult Autorisieren(int? id)
        {
            if (Session["isAdmin"] == null)
            {
                return RedirectToAction("Index");
            }
            else
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Eintraege eintraege = db.Eintraeges.Find(id);
                db.Eintraeges.Find(id).Autorisiert_ID = (int)Session["isAdmin"];
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET: Eintraeges/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eintraege eintraege = db.Eintraeges.Find(id);
            if (eintraege == null)
            {
                return HttpNotFound();
            }
            return View(eintraege);
        }

        // POST: Eintraeges/Edit/5
        // Aktivieren Sie zum Schutz vor Angriffen durch Overposting die jeweiligen Eigenschaften, mit denen eine Bindung erfolgen soll. 
        // Weitere Informationen finden Sie unter https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Eintrag_Id,Vorname,Nachname,Datum,Kommentar")] Eintraege eintraege)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eintraege).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(eintraege);
        }

        // GET: Eintraeges/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eintraege eintraege = db.Eintraeges.Find(id);
            if (eintraege == null)
            {
                return HttpNotFound();
            }
            return View(eintraege);
        }

        // POST: Eintraeges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Eintraege eintraege = db.Eintraeges.Find(id);
            db.Eintraeges.Remove(eintraege);
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
