using System;

namespace CMCS.Models
{
    public class Approval
    {
        public int ApprovalId { get; set; }
        public int ClaimId { get; set; }
        public string ApproverRole { get; set; }
        public string ApproverName { get; set; }
        public string Decision { get; set; }
        public string Comments { get; set; }
        public DateTime ApprovedAt { get; set; }
    }
}