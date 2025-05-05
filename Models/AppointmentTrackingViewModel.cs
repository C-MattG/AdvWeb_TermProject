using System;
using System.Collections.Generic;

namespace HighlandTechSolutions.Models
{
    public class AppointmentTrackingViewModel
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Status { get; set; }
        public List<string> Services { get; set; }
    }
}