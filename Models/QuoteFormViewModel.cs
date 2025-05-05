using System.ComponentModel.DataAnnotations;

namespace HighlandTechSolutions.Models
{
    public class QuoteFormViewModel
    {
        [Required]
        public string Details { get; set; }
    }
}

