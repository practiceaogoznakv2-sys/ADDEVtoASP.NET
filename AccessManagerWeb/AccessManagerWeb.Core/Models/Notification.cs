using System;

namespace AccessManagerWeb.Core.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RelatedRequestId { get; set; }
    }
}