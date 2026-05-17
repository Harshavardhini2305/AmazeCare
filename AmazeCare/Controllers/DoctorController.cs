////Manage doctor data
////View appointments
////Add consultation



//using AmazeCare.Models;
//using AmazeCare.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace AmazeCare.Controllers
//{
//    public class DoctorController : Controller
//    {
//        private readonly AppDbContext _context;

//        public DoctorController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // LOGIN
//        //Match name +password
//        public IActionResult Login(string name, string password)
//        {
//            var doc = _context.Doctors
//                .FirstOrDefault(x => x.Name == name && x.Password == password);//login

//            if (doc == null) return Content("Invalid");

//            return Content("Login Success");
//        }

//        // VIEW APPOINTMENTS
//        //Show appointments for the doctor
//        public IActionResult MyAppointments(int doctorId)
//        {
//            var list = _context.Appointments
//                .Where(x => x.DoctorId == doctorId)//show appoimts for the doctor
//                .ToList();

//            return Json(list);
//        }

//        // ACCEPT / REJECT APPOINTMENT
//        //Update status of appointment
//        public IActionResult UpdateStatus(int id, string status)
//        {
//            var a = _context.Appointments.Find(id);
//            if (a == null) return Content("Not Found");

//            a.Status = status; // Accepted / Rejected
//            _context.SaveChanges();

//            return Content("Status Updated");
//        }

//        // ADD MEDICAL RECORD (CONSULTATION)
//        //Update appointment status to Completed
//        public IActionResult AddMedicalRecord(MedicalRecord r)
//        {

//            var a = _context.Appointments.Find(r.AppointmentId);//find appointment
//            if (a == null) return Content("Invalid Appointment");//if not found

//            a.Status = "Completed";

//            _context.MedicalRecords.Add(r);//add medical record
//            _context.SaveChanges();

//            return Content("Consultation Completed");//
//        }

//        // ADD PRESCRIPTION
//        //Add prescription for an appointment
//        public IActionResult AddPrescription(Prescription p)
//        {
//            //find appointment
//            var a = _context.Appointments.Find(p.AppointmentId);
//            if (a == null) return Content("Invalid Appointment");

//            _context.Prescriptions.Add(p);
//            _context.SaveChanges();

//            return Content("Prescription Added");
//        }
//    }
//}



//Manage doctor data
//View appointments
//Add consultation

using AmazeCare.Data;
using AmazeCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AmazeCare.Controllers
{ 

    [ApiController]   
    [Route("api/[controller]")]   
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }


        // REGISTER
        //Add new doctor to system
        [HttpPost("register")]
        public IActionResult Register([FromBody] Doctor d)
        {
            if (_context.Doctors.Any(x => x.Phone == d.Phone))   
                return BadRequest("Doctor already exists");

            _context.Doctors.Add(d);
            _context.SaveChanges();

            return Ok("Doctor Registered Successfully");
        }

        // LOGIN
        //Match name +password
        [HttpPost("login")]   
        public IActionResult Login(string name, string password)
        {
            var doc = _context.Doctors
                .FirstOrDefault(x => x.Name == name && x.Password == password);//login

            if (doc == null)
                return BadRequest("Invalid");

            return Ok("Login Success");
        }

        // VIEW APPOINTMENTS
        //Show appointments for the doctor

        [HttpGet("appointments/{doctorId}")]
        public IActionResult MyAppointments(int doctorId)
        {
            var list = _context.Appointments
                .Include(x => x.Patient) 
                .Where(x => x.DoctorId == doctorId)
                .Select(x => new
                {
                    x.AppointmentId,
                    PatientName = x.Patient.Name,
                    Phone = x.Patient.Phone,
                    x.Symptoms,
                    x.VisitType,
                    x.AppointmentDate,
                    x.Status
                })
                .ToList();

            return Ok(list);
        }

        // ACCEPT / REJECT APPOINTMENT
        //Update status of appointment
        [HttpPut("status/{id}")]   
        public IActionResult UpdateStatus(int id, string status)
        {
            var a = _context.Appointments.Find(id);
            if (a == null)
                return NotFound("Not Found");

            a.Status = status; // Accepted / Rejected
            _context.SaveChanges();

            return Ok("Status Updated");
        }

        // ADD MEDICAL RECORD (CONSULTATION)
        //Update appointment status to Completed
        [HttpPost("medicalrecord")]   
        public IActionResult AddMedicalRecord([FromBody] MedicalRecord r)
        {
            var a = _context.Appointments.Find(r.AppointmentId);//find appointment
            if (a == null)
                return BadRequest("Invalid Appointment");//if not found

            a.Status = "Completed";

            _context.MedicalRecords.Add(r);//add medical record
            _context.SaveChanges();

            return Ok("Consultation Completed");
        }

        // ADD PRESCRIPTION
        //Add prescription for an appointment
        [HttpPost("prescription")]   
        public IActionResult AddPrescription([FromBody] Prescription p)
        {
            //find appointment
            var a = _context.Appointments.Find(p.AppointmentId);
            if (a == null)
                return BadRequest("Invalid Appointment");

            _context.Prescriptions.Add(p);
            _context.SaveChanges();

            return Ok("Prescription Added");
        }
    }
}