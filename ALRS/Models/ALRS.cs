using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ALRSSystem.Helpers;
using System.ComponentModel;

namespace ALRSSystem.Models
{
    public class PagedALRSModel
    {
        public int TotalRows { get; set; }
        public IEnumerable<ALRS> ALRS { get; set; }
        public int PageSize { get; set; }
    }
    
    public class ALRS
    {
        public IEnumerable<SelectListItem> JudgeOnLeaveList { get; set; }
        
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(100)]
        [Display(Name = "Name")]
        public string ALRSName { get; set; }

        [Required(ErrorMessage = "Please enter the start date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime ALRSStartDate { get; set; }

        [Required(ErrorMessage = "Please enter the end date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime ALRSEndDate { get; set; }

        [Display(Name = "Duration")]
        public int ALRSDuration { get; set; }

        [StringLength(1000)]
        [Display(Name = "Comments")]
        public string ALRSComments { get; set; }

        [Required(ErrorMessage = "You must indicate if your judge is on leave at the same time.")]
        [Display(Name = "JudgeOnLeave")]
        public bool ALRSJudgeOnLeave { get; set; }

        [Required(ErrorMessage = "Requestor's position must be completed")]
        [Display(Name = "Requestor Position")]
        public string ALRSRequestorPosition { get; set; }

        [BooleanRequired(ErrorMessage = "You must agree with the disclaimer to proceed.")]
        [Display(Name = "Disclaimer")]
        public bool ALRSDisclaimer { get; set; }

        [StringLength(20)]
        [Display(Name = "Status")]
        public string ALRSStatus { get; set; }

        //[Required(ErrorMessage = "You must enter an approval date")]
        [Display(Name = "Response Date")]
        public DateTime? ALRSApproverResponseDate { get; set; }

        [StringLength(1000)]
        [Display(Name = "Approver Comments")]
        //[Required(ErrorMessage = "You must enter approval comments")]
        public string ALRSApproverComments { get; set; }

        [StringLength(50)]
        [Display(Name = "Leave Type")]
        public string ALRSLeaveType { get; set; }

        [DefaultValue("true")]
        public string ALRSAllDayEvent { get; set; }
        public string ALRSBackColor { get; set; }    
            
        [Display(Name = "Date Modified")]
        public DateTime DocumentDateModified { get; set; }

        [Display(Name = "Modified By")]
        public string DocumentModifiedBy { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DocumentDateCreated { get; set; }

        [Display(Name = "Created By")]
        public string DocumentCreatedBy { get; set; }
        public bool DocumentDeleted { get; set; }
        //public string FromEmailAddress { get; set; }
        public string ToEmailAddress { get; set; }
    }
}