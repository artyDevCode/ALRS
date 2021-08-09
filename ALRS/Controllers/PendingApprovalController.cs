using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using System.Web.Helpers;
using System.Configuration;

namespace ALRSSystem.Controllers
{
    public class PendingApprovalController : Controller
    {
        ALRSDB _db;

        public PendingApprovalController()
        {
            _db = new ALRSDB();

        }

        public PendingApprovalController(ALRSDB db)
        {
            _db = db;
        }

        [SharePointContextFilter]
        public ActionResult Index(string rp)
        {


            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = _db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";

            if ((ViewData["InAssociatesRole"] != "true") && ViewData["InTipstaffRole"] != "true" && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "ALRS", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }


            if (rp == null)
            {
                rp = "Associates";
            }

            var requestorpositionTypeModel = getRequestorPositions();
            SelectList PANames = new SelectList(requestorpositionTypeModel, rp);
            ViewData["PANames"] = PANames;

            var model = _db.ALRS
                .Where(r => r.ALRSRequestorPosition == rp && r.ALRSStatus == "Pending")
                .OrderBy(r => r.ALRSStartDate).ThenBy(e => e.ALRSEndDate)
                .ToList();

            //model.OrderByDescending(a => a.ALRSStartDate);
            return View(model);
        }
        private List<string> getRequestorPositions()
        {
            List<string> model = _db.RequestorPosition
                .Select(r => r.RequestorPositionName).ToList();
            return model;
        }


        [SharePointContextFilter]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [SharePointContextFilter]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}