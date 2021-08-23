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
    public class ResourceTypeController : Controller
    {
        private rmsWasa01Entities db = new rmsWasa01Entities();

        // GET: ResourceType
        public ActionResult Index()
        {
            var tblResourceTypes = db.tblResourceTypes.Include(t => t.tblCompany);
            return View(tblResourceTypes.ToList());
        }

        // GET: ResourceType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResourceType tblResourceType = db.tblResourceTypes.Find(id);
            if (tblResourceType == null)
            {
                return HttpNotFound();
            }
            return View(tblResourceType);
        }

        // GET: ResourceType/Create
        public ActionResult Create()
        {
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName");
            return View();
        }

        // POST: ResourceType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "resourceTypeID,resourceTypeName,resourceTypeDescription,parameterCount,separator,companyID")] tblResourceType tblResourceType)
        {
            if (ModelState.IsValid)
            {
                db.tblResourceTypes.Add(tblResourceType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResourceType.companyID);
            return View(tblResourceType);
        }

        // GET: ResourceType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResourceType tblResourceType = db.tblResourceTypes.Find(id);
            if (tblResourceType == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResourceType.companyID);
            return View(tblResourceType);
        }

        // POST: ResourceType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "resourceTypeID,resourceTypeName,resourceTypeDescription,parameterCount,separator,companyID")] tblResourceType tblResourceType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblResourceType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResourceType.companyID);
            return View(tblResourceType);
        }

        // GET: ResourceType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResourceType tblResourceType = db.tblResourceTypes.Find(id);
            if (tblResourceType == null)
            {
                return HttpNotFound();
            }
            return View(tblResourceType);
        }

        // POST: ResourceType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblResourceType tblResourceType = db.tblResourceTypes.Find(id);
            db.tblResourceTypes.Remove(tblResourceType);
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
