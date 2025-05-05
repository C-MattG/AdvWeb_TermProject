using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HighlandTechSolutions.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Range(0, 10000)]
        public decimal Price { get; set; }

        // Navigation
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}

