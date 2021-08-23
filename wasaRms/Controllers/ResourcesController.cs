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
    public class ResourcesController : Controller
    {
        private rmsWasa01Entities db = new rmsWasa01Entities();

        // GET: Resources
        public ActionResult Index()
        {
            var tblResources = db.tblResources.Include(t => t.tblCompany).Include(t => t.tblResourceType).Include(t => t.tblSubResource).Include(t => t.tblUser);
            return View(tblResources.ToList());
        }

        // GET: Resources/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResource tblResource = db.tblResources.Find(id);
            if (tblResource == null)
            {
                return HttpNotFound();
            }
            return View(tblResource);
        }

        // GET: Resources/Create
        public ActionResult Create()
        {
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName");
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName");
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName");
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName");
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "resourceID,resourceName,resourceLocationName,resourceDescription,resourceCode,resourceNumber,resourceGeoLocatin,resourceTypeID,companyID,resourceStatus,managedBy,minThr,maxThr")] tblResource tblResource)
        {
            if (ModelState.IsValid)
            {
                db.tblResources.Add(tblResource);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResource.companyID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResource.resourceTypeID);
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName", tblResource.resourceID);
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName", tblResource.managedBy);
            return View(tblResource);
        }

        // GET: Resources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResource tblResource = db.tblResources.Find(id);
            if (tblResource == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResource.companyID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResource.resourceTypeID);
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName", tblResource.resourceID);
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName", tblResource.managedBy);
            return View(tblResource);
        }

        // POST: Resources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "resourceID,resourceName,resourceLocationName,resourceDescription,resourceCode,resourceNumber,resourceGeoLocatin,resourceTypeID,companyID,resourceStatus,managedBy,minThr,maxThr")] tblResource tblResource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblResource).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResource.companyID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResource.resourceTypeID);
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName", tblResource.resourceID);
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName", tblResource.managedBy);
            return View(tblResource);
        }

        // GET: Resources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResource tblResource = db.tblResources.Find(id);
            if (tblResource == null)
            {
                return HttpNotFound();
            }
            return View(tblResource);
        }

        // POST: Resources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblResource tblResource = db.tblResources.Find(id);
            db.tblResources.Remove(tblResource);
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
