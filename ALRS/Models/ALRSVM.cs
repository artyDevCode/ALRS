using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALRSSystem.Models
{
    public class ALRSVM
    {
        public int VM_Id { get; set; }
        public string VM_Name { get; set; }
        public string VM_Status { get; set; }
        public string VM_LeaveType { get; set; }
        public DateTime VM_StartDate { get; set; }
        public DateTime VM_EndDate { get; set; }
        public int VM_Duration { get; set; }
    }
    public class ALRSLeaveTotalsVM
    {
        public int VM_Id { get; set; }
        public int VM_Year { get; set; }
        public string VM_Status { get; set; }
        public string VM_Name { get; set; }
        public string VM_LeaveType { get; set; }
        public DateTime VM_StartDate { get; set; }
        public DateTime VM_EndDate { get; set; }
        public int VM_Duration { get; set; }
    }
}