using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using wasaRms.Models;

namespace wasaRms.Controllers
{
    public class tblResourcesController : Controller
    {
        private rmsWasa01Entities db = new rmsWasa01Entities();

        // GET: tblResources
        public async Task<ActionResult> Index()
        {
            var tblResources = db.tblResources.Include(t => t.tblCompany).Include(t => t.tblResourceType).Include(t => t.tblSubResource).Include(t => t.tblUser);
            return View(await tblResources.ToListAsync());
        }

        // GET: tblResources/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResource tblResource = await db.tblResources.FindAsync(id);
            if (tblResource == null)
            {
                return HttpNotFound();
            }
            return View(tblResource);
        }

        // GET: tblResources/Create
        public ActionResult Create()
        {
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName");
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName");
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName");
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName");
            return View();
        }

        // POST: tblResources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "resourceID,resourceName,resourceLocationName,resourceDescription,resourceCode,resourceNumber,resourceGeoLocatin,resourceTypeID,companyID,resourceStatus,managedBy,minThr,maxThr")] tblResource tblResource)
        {
            if (ModelState.IsValid)
            {
                db.tblResources.Add(tblResource);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResource.companyID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResource.resourceTypeID);
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName", tblResource.resourceID);
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName", tblResource.managedBy);
            return View(tblResource);
        }

        // GET: tblResources/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResource tblResource = await db.tblResources.FindAsync(id);
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

        // POST: tblResources/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "resourceID,resourceName,resourceLocationName,resourceDescription,resourceCode,resourceNumber,resourceGeoLocatin,resourceTypeID,companyID,resourceStatus,managedBy,minThr,maxThr")] tblResource tblResource)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblResource).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.companyID = new SelectList(db.tblCompanies, "companyID", "companyName", tblResource.companyID);
            ViewBag.resourceTypeID = new SelectList(db.tblResourceTypes, "resourceTypeID", "resourceTypeName", tblResource.resourceTypeID);
            ViewBag.resourceID = new SelectList(db.tblSubResources, "resourceID", "subResourceName", tblResource.resourceID);
            ViewBag.managedBy = new SelectList(db.tblUsers, "userID", "userFullName", tblResource.managedBy);
            return View(tblResource);
        }

        // GET: tblResources/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblResource tblResource = await db.tblResources.FindAsync(id);
            if (tblResource == null)
            {
                return HttpNotFound();
            }
            return View(tblResource);
        }

        // POST: tblResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            tblResource tblResource = await db.tblResources.FindAsync(id);
            db.tblResources.Remove(tblResource);
            await db.SaveChangesAsync();
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
