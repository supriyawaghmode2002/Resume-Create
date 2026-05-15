using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeManager.Models
{
    public class Applicant
    {
        [Key]
        public int? Id { get; set; }

        [Required, StringLength(100)]
        public string?Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string? Gender { get; set; } = string.Empty;

        [Required]
        public int?Age { get; set; }

        [Required, StringLength(100)]
        public string?Qulification { get; set; } = string.Empty;

        [Required]
        public int? TotalExperience { get; set; }

        public string? PhotoUrl { get; set; }

        
        public List<Experience> Experiences { get; set; } = new List<Experience>();
    }
}
