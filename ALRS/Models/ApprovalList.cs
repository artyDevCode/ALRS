using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ALRSSystem.Models
{
    public class ApprovalList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(150)]
        [Required(ErrorMessage = "Please enter the Associate Approver")]
        [Display(Name = "Associate Approver")]
        public string AssociateApprover { get; set; }
        
        [StringLength(150)]
        [Display(Name = "Associate Approver Email")]
        public string AssociateApproverEmail { get; set; }
        
        [StringLength(150)]
        [Required(ErrorMessage = "Please enter the Tipstaff Approver")]
        [Display(Name = "Tipstaff Approver")]
        public string TipStaffApprover { get; set; }

        [StringLength(150)]
        [Display(Name = "Tipstaff Approver Email")]
        public string TipStaffApproverEmail { get; set; }

        [StringLength(150)]
        [Display(Name = "HR Notification")]
        public string HRNotification { get; set; }

        [StringLength(150)]
        [Display(Name = "HR Notification Email")]
        public string HRNotificationEmail { get; set; }

    }
}