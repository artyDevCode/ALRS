using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace ALRSSystem.Models
{
    public class RequestorPosition
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [StringLength(100)]
        [Display(Name = "Requestor Position")]
        public string RequestorPositionName { get; set; }
    }
}