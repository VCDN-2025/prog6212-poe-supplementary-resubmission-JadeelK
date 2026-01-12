using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public int LecturerId { get; set; }

        [Display(Name = "Lecturer")]
        public string? LecturerName { get; set; }  // Made nullable

        [Required(ErrorMessage = "Please select a month")]
        [Display(Name = "Period Month")]
        public string? PeriodMonth { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(0.1, 744, ErrorMessage = "Hours must be between 0.1 and 744")]
        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0.01, 10000, ErrorMessage = "Hourly rate must be between 0.01 and 10000")]
        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Status")]
        public string? Status { get; set; }  // Made nullable

        [Display(Name = "Submitted At")]
        public DateTime SubmittedAt { get; set; }

        [Display(Name = "Additional Notes")]
        public string? Notes { get; set; }

        public List<SupportingDocument> SupportingDocuments { get; set; } = new List<SupportingDocument>();
        public List<Approval> Approvals { get; set; } = new List<Approval>();

        public void CalculateTotalAmount()
        {
            TotalAmount = HoursWorked * HourlyRate;
        }
    }
}