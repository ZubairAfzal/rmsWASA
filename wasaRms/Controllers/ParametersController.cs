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
    public class ParametersController : Controller
    {
        private rmsWasa01Entities db = new rmsWasa01Entities();

        // GET: Parameters
        public ActionResult Index()
        {
            var tblParameters = db.tblParameters.Include(t => t.tblCompany);
            return View(tblParameters.ToList());
        }

        // GET: Parameters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblParameter tblParameter = db.tblParameters.Find(id);
            if (tblParameter == null)
            {
                return HttpNotFound();
            }
            return View(tblParameter);
        }

        // GET: Parameters/Create
        public ActionResult Create()
        {
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName");
            return View();
        }

        // POST: Parameters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "parameterID,parameterName,parameterUnit,parameterMinThr,parameterMaxThr,companyID")] tblParameter tblParameter)
        {
            if (ModelState.IsValid)
            {
                db.tblParameters.Add(tblParameter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblParameter.companyID);
            return View(tblParameter);
        }

        // GET: Parameters/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblParameter tblParameter = db.tblParameters.Find(id);
            if (tblParameter == null)
            {
                return HttpNotFound();
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblParameter.companyID);
            return View(tblParameter);
        }

        // POST: Parameters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "parameterID,parameterName,parameterUnit,parameterMinThr,parameterMaxThr,companyID")] tblParameter tblParameter)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblParameter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblParameter.companyID);
            return View(tblParameter);
        }

        // GET: Parameters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblParameter tblParameter = db.tblParameters.Find(id);
            if (tblParameter == null)
            {
                return HttpNotFound();
            }
            return View(tblParameter);
        }

        // POST: Parameters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblParameter tblParameter = db.tblParameters.Find(id);
            db.tblParameters.Remove(tblParameter);
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
