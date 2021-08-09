using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using System.Configuration;

namespace ALRSSystem.Controllers
{
    public class RequestorPositionController : Controller
    {
        private ALRSDB db = new ALRSDB();

        // GET: /RequestorPosition/
        [SharePointContextFilter]
        public ActionResult Index()
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";

            if ((ViewData["InAssociatesRole"] == "false") && ViewData["InTipstaffRole"] == "false" && ViewData["InOwnerRole"] == "false")
            {
                return RedirectToAction("Unauthorised", "ALRS", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            else { return View(db.RequestorPosition.ToList()); }            
            
        }

        // GET: /RequestorPosition/Details/5
        [SharePointContextFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestorPosition requestorposition = db.RequestorPosition.Find(id);
            if (requestorposition == null)
            {
                return HttpNotFound();
            }
            return View(requestorposition);
        }

        // GET: /RequestorPosition/Create
        [SharePointContextFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /RequestorPosition/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,RequestorPositionName")] RequestorPosition requestorposition)
        {
            if (ModelState.IsValid)
            {
                db.RequestorPosition.Add(requestorposition);
                db.SaveChanges();
                return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            return View(requestorposition);
        }

        // GET: /RequestorPosition/Edit/5
        [SharePointContextFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestorPosition requestorposition = db.RequestorPosition.Find(id);
            if (requestorposition == null)
            {
                return HttpNotFound();
            }
            return View(requestorposition);
        }

        // POST: /RequestorPosition/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,RequestorPositionName")] RequestorPosition requestorposition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requestorposition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(requestorposition);
        }

        // GET: /RequestorPosition/Delete/5
        [SharePointContextFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequestorPosition requestorposition = db.RequestorPosition.Find(id);
            if (requestorposition == null)
            {
                return HttpNotFound();
            }
            return View(requestorposition);
        }

        // POST: /RequestorPosition/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RequestorPosition requestorposition = db.RequestorPosition.Find(id);
            db.RequestorPosition.Remove(requestorposition);
            db.SaveChanges();
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
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
