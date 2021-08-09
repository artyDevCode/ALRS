using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using ALRSSystem.Helpers;
using System.DirectoryServices;
using System.IO;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;


namespace ALRSSystem.Controllers
{
    public class ALRSController : Controller
    {
        private ALRSDB db = new ALRSDB();

        // GET: /ALRS/
        [SharePointContextFilter]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        // GET: /ALRS/Details/5
        [SharePointContextFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InChangeNameRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("change")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";

            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return HttpNotFound();
            }
            return View(alrs);
        }

        [SharePointContextFilter]
        public ActionResult Unauthorised(string alertMessage)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InChangeNameRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("change")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";
            TempData["alertMessage"] = alertMessage;
            return View("Unauthorised");
        }

        [SharePointContextFilter]
        public ActionResult Alert(string alertMessage)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InChangeNameRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("change")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";
            TempData["alertMessage"] = alertMessage;
            return View();
        }

        private List<string> getRequestorPositions()
        {
            List<string> model = db.RequestorPosition
                .Select(r => r.RequestorPositionName).ToList();
            return model;
        }

        public static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = ConfigurationManager.AppSettings["ADConnection"]; //"LDAP://dev.justice.vic.gov.au/CN=Users,DC=dev,DC=justice,DC=vic,DC=gov,DC=au"; 
            de.AuthenticationType = AuthenticationTypes.Secure;
            return de;
        } 

        public string GetUserEmailAddress(string user)
        {
            string uemail = "";

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);
            UserPrincipal u = UserPrincipal.FindByIdentity(ctx, user);
            if (u != null)
            {
                uemail = u.EmailAddress;
            }
            return uemail;
        }
        public JsonResult getUsers(string term)
        {

            DirectoryEntry de = GetDirectoryEntry();
            DirectorySearcher deSearch = new DirectorySearcher();
            List<string> groupMembers = new List<string>();

            de.Password = ConfigurationManager.AppSettings["ADConnectionPassword"];
            de.Username = ConfigurationManager.AppSettings["ADConnectionUserName"];

            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user) (cn=" + "*" + "))";

            foreach (SearchResult sr in deSearch.FindAll())
            {
                foreach (string str in sr.Properties["name"])
                {

                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);
                    UserPrincipal u = UserPrincipal.FindByIdentity(ctx, str);
                    if (u.EmailAddress != null)
                    { groupMembers.Add(str); }

                }
            }

            return Json(groupMembers, JsonRequestBehavior.AllowGet);
        }

        public List<string> UserExists()
        {
            DirectoryEntry de = GetDirectoryEntry();
            DirectorySearcher deSearch = new DirectorySearcher();
            List<string> groupMembers = new List<string>();

            try
            {
                de.Password = ConfigurationManager.AppSettings["ADConnectionPassword"]; 
                de.Username = ConfigurationManager.AppSettings["ADConnectionUserName"]; 

                deSearch.SearchRoot = de;
                deSearch.Filter = "(&(objectClass=user) (cn=" + "*" + "))";  //"(&(objectCategory=Private Cloud)(objectClass=user))"; 

                foreach (SearchResult sr in deSearch.FindAll())
                {
                    foreach (string str in sr.Properties["name"])
                    {
                        if (GetUserEmailAddress(str) != null)
                        { groupMembers.Add(str); }
                    }
                }
            }
            catch
            { }

            return groupMembers; ;
        }

        // GET: /ALRS/Create
        [SharePointContextFilter]
        public ActionResult Create()
        {

            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InChangeNameRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("change")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";


            if (ViewData["InOwnerRole"] == "true" || ViewData["InChangeNameRole"] == "true")
            {
                ViewData["InOwnerRole"] = "true";
                //get active directory user list
                var ADModel = UserExists();
                SelectList ADNames = new SelectList(ADModel);
                ViewData["ADNames"] = ADNames;
            }
            else
            {
                ViewData["InOwnerRole"] = "false";
            }

            var requestorpositionTypeModel = getRequestorPositions();
            SelectList ARPNames = new SelectList(requestorpositionTypeModel);
            ViewData["ARPNames"] = ARPNames;

            List<aJudgeList> list2 = new List<aJudgeList>();
            list2.Add(new aJudgeList() { ID = true, Name = "Yes" });
            list2.Add(new aJudgeList() { ID = false, Name = "  No" });
            SelectList sl2 = new SelectList(list2, "ID", "Name");

            ALRS model = new ALRS();

            CultureInfo provider = CultureInfo.InvariantCulture;
            var result = DateTime.ParseExact(DateTime.Now.ToShortDateString(), "dd/MM/yyyy", provider);
            model.ALRSStartDate = result;
            model.ALRSEndDate = result;

            index = User.Identity.Name.IndexOf("\\");
            model.ALRSName = User.Identity.Name.Substring(index + 1);

            model.ALRSDuration = 1;
            model.JudgeOnLeaveList = sl2;
            model.DocumentDateCreated = DateTime.Now;
            model.DocumentDateModified = DateTime.Now;
            model.DocumentCreatedBy = User.Identity.Name;
            model.DocumentModifiedBy = User.Identity.Name;
            model.ALRSStatus = "Pending";
            model.ALRSAllDayEvent = "true";

            ViewData["ModelState"] = "false";

            return View(model);
        }

        // POST: /ALRS/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public async Task<ActionResult> Create([Bind(Include = "ID,ALRSName,ALRSStartDate,ALRSEndDate,ALRSDuration,ALRSComments,ALRSJudgeOnLeave,ALRSStatus,ALRSApproverComments,ALRSApproverResponseDate,ALRSRequestorPosition,ALRSDisclaimer,DocumentDateCreated,DocumentCreatedBy,DocumentDateModified,DocumentModifiedBy,ALRSBackColor,ALRSAllDayEvent, DocumentDeleted,ALRSLeaveType,ToEmailAddress")] ALRS alrs)
        {

            //throw new Exception(alrs.ID.ToString() + alrs.ALRSName + "-" + alrs.ALRSStartDate + "-" + alrs.ALRSEndDate + "-" + alrs.ALRSDuration + alrs.ALRSComments + alrs.ALRSJudgeOnLeave + alrs.ALRSStatus + alrs.ALRSApproverComments + alrs.ALRSApproverResponseDate + alrs.ALRSRequestorPosition + alrs.ALRSDisclaimer + alrs.DocumentDateCreated + alrs.DocumentCreatedBy + alrs.DocumentDateModified + alrs.DocumentModifiedBy + alrs.ALRSBackColor + alrs.ALRSAllDayEvent + alrs.DocumentDeleted + alrs.ALRSLeaveType + alrs.ToEmailAddress);

            if (ModelState.IsValid)
            {

                ViewData["ModelState"] = "true";

                alrs.ALRSStartDate = Convert.ToDateTime(alrs.ALRSStartDate.ToString("dd/MM/yyyy" + " 09:00:00"));
                alrs.ALRSEndDate = Convert.ToDateTime(alrs.ALRSEndDate.ToString("dd/MM/yyyy" + " 17:00:00"));
                alrs.DocumentDateCreated = DateTime.Now;
                alrs.DocumentDateModified = DateTime.Now;
                alrs.DocumentCreatedBy = User.Identity.Name;
                alrs.DocumentModifiedBy = User.Identity.Name;
                alrs.ALRSStatus = "Pending";
                alrs.ALRSBackColor = "silver";
                alrs.ALRSAllDayEvent = "true";
                alrs.ALRSLeaveType = "Additional Leave";
                alrs.ALRSApproverResponseDate = DateTime.Now;

                db.ALRS.Add(alrs);
                await db.SaveChangesAsync();

                try
                {
                    //Send New Request Email
                    User newUser = new User();
                    newUser.ALRSId = alrs.ID;
                    newUser.Subject = "A Leave Request (Additional Leave) for " + alrs.ALRSName + " requires your attention";
                    newUser.ALRSLink = @Url.AbsoluteAction("Details", "ALRS", new { id = alrs.ID });

                    if (alrs.ALRSRequestorPosition == "Associates")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.AssociateApprover, x.AssociateApproverEmail });
                        newUser.ToEmail = toap.First().AssociateApproverEmail;
                        newUser.ToName = toap.First().AssociateApprover;
                        alrs.ALRSRequestorPosition = "Associates";
                    }
                    if (alrs.ALRSRequestorPosition == "Tip Staff")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.TipStaffApprover, x.TipStaffApproverEmail });
                        newUser.ToEmail = toap.First().TipStaffApproverEmail;
                        newUser.ToName = toap.First().TipStaffApprover;
                        alrs.ALRSRequestorPosition = "Tipstaff";
                    }

                    newUser.FromEmail = GetUserEmailAddress(alrs.ALRSName); 
                    newUser.FromName = alrs.ALRSName;
                    newUser.ALRSEndDate = alrs.ALRSEndDate;
                    newUser.ALRSStartDate = alrs.ALRSStartDate;

                    new MailController().LeaveRequestEmail(newUser).Deliver();
                }
                catch (Exception ex)
                {
                    Log4NetLogger logger2 = new Log4NetLogger();
                    logger2.Debug(ex.Source + "-" + ex.Message + "-" + ex.InnerException);
                }

                //Response.Write("<script>alert('Your alert box')</script>"); 
                //ClientScriptManager clientScript;
                //clientScript.RegisterStartupScript(typeof(this.Page), "warning", "submitted", true);
                //return JavaScript("<script>alert(\"some message\")</script>");
                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Your leave request has been sent.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            else
            {
                ViewData["ModelState"] = "false";
            }

            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InChangeNameRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("change")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff approver")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesApproverRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates approver")).Count() > 0 ? "true" : "false";
            ViewData["InTipStaffRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("tipstaff")).Count() > 0 ? "true" : "false";
            ViewData["InAssociatesRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("associates")).Count() > 0 ? "true" : "false";

            if (ViewData["InOwnerRole"] == "true" || ViewData["InChangeNameRole"] == "true")
            {
                ViewData["InOwnerRole"] = "true";
                UserExists();  //get active directory user list
                var ADModel = UserExists();
                SelectList ADNames = new SelectList(ADModel);
                ViewData["ADNames"] = ADNames;
            }
            else
            {
                ViewData["InOwnerRole"] = "false";
            }


            var requestorpositionTypeModel = getRequestorPositions();
            SelectList ARPNames = new SelectList(requestorpositionTypeModel);
            ViewData["ARPNames"] = ARPNames;

            List<aJudgeList> list2 = new List<aJudgeList>();
            list2.Add(new aJudgeList() { ID = true, Name = "Yes" });
            list2.Add(new aJudgeList() { ID = false, Name = "No" });
            SelectList sl2 = new SelectList(list2, "ID", "Name");

            alrs.JudgeOnLeaveList = sl2;
            alrs.DocumentDateCreated = DateTime.Now;
            alrs.DocumentDateModified = DateTime.Now;
            alrs.DocumentCreatedBy = User.Identity.Name;
            alrs.DocumentModifiedBy = User.Identity.Name;
            alrs.ALRSStatus = "Pending";
            alrs.ALRSBackColor = "silver";
            alrs.ALRSAllDayEvent = "true";

            return View(alrs);
        }

        ////Jsonresult to list AD users
        //public JsonResult getUsers(string term)
        //{

        //    DirectoryEntry de = GetDirectoryEntry();
        //    DirectorySearcher deSearch = new DirectorySearcher();
        //    List<string> groupMembers = new List<string>();

        //    de.Password = ConfigurationManager.AppSettings["ADConnectionPassword"];
        //    de.Username = ConfigurationManager.AppSettings["ADConnectionUserName"];

        //    deSearch.SearchRoot = de;
        //    deSearch.Filter = "(&(objectClass=user) (cn=" + "*" + "))";

        //    foreach (SearchResult sr in deSearch.FindAll())
        //    {
        //        foreach (string str in sr.Properties["name"])
        //        {

        //            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);
        //            UserPrincipal u = UserPrincipal.FindByIdentity(ctx, str);
        //            if (u.EmailAddress != null)
        //            { groupMembers.Add(str); }

        //        }
        //    }

        //    return Json(groupMembers, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult TestAjax()
        {
            return JavaScript("<script>alert(\"some message\")</script>");
        }
        
        // GET: /ALRS/Edit/5
        [SharePointContextFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Leave request not found, contact administrator.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Leave request not found, contact administrator.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(alrs);
        }

        // POST: /ALRS/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,ALRSName,ALRSStartDate,ALRSEndDate,ALRSDuration,ALRSComments,ALRSJudgeOnLeave,ALRSStatus,ALRSApproverComments,ALRSApproverResponseDate,ALRSRequestorPosition,ALRSDisclaimer,DocumentDateCreated,DocumentCreatedBy,DocumentDateModified,DocumentModifiedBy,ALRSBackColor,ALRSAllDayEvent, DocumentDeleted,ALRSLeaveType,ToEmailAddress")] ALRS alrs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(alrs).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(alrs);
        }

        // GET: /ALRS/Delete/5
        [SharePointContextFilter]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Leave request not found, contact administrator.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Leave request not found, contact administrator.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(alrs);
        }

        // POST: /ALRS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ALRS alrs = db.ALRS.Find(id);
            db.ALRS.Remove(alrs);
            db.SaveChanges();
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        // GET: /ALRS/Cancel/5
        [SharePointContextFilter]
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return HttpNotFound();
            }
            return View(alrs);
        }

        // POST: /ALRS/Cancel/5
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelConfirmed(int id)
        {
            ALRS alrs = db.ALRS.Find(id);
            alrs.ALRSStatus = "Cancelled";
            alrs.ALRSBackColor = "indianred";
            alrs.DocumentDateModified = DateTime.Now;
            alrs.DocumentModifiedBy = User.Identity.Name;
            db.Entry(alrs).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        // GET: /ALRS/Approved/5
        [SharePointContextFilter]
        public ActionResult Approved(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return HttpNotFound();
            }
            return View(alrs);
        }

        // POST: /ALRS/Approved/5
        [HttpPost, ActionName("Approved")]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovedConfirmed([Bind(Include = "ID,ALRSName,ALRSStartDate,ALRSEndDate,ALRSDuration,ALRSComments,ALRSJudgeOnLeave,ALRSStatus,ALRSApproverComments,ALRSApproverResponseDate,ALRSRequestorPosition,ALRSDisclaimer,DocumentDateCreated,DocumentCreatedBy,DocumentDateModified,DocumentModifiedBy,ALRSBackColor,ALRSAllDayEvent, DocumentDeleted,ALRSLeaveType,ToEmailAddress")] ALRS alrs)
        {

            if (ModelState.IsValid)
            {
                alrs.ALRSStatus = "Approved";
                alrs.ALRSBackColor = "green";
                alrs.ALRSAllDayEvent = "true";
                alrs.ALRSLeaveType = "Additional Leave";
                alrs.DocumentDateModified = DateTime.Now;
                alrs.DocumentModifiedBy = User.Identity.Name;
                alrs.ALRSApproverResponseDate = DateTime.Now;
                db.Entry(alrs).State = EntityState.Modified;
                db.SaveChanges();

                try
                {
                    //Send Approved Email
                    User newUser = new User();
                    newUser.ALRSId = alrs.ID;
                    newUser.Subject = "Leave Request has been approved";
                    newUser.ALRSLink = @Url.AbsoluteAction("Details", "ALRS", new { id = alrs.ID }); 

                    if (alrs.ALRSRequestorPosition == "Associates")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.AssociateApprover, x.AssociateApproverEmail });
                        newUser.FromEmail = toap.First().AssociateApproverEmail;
                        newUser.FromName = toap.First().AssociateApprover;
                        alrs.ALRSRequestorPosition = "Associates";
                    }
                    if (alrs.ALRSRequestorPosition == "Tip Staff")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.TipStaffApprover, x.TipStaffApproverEmail });
                        newUser.FromEmail = toap.First().TipStaffApproverEmail;
                        newUser.FromName = toap.First().TipStaffApprover;
                        alrs.ALRSRequestorPosition = "Tipstaff";
                    }

                    newUser.ToEmail = GetUserEmailAddress(alrs.ALRSName); 
                    newUser.ToName = alrs.ALRSName;
                    newUser.ALRSStartDate = alrs.ALRSStartDate;
                    newUser.ALRSEndDate = alrs.ALRSEndDate;
                    newUser.ApproverComments = alrs.ALRSApproverComments;
                    new MailController().ApprovedRequestEmail(newUser).Deliver();
                }
                catch (Exception ex)
                {
                    Log4NetLogger logger2 = new Log4NetLogger();
                    logger2.Debug(ex.Source + "-" + ex.Message + "-" + ex.InnerException);
                }

                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Leave request notification has been sent.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(alrs);

        }

        // GET: /ALRS/ApprovedNewDates/5
        [SharePointContextFilter]
        public ActionResult ApprovedNewDates(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return HttpNotFound();
            }
            return View(alrs);
        }

        // POST: /ALRS/ApprovedNewDates/5
        [HttpPost, ActionName("ApprovedNewDates")]
        [ValidateAntiForgeryToken]
        public ActionResult ApprovedNewDatesConfirmed([Bind(Include = "ID,ALRSName,ALRSStartDate,ALRSEndDate,ALRSDuration,ALRSComments,ALRSJudgeOnLeave,ALRSStatus,ALRSApproverComments,ALRSApproverResponseDate,ALRSRequestorPosition,ALRSDisclaimer,DocumentDateCreated,DocumentCreatedBy,DocumentDateModified,DocumentModifiedBy,ALRSBackColor,ALRSAllDayEvent, DocumentDeleted,ALRSLeaveType,FromEmailAddress,ToEmailAddress")] ALRS alrs)
        {
            //throw new Exception(alrs.ID.ToString() + alrs.ALRSName + "-" + alrs.ALRSStartDate + "-" + alrs.ALRSEndDate + "-" + alrs.ALRSDuration + alrs.ALRSComments + alrs.ALRSJudgeOnLeave + alrs.ALRSStatus + alrs.ALRSApproverComments + alrs.ALRSApproverResponseDate + alrs.ALRSRequestorPosition + alrs.ALRSDisclaimer + alrs.DocumentDateCreated + alrs.DocumentCreatedBy + alrs.DocumentDateModified + alrs.DocumentModifiedBy + alrs.ALRSBackColor + alrs.ALRSAllDayEvent + alrs.DocumentDeleted + alrs.ALRSLeaveType + alrs.ToEmailAddress);

            if (ModelState.IsValid)
            {
                alrs.ALRSStartDate = Convert.ToDateTime(alrs.ALRSStartDate.ToString("dd/MM/yyyy" + " 09:00:00"));
                alrs.ALRSEndDate = Convert.ToDateTime(alrs.ALRSEndDate.ToString("dd/MM/yyyy" + " 17:00:00"));
                alrs.ALRSStatus = "Approved";
                alrs.ALRSBackColor = "green";
                alrs.ALRSAllDayEvent = "true";
                alrs.ALRSLeaveType = "Additional Leave";
                alrs.DocumentDateModified = DateTime.Now;
                alrs.DocumentModifiedBy = User.Identity.Name;
                alrs.ALRSApproverResponseDate = DateTime.Now;
                db.Entry(alrs).State = EntityState.Modified;
                db.SaveChanges();

                try
                {
                    //Send Approved Email
                    User newUser = new User();
                    newUser.ALRSId = alrs.ID;
                    newUser.Subject = "Leave Request has been approved with new dates";
                    newUser.ALRSLink = @Url.AbsoluteAction("Details", "ALRS", new { id = alrs.ID });

                    if (alrs.ALRSRequestorPosition == "Associates")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.AssociateApprover, x.AssociateApproverEmail });
                        newUser.FromEmail = toap.First().AssociateApproverEmail;
                        newUser.FromName = toap.First().AssociateApprover;
                        alrs.ALRSRequestorPosition = "Associates";
                    }
                    if (alrs.ALRSRequestorPosition == "Tip Staff")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.TipStaffApprover, x.TipStaffApproverEmail });
                        newUser.FromEmail = toap.First().TipStaffApproverEmail;
                        newUser.FromName = toap.First().TipStaffApprover;
                        alrs.ALRSRequestorPosition = "Tipstaff";
                    }

                    newUser.ToEmail = GetUserEmailAddress(alrs.ALRSName); //TODO + Add alert;
                    newUser.ToName = alrs.ALRSName;
                    newUser.ALRSStartDate = alrs.ALRSStartDate;
                    newUser.ALRSEndDate = alrs.ALRSEndDate;
                    newUser.ApproverComments = alrs.ALRSApproverComments;
                    new MailController().ApprovedRequestEmail(newUser).Deliver();
                }
                catch (Exception ex)
                {
                    Log4NetLogger logger2 = new Log4NetLogger();
                    logger2.Debug(ex.Source + "-" + ex.Message + "-" + ex.InnerException);
                }

                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Requested date changes have been made and leave request notification has been sent.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(alrs);

        }

        // GET: /ALRS/Unsuccessful/5
        [SharePointContextFilter]
        public ActionResult Unsuccessful(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ALRS alrs = db.ALRS.Find(id);
            if (alrs == null)
            {
                return HttpNotFound();
            }
            return View(alrs);
        }

        // POST: /ALRS/Unsuccessful/5
        [HttpPost, ActionName("Unsuccessful")]
        [ValidateAntiForgeryToken]
        public ActionResult UnsuccessfulConfirmed([Bind(Include = "ID,ALRSName,ALRSStartDate,ALRSEndDate,ALRSDuration,ALRSComments,ALRSJudgeOnLeave,ALRSStatus,ALRSApproverComments,ALRSApproverResponseDate,ALRSRequestorPosition,ALRSDisclaimer,DocumentDateCreated,DocumentCreatedBy,DocumentDateModified,DocumentModifiedBy,ALRSBackColor,ALRSAllDayEvent, DocumentDeleted,ALRSLeaveType,FromEmailAddress,ToEmailAddress")] ALRS alrs)
        {

            if (ModelState.IsValid)
            {
                alrs.ALRSStatus = "Unsuccessful";
                alrs.ALRSBackColor = "indianred";
                alrs.ALRSAllDayEvent = "true";
                alrs.ALRSLeaveType = "Additional Leave";
                alrs.DocumentDateModified = DateTime.Now;
                alrs.DocumentModifiedBy = User.Identity.Name;
                alrs.ALRSApproverResponseDate = DateTime.Now;
                db.Entry(alrs).State = EntityState.Modified;
                db.SaveChanges();

                //Send Unsuccessful Email
                try
                {
                    User newUser = new User();
                    newUser.ALRSId = alrs.ID;
                    newUser.Subject = "Leave Request has been unsuccessful";
                    newUser.ALRSLink = @Url.AbsoluteAction("Details", "ALRS", new { id = alrs.ID });

                    if (alrs.ALRSRequestorPosition == "Associates")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.AssociateApprover, x.AssociateApproverEmail });
                        newUser.FromEmail = toap.First().AssociateApproverEmail;
                        newUser.FromName = toap.First().AssociateApprover;
                        alrs.ALRSRequestorPosition = "Associates";
                    }
                    if (alrs.ALRSRequestorPosition == "Tip Staff")
                    {
                        var toap = db.tblApprovalList
                        .Select(x => new { x.TipStaffApprover, x.TipStaffApproverEmail });
                        newUser.FromEmail = toap.First().TipStaffApproverEmail;
                        newUser.FromName = toap.First().TipStaffApprover;
                        alrs.ALRSRequestorPosition = "Tipstaff";
                    }

                    newUser.ToEmail = GetUserEmailAddress(alrs.ALRSName); //TODO + Add alert;
                    newUser.ToName = alrs.ALRSName;
                    newUser.ALRSStartDate = alrs.ALRSStartDate;
                    newUser.ALRSEndDate = alrs.ALRSEndDate;
                    newUser.ApproverComments = alrs.ALRSApproverComments;
                    new MailController().UnsuccessfulRequestEmail(newUser).Deliver();
                }
                catch (Exception ex)
                {
                    Log4NetLogger logger2 = new Log4NetLogger();
                    logger2.Debug(ex.Source + "-" + ex.Message + "-" + ex.InnerException);
                }
                return RedirectToAction("Alert", "ALRS", new { alertMessage = "Unsuccessful leave request notification has been sent.", SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(alrs);

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
