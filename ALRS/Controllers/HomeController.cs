using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using System.DirectoryServices;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.Web.Security;
using ALRSSystem.Helpers;
using System.Collections.Specialized;

namespace ALRSSystem.Controllers
{
    public class HomeController : Controller
    {
        ALRSDB _db;

        public HomeController()
        {
            _db = new ALRSDB();

        }

        public HomeController(ALRSDB db)
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


        //private static ALRSVM CheckValues(ALRS c)
        //{

        //    ALRSVM v = new ALRSVM
        //    {
        //        VM_Id = c.ID,
        //        VM_Name = c.ALRSName,
        //        VM_Status = c.ALRSStatus,
        //        VM_LeaveType = c.ALRSLeaveType,
        //        VM_StartDate = c.ALRSStartDate,
        //        VM_EndDate = c.ALRSEndDate,
        //        VM_Duration = c.ALRSDuration 
        //    };


        //    if (c.PF_Date_Returned == null && DateTime.Now > c.PF_Passport_Expiry_Date && c.PF_Status == "")
        //        v.VM_P_ViewColumn = "PASSPORT EXPIRED CURRENTLY HELD";

        //    if (c.PF_Date_Returned == null && DateTime.Now <= c.PF_Passport_Expiry_Date && c.PF_Status == "")
        //        v.VM_P_ViewColumn = "CURRENTLY HELD";

        //    if (c.PF_Date_Returned == null && DateTime.Now > c.PF_Passport_Expiry_Date && c.PF_Status == "Pending")
        //        v.VM_P_ViewColumn = "PENDING TRANSFER EXPIRED";

        //    if (c.PF_Date_Returned == null && DateTime.Now <= c.PF_Passport_Expiry_Date && c.PF_Status == "Pending")
        //        v.VM_P_ViewColumn = "PENDING TRANSFER";
        //    if (c.PF_Status == "Returned")
        //        v.VM_P_ViewColumn = "RETURNED";

        //    return v;
        //}
        
        public JsonResult GetAjaxData(JQueryDataTableParamModel param)
        {

            var model = _db.ALRS
               .Where(r => r.DocumentDeleted == false && r.ALRSRequestorPosition == "Associates")
               .Select(r => new ALRSVM { VM_Name = r.ALRSName, VM_Status = r.ALRSStatus, VM_LeaveType = r.ALRSLeaveType, VM_StartDate = r.ALRSStartDate, VM_EndDate = r.ALRSEndDate, VM_Duration = r.ALRSDuration, VM_Id = r.ID })
                .ToList();

            List<ALRSVM> modelsorted;

            int totalRowsCount = 0;
            int filteredRowsCount = 0;
            string[][] aaData;

            //Log4NetLogger logger2 = new Log4NetLogger();
            //logger2.Debug("return model1");
            

            if (param.first_data != null && param.second_data != null)
            {
                //throw new Exception("first=" + param.first_data + "second=" + param.second_data);
                
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
                            group.VM_LeaveType.ToLower().Contains(param.sSearch.ToLower()) )

                            aa.Add(group);

                    }

                    modelsorted = aa.OrderBy(r => r.VM_Name).ThenBy(e => e.VM_Status).ToList();
                    aa = null;
                    aaData = modelsorted.Select(d => new string[] { d.VM_Name, d.VM_Status, d.VM_LeaveType, d.VM_StartDate.ToString("dd-MM-yyyy"), d.VM_EndDate.ToString("dd-MM-yyyy"), d.VM_Duration.ToString()}).ToArray();

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
            //}

            totalRowsCount = model.Count();
            filteredRowsCount = model.Count();
            var vv = model.Select(r => new { r.VM_Name, r.VM_Status }).Distinct().OrderBy(r => r.VM_Name).ThenBy(e => e.VM_Status).ToList();

            aaData = vv.Select(d => new string[] { d.VM_Name, d.VM_Status, "", "", "", "" }).ToArray();

            //logger2.Debug("return model2=" + totalRowsCount.ToString());

            return Json(new
            {
                sEcho = param.sEcho,
                aaData = aaData,
                iTotalRecords = Convert.ToInt32(totalRowsCount),
                iTotalDisplayRecords = Convert.ToInt32(filteredRowsCount)
            }, JsonRequestBehavior.AllowGet);

        }

        [SharePointContextFilter]
        public ActionResult Index(string rp)
        {

            if (rp == null)
            {
                rp="Associates";
            }
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

        
        private List<Access> GetUserAccessGroups(string user)
        {
            List<Access> accessList = _db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            return accessList; ;
        }
        
        private static string IsInAnyRole(IPrincipal user, string role)
        {
            var userRoles = Roles.GetRolesForUser(user.Identity.Name);

            return "a"; // userRoles.Any(u => userRoles.Contains(u));
        }
        
        public static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = ConfigurationManager.AppSettings["ADConnection"]; 
            de.AuthenticationType = AuthenticationTypes.Secure;
            return de;
        }



        public List<string> GetUserGroups(string userName)
        {

            List<string> result = new List<string>();

            // establish domain context
            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add(p.Name);
                    }
                }
            }

            return result.ToList();

        }

        public List<GroupPrincipal> GetGroups(string userName)
        {

            //userName = "adm-ric";
            List<GroupPrincipal> result = new List<GroupPrincipal>();

            // establish domain context
            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);

            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, userName);

            // if found - grab its groups
            if (user != null)
            {
                PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();

                // iterate over all groups
                foreach (Principal p in groups)
                {
                    // make sure to add only group principals
                    if (p is GroupPrincipal)
                    {
                        result.Add((GroupPrincipal)p);
                    }
                }
            }

            return result.ToList();
            
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
            else

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