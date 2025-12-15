using System.Collections.Generic;

namespace SecondHandPlatform.ViewModels
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public bool IsLockedOut { get; set; } 
    }

    public class UserDetailsViewModel
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public List<string> Roles { get; set; }
        public List<Models.Product> Products { get; set; }
        public bool IsLockedOut { get; set; }
    }
}