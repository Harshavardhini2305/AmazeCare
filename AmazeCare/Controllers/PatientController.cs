

////Patient CRUD
////View appointments
////Book / cancel / reschedule



//using AmazeCare.Models;
//using AmazeCare.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace AmazeCare.Controllers
//{

//    public class PatientController : Controller
//    {
//        private readonly AppDbContext _context;

//        public PatientController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // REGISTER
//        //Check if phone already exists
//        //If exists return Patient already exists
//        //If not → save patient and return Registered Successfully
//        public IActionResult Register(Patient p)
//        {
//            if (_context.Patients.Any(x => x.Phone == p.Phone))
//                return Content("Patient already exists");

//            _context.Patients.Add(p);
//            _context.SaveChanges();
//            return new ContentResult { Content = "Registered Successfully" };
//        }

//        // LOGIN
//        //Match phone +password
//        //If found → login success
//        public IActionResult Login(string phone, string password)
//        {
//            var user = _context.Patients
//                .FirstOrDefault(x => x.Phone == phone && x.Password == password);

//            if (user == null) return Content("Invalid Credentials");

//            return new ContentResult { Content = "Login Success" };
//        }

//        // UPDATE PROFILE
//        //find patient by id
//        //update fields
//        //save changes
//        public IActionResult Update(Patient p)
//        {
//            var existing = _context.Patients.Find(p.PatientId);//find paient by id
//            if (existing == null) return Content("Not Found");

//            existing.Name = p.Name;//update fields
//            existing.Phone = p.Phone;
//            existing.Gender = p.Gender;
//            existing.Address = p.Address;

//            _context.SaveChanges();//save changes
//            return new ContentResult { Content = "Updated Successfully" };
//        }

//        // BOOK APPOINTMENT
//        //check if doctor is available at that time
//        //if available → book appointment
//        //if not → return doctor not available

//        public IActionResult BookAppointment(Appointment a)
//        {
//            // Here we are checking if there is any existing appointment
//            // for the same doctor at the same date and time.
//            var exists = _context.Appointments.Any(x =>
//                x.DoctorId == a.DoctorId &&
//                x.AppointmentDate == a.AppointmentDate); // ✅ SIMPLE

//            if (exists)
//                return new ContentResult { Content = "Doctor not available" };

//            a.Status = "Booked";

//            _context.Appointments.Add(a);
//            _context.SaveChanges();

//            return new ContentResult { Content = "Appointment Booked" };
//        }

//        // VIEW APPOINTMENTS

//        public IActionResult MyAppointments(int patientId)
//        {
//            // Here we are fetching all appointments for the given patient ID.
//            var list = _context.Appointments
//                .Where(x => x.PatientId == patientId)//show only this patients appoinment
//                .ToList();

//            return Json(list);
//        }

//        // CANCEL APPOINTMENT

//        public IActionResult CancelAppointment(int id)
//        {
//            // Here we are finding the appointment by its ID.
//            var a = _context.Appointments.Find(id);
//            if (a == null) return Content("Not Found");//


//            a.Status = "Cancelled";//here we will not delete directly but update status
//            _context.SaveChanges();


//            return new ContentResult { Content = "Cancelled Successfully" };
//        }

//        // MEDICAL HISTORY
//        //get all medical records of patient
//        public IActionResult MedicalHistory(int patientId)
//        {
//            var records = _context.MedicalRecords
//                .Where(x => x.Appointment.PatientId == patientId)//get all medicla records of patient
//                .ToList();

//            return Json(records);
//        }
//    }
//}










































using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AmazeCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientController : Controller
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }



        // REGISTER
        //Check if phone already exists
        //If exists return Patient already exists
        //If not → save patient and return Registered Successfully
        [HttpPost("register")]
        public IActionResult Register([FromBody] Patient p)
        {
            if (_context.Patients.Any(x => x.Phone == p.Phone))
                return BadRequest("Patient already exists");

            _context.Patients.Add(p);
            _context.SaveChanges();

            return Ok("Registered Successfully");
        }

        // LOGIN
        //Match phone +password
        //If found → login success
        [HttpPost("login")]
        public IActionResult Login(string phone, string password)
        {
            var user = _context.Patients
                .FirstOrDefault(x => x.Phone == phone && x.Password == password);

            if (user == null)
                return BadRequest("Invalid Credentials");

            return Ok("Login Success");
        }

        // UPDATE PROFILE
        //find patient by id
        //update fields
        //save changes
        [HttpPut("update")]
        public IActionResult Update([FromBody] Patient p)
        {
            var existing = _context.Patients.Find(p.PatientId);
            if (existing == null)
                return NotFound("Not Found");

            existing.Name = p.Name;
            existing.Phone = p.Phone;
            existing.Gender = p.Gender;
            existing.Address = p.Address;

            _context.SaveChanges();

            return Ok("Updated Successfully");
        }

        // VIEW DOCTORS and their details
        [HttpGet("doctors")]
        public IActionResult GetDoctors()
        {
            var doctors = _context.Doctors
                .Select(d => new
                {
                    d.DoctorId,
                    d.Name,
                    d.Specialization,
                    d.Experience,
                    d.Designation
                })
                .ToList();

            return Ok(doctors);
        }


        //search doctor by specialization

        [HttpGet("search")]
        public IActionResult SearchDoctor(string specialization)
        {
            var doctors = _context.Doctors
                .Where(d => d.Specialization != null &&
                            d.Specialization.ToLower().Contains(specialization.ToLower()))
                .Select(d => new
                {
                    d.DoctorId,
                    d.Name,
                    d.Specialization,
                    d.Experience
                })
                .ToList();

            return Ok(doctors);
        }

        // BOOK APPOINTMENT
        //check if doctor is available at that time
        //if available → book appointment
        //if not → return doctor not available
        [HttpPost("book")]
        public IActionResult BookAppointment([FromBody] Appointment a)
        {
            var exists = _context.Appointments.Any(x =>
                x.DoctorId == a.DoctorId &&
                x.AppointmentDate == a.AppointmentDate);

            if (exists)
                return BadRequest("Doctor not available");

            a.Status = "Booked";

            _context.Appointments.Add(a);
            _context.SaveChanges();

            return Ok("Appointment Booked");
        }

        

        // VIEW APPOINTMENTS
        [HttpGet("appointments/{patientId}")]  
        public IActionResult MyAppointments(int patientId)
        {
            var list = _context.Appointments
                .Where(x => x.PatientId == patientId)
                .ToList();

            return Ok(list);
        }

        // CANCEL APPOINTMENT
        [HttpPut("cancel/{id}")]   
        public IActionResult CancelAppointment(int id)
        {
            var a = _context.Appointments.Find(id);
            if (a == null)
                return NotFound("Not Found");

            a.Status = "Cancelled";
            _context.SaveChanges();

            return Ok("Cancelled Successfully");
        }

        // MEDICAL HISTORY
        [HttpGet("medicalhistory/{patientId}")]   
        public IActionResult MedicalHistory(int patientId)
        {
            var records = _context.MedicalRecords
                .Include(x=>x.Appointment)
                .Where(x => x.Appointment.PatientId == patientId)
                .ToList();

            return Ok(records);
        }
    }
}