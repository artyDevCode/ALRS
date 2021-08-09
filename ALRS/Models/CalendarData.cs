using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ALRSSystem.Models
{
    public class CalendarData
    {
        [Key]
        public int Id { get; set; } 
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Resource { get; set; }
        public string Color { get; set; }
        public bool Allday { get; set; }
        public string Text { get; set; }
    }
}