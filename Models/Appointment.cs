using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HighlandTechSolutions.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Status { get; set; }

        public bool IsDeleted { get; set; }  // default = false, meaning it's not deleted

        public ApplicationUser User { get; set; }
        public ICollection<AppointmentService> AppointmentServices { get; set; }
    }


    public class AppointmentService
    {
        public int AppointmentId { get; set; }
        public Appointment Appointment { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}


