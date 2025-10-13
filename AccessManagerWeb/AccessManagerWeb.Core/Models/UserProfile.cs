using System;

namespace AccessManagerWeb.Core.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public bool HasAccess { get; set; }
        public DateTime? AccessGrantedDate { get; set; }
        public string AccessGrantedBy { get; set; }
    }
}