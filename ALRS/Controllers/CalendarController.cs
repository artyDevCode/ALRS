using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using System.Data.Entity;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Data;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;
using DayPilot.Web.Mvc.Events.Common;
using DayPilot.Web.Mvc.Events.Navigator;
using DayPilot.Web.Mvc.Json;
using BeforeCellRenderArgs = DayPilot.Web.Mvc.Events.Calendar.BeforeCellRenderArgs;
using TimeRangeSelectedArgs = DayPilot.Web.Mvc.Events.Calendar.TimeRangeSelectedArgs;
using System.Configuration;

namespace ALRSSystem.Controllers
{
    public class CalendarController : Controller
    {
        public static string sphostUrl; 
        
        private ALRSDB db = new ALRSDB();

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

            if ((ViewData["InAssociatesRole"] != "true") && ViewData["InTipstaffRole"] != "true" && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "ALRS", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            else
            {
                sphostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri; 
                return View();
            }

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
