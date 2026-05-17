//using AmazeCare.Models;
//using AmazeCare.Data;
//using Microsoft.AspNetCore.Mvc;
////using System.Linq;

//namespace AmazeCare.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly AppDbContext _context;

//        public AdminController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // LOGIN
//        //Match username +password
//        public IActionResult Login(string username, string password)
//        {
//            var admin = _context.Admins
//                .FirstOrDefault(x => x.Username == username && x.Password == password);

//            if (admin == null) return Content("Invalid");

//            return Content("Login Success");
//        }

//        // ADD DOCTOR
//        //Accept a Doctor object, add to DB, save changes
//        public IActionResult AddDoctor(Doctor d)
//        {
//            _context.Doctors.Add(d);
//            _context.SaveChanges();

//            return Content("Doctor Added");
//        }

//        // UPDATE DOCTOR

//        public IActionResult UpdateDoctor(Doctor d)
//        {
//            //Find doctor by ID, if not found return Not Found
//            var doc = _context.Doctors.Find(d.DoctorId);
//            if (doc == null) return Content("Not Found");

//            //Update fields and save changes
//            doc.Name = d.Name;
//            doc.Specialization = d.Specialization;
//            doc.Experience = d.Experience;

//            _context.SaveChanges();

//            return Content("Updated");
//        }

//        // DELETE DOCTOR
//        //Find doctor by ID, if found remove and save changes
//        public IActionResult DeleteDoctor(int id)
//        {
//            var d = _context.Doctors.Find(id);
//            if (d != null)
//            {
//                _context.Doctors.Remove(d);
//                _context.SaveChanges();
//            }

//            return Content("Deleted");
//        }

//        // VIEW ALL APPOINTMENTS
//        //Fetch all appointments from DB
//        public IActionResult AllAppointments()
//        {
//            return Json(_context.Appointments.ToList());
//        }

//        // REPORT
//        //Return counts of patients, doctors, and appointment
//        public IActionResult Report()
//        {
//            var report = new
//            {
//                Patients = _context.Patients.Count(),
//                Doctors = _context.Doctors.Count(),
//                Appointments = _context.Appointments.Count()
//            };

//            return Json(report);
//        }
//    }
//}







using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{
    [ApiController]  
    [Route("api/[controller]")]   
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN
        //Match username +password
        [HttpPost("login")]   
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins
                .FirstOrDefault(x => x.Username == username && x.Password == password);

            if (admin == null)
                return BadRequest("Invalid");

            return Ok("Login Success");
        }

        // ADD DOCTOR
        //Accept a Doctor object, add to DB, save changes
        [HttpPost("adddoctor")]  
        public IActionResult AddDoctor([FromBody] Doctor d)
        {
            _context.Doctors.Add(d);
            _context.SaveChanges();

            return Ok("Doctor Added");
        }

        // UPDATE DOCTOR

        [HttpPut("updatedoctor")]  
        public IActionResult UpdateDoctor([FromBody] Doctor d)
        {
            //Find doctor by ID, if not found return Not Found
            var doc = _context.Doctors.Find(d.DoctorId);
            if (doc == null)
                return NotFound("Not Found");

            //Update fields and save changes
            doc.Name = d.Name;
            doc.Specialization = d.Specialization;
            doc.Experience = d.Experience;

            _context.SaveChanges();

            return Ok("Updated");
        }

        // DELETE DOCTOR
        //Find doctor by ID, if found remove and save changes
        [HttpDelete("deletedoctor/{id}")]   
        public IActionResult DeleteDoctor(int id)
        {
            var d = _context.Doctors.Find(id);
            if (d == null)
                return NotFound("Not Found");

            _context.Doctors.Remove(d);
            _context.SaveChanges();

            return Ok("Deleted");
        }

        // VIEW ALL APPOINTMENTS
        //Fetch all appointments from DB
        [HttpGet("appointments")]   
        public IActionResult AllAppointments()
        {
            return Ok(_context.Appointments.ToList());
        }

        // REPORT
        //Return counts of patients, doctors, and appointment
        [HttpGet("report")]   
        public IActionResult Report()
        {
            var report = new
            {
                Patients = _context.Patients.Count(),
                Doctors = _context.Doctors.Count(),
                Appointments = _context.Appointments.Count()
            };

            return Ok(report);
        }
    }
}

