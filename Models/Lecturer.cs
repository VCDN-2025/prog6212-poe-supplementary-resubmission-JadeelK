using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Lecturer
    {
        public int LecturerId { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Contract")]
        public int? ContractId { get; set; }
    }
}