using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALRSSystem.Models
{
    public class User
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public int ALRSId { get; set; }
        public string ALRSLink { get; set; }
        public DateTime ALRSStartDate { get; set; }
        public DateTime ALRSEndDate { get; set; }
        public string ApproverComments { get; set; }
    }
    public class UserRole
    {
        //public string UserName { get; set; }
        public string RoleName { get; set; }
    }

}