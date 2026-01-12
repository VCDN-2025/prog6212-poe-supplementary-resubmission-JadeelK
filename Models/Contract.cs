using System;

namespace CMCS.Models
{
    public class Contract
    {
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public decimal HourlyRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}