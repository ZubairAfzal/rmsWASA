using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class SubResourcesController : Controller
    {
        private rmsWasa01Entities db = new rmsWasa01Entities();

        // GET: SubResources
        public ActionResult Index()
        {
            var tblSubResources = db.tblSubResources.Include(t => t.tblCompany).Include(t => t.tblResource);
            return View(tblSubResources.ToList());
        }

        // GET: SubResources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSubResource tblSubResource = db.tblSubResources.Find(id);
            if (tblSubResource == null)
            {
                return HttpNotFound();
            }
            return View(tblSubResource);
        }

        // GET: SubResources/Create
        public ActionResult Create()
        {
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName");
            ViewBag.resourceID = new SelectList(db.tblResources, "resourceID", "resourceName");
            return View();
        }

        // POST: SubResources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "subResourceID,subResourceName,subResourceLocationName,subResourceDescription,subResourceCode,subResourceNumber,subResourceGeoLocatin,resourceID,companyID,subResourceStatus")] tblSubResource tblSubResource)
        {
            if (ModelState.IsValid)
            {
                db.tblSubResources.Add(tblSubResource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblSubResource.companyID);
            ViewBag.resourceID = new SelectList(db.tblResources, "resourceID", "resourceName", tblSubResource.resourceID);
            return View(tblSubResource);
        }

        // GET: SubResources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSubResource tblSubResource = db.tblSubResources.Find(id);
            if (tblSubResource == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblSubResource.companyID);
            ViewBag.resourceID = new SelectList(db.tblResources, "resourceID", "resourceName", tblSubResource.resourceID);
            return View(tblSubResource);
        }

        // POST: SubResources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "subResourceID,subResourceName,subResourceLocationName,subResourceDescription,subResourceCode,subResourceNumber,subResourceGeoLocatin,resourceID,companyID,subResourceStatus")] tblSubResource tblSubResource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblSubResource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblSubResource.companyID);
            ViewBag.resourceID = new SelectList(db.tblResources, "resourceID", "resourceName", tblSubResource.resourceID);
            return View(tblSubResource);
        }

        // GET: SubResources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSubResource tblSubResource = db.tblSubResources.Find(id);
            if (tblSubResource == null)
            {
                return HttpNotFound();
            }
            return View(tblSubResource);
        }

        // POST: SubResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSubResource tblSubResource = db.tblSubResources.Find(id);
            db.tblSubResources.Remove(tblSubResource);
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
