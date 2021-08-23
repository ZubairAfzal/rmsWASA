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
    public class ResourceTypeParametersController : Controller
    {
        private rmsWasa01Entities db = new rmsWasa01Entities();

        // GET: ResourceTypeParameters
        public ActionResult Index()
        {
            var tblResourceTypeParameters = db.tblResourceTypeParameters.Include(t => t.tblCompany).Include(t => t.tblParameter).Include(t => t.tblResourceType);
            return View(tblResourceTypeParameters.ToList());
        }

        // GET: ResourceTypeParameters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResourceTypeParameter tblResourceTypeParameter = db.tblResourceTypeParameters.Find(id);
            if (tblResourceTypeParameter == null)
            {
                return HttpNotFound();
            }
            return View(tblResourceTypeParameter);
        }

        // GET: ResourceTypeParameters/Create
        public ActionResult Create()
        {
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName");
            ViewBag.parameterID = new SelectList(db.tblParameters, "parameterID", "parameterName");
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName");
            return View();
        }

        // POST: ResourceTypeParameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "resourceTypeParameterID,resourceTypeID,parameterID,companyID")] tblResourceTypeParameter tblResourceTypeParameter)
        {
            if (ModelState.IsValid)
            {
                db.tblResourceTypeParameters.Add(tblResourceTypeParameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResourceTypeParameter.companyID);
            ViewBag.parameterID = new SelectList(db.tblParameters, "parameterID", "parameterName", tblResourceTypeParameter.parameterID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResourceTypeParameter.resourceTypeID);
            return View(tblResourceTypeParameter);
        }

        // GET: ResourceTypeParameters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResourceTypeParameter tblResourceTypeParameter = db.tblResourceTypeParameters.Find(id);
            if (tblResourceTypeParameter == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResourceTypeParameter.companyID);
            ViewBag.parameterID = new SelectList(db.tblParameters, "parameterID", "parameterName", tblResourceTypeParameter.parameterID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResourceTypeParameter.resourceTypeID);
            return View(tblResourceTypeParameter);
        }

        // POST: ResourceTypeParameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "resourceTypeParameterID,resourceTypeID,parameterID,companyID")] tblResourceTypeParameter tblResourceTypeParameter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblResourceTypeParameter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResourceTypeParameter.companyID);
            ViewBag.parameterID = new SelectList(db.tblParameters, "parameterID", "parameterName", tblResourceTypeParameter.parameterID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResourceTypeParameter.resourceTypeID);
            return View(tblResourceTypeParameter);
        }

        // GET: ResourceTypeParameters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResourceTypeParameter tblResourceTypeParameter = db.tblResourceTypeParameters.Find(id);
            if (tblResourceTypeParameter == null)
            {
                return HttpNotFound();
            }
            return View(tblResourceTypeParameter);
        }

        // POST: ResourceTypeParameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblResourceTypeParameter tblResourceTypeParameter = db.tblResourceTypeParameters.Find(id);
            db.tblResourceTypeParameters.Remove(tblResourceTypeParameter);
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
