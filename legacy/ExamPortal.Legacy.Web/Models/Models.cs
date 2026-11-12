using System;

namespace ExamPortal.Legacy.Web.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
    }

    public class Candidate
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public int OwnerUserId { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Serializable]
    public class UserPreferences
    {
        public string Theme { get; set; }
        public int PageSize { get; set; }
    }
}
