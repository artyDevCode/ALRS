using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Models;
using System.DirectoryServices;
using System.Configuration;
using System.DirectoryServices.AccountManagement;

namespace ALRSSystem.Controllers
{
    public class ApprovalListController : Controller
    {
        private ALRSDB db = new ALRSDB();

        // GET: /ApprovalList/
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

            var count = (from result1
                in db.tblApprovalList
                    select result1).Count();

            if (count == 0)
            { 
                var appList = new ApprovalList();

                appList.AssociateApprover = "Please assign associate approver";
                appList.TipStaffApprover = "Please assign tipstaff approver";
                appList.HRNotification = "Please assign HR notification";

                db.tblApprovalList.Add(appList);
                db.SaveChanges();
            }

            return View(db.tblApprovalList.ToList());
        }

        public static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = ConfigurationManager.AppSettings["ADConnection"]; //"LDAP://dev.justice.vic.gov.au/CN=Users,DC=dev,DC=justice,DC=vic,DC=gov,DC=au"; 
            de.AuthenticationType = AuthenticationTypes.Secure;
            return de;
        }

        public List<string> UserEmails()
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
            return groupMembers; ;
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

        // GET: /ApprovalList/Details/5
        [SharePointContextFilter]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovalList approvallist = db.tblApprovalList.Find(id);
            if (approvallist == null)
            {
                return HttpNotFound();
            }
            return View(approvallist);
        }

        // GET: /ApprovalList/Create
        [SharePointContextFilter]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ApprovalList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,AssociateApprover,TipStaffApprover,HRNotification")] ApprovalList approvallist)
        {
            if (ModelState.IsValid)
            {
                approvallist.AssociateApprover = GetUserEmailAddress(approvallist.AssociateApprover);
                approvallist.TipStaffApprover = GetUserEmailAddress(approvallist.TipStaffApprover);
                db.tblApprovalList.Add(approvallist);
                db.SaveChanges();
                return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            return View(approvallist);
        }

        // GET: /ApprovalList/Edit/5
        [SharePointContextFilter]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovalList approvallist = db.tblApprovalList.Find(id);
            if (approvallist == null)
            {
                return HttpNotFound();
            }
            
           return View(approvallist);
        }

        // POST: /ApprovalList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,AssociateApprover,TipStaffApprover,HRNotification")] ApprovalList approvallist)
        {
            if (ModelState.IsValid)
            {
                approvallist.AssociateApproverEmail = GetUserEmailAddress(approvallist.AssociateApprover);
                approvallist.TipStaffApproverEmail = GetUserEmailAddress(approvallist.TipStaffApprover);
                db.Entry(approvallist).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(approvallist);
        }

        public JsonResult getApprover(string term)
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

        // GET: /ApprovalList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApprovalList approvallist = db.tblApprovalList.Find(id);
            if (approvallist == null)
            {
                return HttpNotFound();
            }
            return View(approvallist);
        }

        // POST: /ApprovalList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ApprovalList approvallist = db.tblApprovalList.Find(id);
            db.tblApprovalList.Remove(approvallist);
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
