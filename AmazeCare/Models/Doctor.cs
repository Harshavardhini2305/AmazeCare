// Stores patient data(Name, DOB, Phone, Password)
// Represents Patient table in database
// Used to send/receive data in API
// Phone + Password used for login
// All fields saved during registration





namespace AmazeCare.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string? Name { get; set; }
        public string? Specialization { get; set; }
        public int Experience { get; set; }
        public string? Qualification { get; set; }
        public string? Designation { get; set; }

        // Login
        public string? Password { get; set; }
    }
}
