using System;

namespace kafi.Models
{
    public enum Role
    {
        Manager,
        Employee
    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
