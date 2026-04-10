//Stores admin login data
// Only Username + Password
// Used for authentication
// No extra fields needed





namespace AmazeCare.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}

