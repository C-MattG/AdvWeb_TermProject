using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HighlandTechSolutions.Models
{
    public class AppointmentFormViewModel
    {
        public int Id { get; set; } //needed for edit

        [Required]
        public List<int> SelectedServiceIds { get; set; } = new();

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan Time { get; set; }

        public List<SelectListItem> AvailableServices { get; set; } = new();
        public List<SelectListItem> AvailableTimes { get; set; } = new();
    }
}


