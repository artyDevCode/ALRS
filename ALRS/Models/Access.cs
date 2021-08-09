using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ALRSSystem.Models
{
    public class Access
    {
        public int Id { get; set; }
        
        [Display(Name = "User Id")]
        [StringLength(500)]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter the user")]
        [StringLength(500)]
        [Display(Name = "Users")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter the access group")]
        [Display(Name = "Access Group")]
        [StringLength(100)]
        public string AccessGroup { get; set; }
    }

}