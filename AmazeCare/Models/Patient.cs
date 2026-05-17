
// Stores patient data(Name, DOB, Phone, Password)
// Represents Patient table in database
// Used to send/receive data in API
// Phone + Password used for login
// All fields saved during registration




namespace AmazeCare.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string? Name { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }


        public string? Address { get; set; }

    }
}
