using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ALRSSystem.Models
{
    public class ALRSExtract
    {
        [Key]
        public int ID { get; set; }

        public string ALRSName { get; set; }

        public string ALRSStatus { get; set; }

        public string ALRSLeaveType { get; set; }

        public string ALRSStartDate { get; set; }

        public DateTime ALRSEndDate { get; set; }

        public string ALRSDuration { get; set; }
    }
}