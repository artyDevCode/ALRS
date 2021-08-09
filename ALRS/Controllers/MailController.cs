using ActionMailer.Net.Mvc;
using ActionMailer.Net;
using ALRSSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALRSSystem.Controllers
{
    public class MailController : MailerBase
    {
        //New Leave Request Mail
        public EmailResult LeaveRequestEmail(User model)
        {
            To.Add(model.ToEmail);
            From = model.FromEmail;
            Subject = model.Subject;
            return Email("LeaveRequestEmail", model);
        }

        //Unsuccessful Leave Request Mail
        public EmailResult UnsuccessfulRequestEmail(User model)
        {
            To.Add(model.ToEmail);
            From = model.FromEmail; 
            Subject = model.Subject;
            return Email("UnsuccessfulLeaveRequestEmail", model);
        }

        //Unsuccessful Leave Request Mail
        public EmailResult ApprovedRequestEmail(User model)
        {
            To.Add(model.ToEmail);
            From = model.FromEmail;
            Subject = model.Subject;
            return Email("ApprovedLeaveRequestEmail", model);
        }

    }
}