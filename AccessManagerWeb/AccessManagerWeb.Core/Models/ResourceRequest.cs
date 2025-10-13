using System;

namespace AccessManagerWeb.Core.Models
{
    public class ResourceRequest
    {
        public int Id { get; set; }
        public string RequestorUsername { get; set; }
        public string ResourceName { get; set; }
        public string RequestReason { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
    }
}