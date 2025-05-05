using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HighlandTechSolutions.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime DateSubmitted { get; set; }

        public ApplicationUser User { get; set; }
    }
}



