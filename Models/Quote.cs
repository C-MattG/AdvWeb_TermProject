using System;
using System.ComponentModel.DataAnnotations;

namespace HighlandTechSolutions.Models
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public string Details { get; set; }

        public DateTime DateRequested { get; set; }

        public string Status { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
    }
}


