using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using System.Configuration;
using ALRSSystem.Helpers;
using System.Web.Security;

namespace ALRSSystem.Controllers
{
    //[MyAuthorize("TipstaffRole, OwnerRole")]
    public class HomeTipStaffController : Controller
    {
        ALRSDB _db;

        public HomeTipStaffController()
        {
            _db = new ALRSDB();

        }

        public HomeTipStaffController(ALRSDB db)
        {
            _db = db;
        }

        public class JQueryDataTableParamModel
        {
            public string sEcho { get; set; }
            public string sSearch { get; set; }
            public int iDisplayLength { get; set; }
            public int iDisplayStart { get; set; }
            public string first_data { get; set; }
            public string second_data { get; set; }
        }

        public JsonResult GetAjaxDataTip(JQueryDataTableParamModel param)
        {

            var model = _db.ALRS
               .Where(r => r.DocumentDeleted == false && r.ALRSRequestorPosition == "Tip Staff")
               .Select(r => new ALRSVM { VM_Name = r.ALRSName, VM_Status = r.ALRSStatus, VM_LeaveType = r.ALRSLeaveType, VM_StartDate = r.ALRSStartDate, VM_EndDate = r.ALRSEndDate, VM_Duration = r.ALRSDuration, VM_Id = r.ID })
                .ToList();

            List<ALRSVM> modelsorted;

            int totalRowsCount = 0;
            int filteredRowsCount = 0;
            string[][] aaData;

            if (param.first_data != null && param.second_data != null)
            {
                List<ALRSVM> aa = new List<ALRSVM>();
                foreach (var group in model)
                {
                    var test = group.VM_Name.Replace(" ", "").Trim('-').ToLower();
                    if (test.Contains(param.first_data) &&
                        group.VM_Status.ToLower().Contains(param.second_data.Trim()))
                        aa.Add(group);
                }

                modelsorted = aa.OrderBy(r => r.VM_Name).ThenBy(e => e.VM_Status).ToList();
                aa = null;
                aaData = modelsorted.Select(d => new string[] { d.VM_Name, d.VM_Status, d.VM_LeaveType, d.VM_StartDate.ToString("dd-MM-yyyy"), d.VM_EndDate.ToString("dd-MM-yyyy"), d.VM_Duration.ToString(), d.VM_Id.ToString() }).ToArray();

                totalRowsCount = modelsorted.Count();
                filteredRowsCount = modelsorted.Count();

                return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = Convert.ToInt32(totalRowsCount),
                    iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount)
                }, JsonRequestBehavior.AllowGet);


            }

            if (param.sSearch != null)
            {
                if (param.sSearch.Length > 2)
                {

                    List<ALRSVM> aa = new List<ALRSVM>();
                    foreach (var group in model)
                    {
                        if (group.VM_Name.ToLower().Contains(param.sSearch.ToLower()) || group.VM_Status.ToLower().Contains(param.sSearch.ToLower()) ||
                            group.VM_LeaveType.ToLower().Contains(param.sSearch.ToLower()))

                            aa.Add(group);

                    }

                    modelsorted = aa.OrderBy(r => r.VM_Name).ThenBy(e => e.VM_Status).ToList();
                    aa = null;
                    aaData = modelsorted.Select(d => new string[] { d.VM_Name, d.VM_Status, d.VM_LeaveType, d.VM_StartDate.ToString("dd-MM-yyyy"), d.VM_EndDate.ToString("dd-MM-yyyy"), d.VM_Duration.ToString() }).ToArray();

                    totalRowsCount = modelsorted.Count();
                    filteredRowsCount = modelsorted.Count();

                    return Json(new
                    {
                        sEcho = param.sEcho,
                        aaData = aaData,
                        iTotalRecords = Convert.ToInt32(totalRowsCount),
                        iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount)
                    }, JsonRequestBehavior.AllowGet);
                }

            }



            totalRowsCount = model.Count();
            filteredRowsCount = model.Count();
            var vv = model.Select(r => new { r.VM_Name, r.VM_Status }).Distinct().OrderBy(r => r.VM_Name).ThenBy(e => e.VM_Status).ToList();

            aaData = vv.Select(d => new string[] { d.VM_Name, d.VM_Status, "", "", "", "" }).ToArray();


            return Json(new
            {
                sEcho = param.sEcho,
                aaData = aaData,
                iTotalRecords = Convert.ToInt32(totalRowsCount),
                iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount)
            }, JsonRequestBehavior.AllowGet);

        }

        [SharePointContextFilter]
        public ActionResult Index(string rp = "Tip Staff")
        {

            var requestorpositionTypeModel = getRequestorPositions();
            SelectList HRPNames = new SelectList(requestorpositionTypeModel, rp);
            ViewData["HRPNames"] = HRPNames;

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
            else
            {
                return View();
            }
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
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [SharePointContextFilter]
        public ActionResult Contact()
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
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}