using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HighlandTechSolutions.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string ContactEmail { get; set; }

        public ICollection<Quote> Quotes { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public Review Review { get; set; }
    }

}

