//using AmazeCare.Models;
//using AmazeCare.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace AmazeCare.Controllers
//{
//    public class PrescriptionController : Controller
//    {
//        private readonly AppDbContext _context;

//        public PrescriptionController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // ADD PRESCRIPTION

//        public IActionResult Add(Prescription p)
//        {
//            // Validate appointment exists
//            var a = _context.Appointments.Find(p.AppointmentId);
//            if (a == null) return Content("Invalid Appointment");

//            // Add prescription
//            _context.Prescriptions.Add(p);
//            _context.SaveChanges();

//            // Return success message
//            return Content("Prescription Added");
//        }

//        // GET ALL
//        public IActionResult GetAll()
//        // Return all prescriptions from the database
//        {
//            return Json(_context.Prescriptions.ToList());
//        }

//        // GET BY APPOINTMENT
//        public IActionResult GetByAppointment(int appointmentId)
//        {
//            // Return prescriptions for a specific appointment based on the provided appointment ID.
//            var list = _context.Prescriptions
//                .Where(x => x.AppointmentId == appointmentId)
//                .ToList();

//            return Json(list);
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
    public class PrescriptionController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionController(AppDbContext context)
        {
            _context = context;
        }

        // ADD PRESCRIPTION
        [HttpPost]   
        public IActionResult Add([FromBody] Prescription p)
        {
            // Validate appointment exists
            var a = _context.Appointments.Find(p.AppointmentId);
            if (a == null)
                return BadRequest("Invalid Appointment");

            // Add prescription
            _context.Prescriptions.Add(p);
            _context.SaveChanges();

            // Return success message
            return Ok("Prescription Added");
        }

        // GET ALL
        [HttpGet]   
        public IActionResult GetAll()
        // Return all prescriptions from the database
        {
            return Ok(_context.Prescriptions.ToList());
        }

        // GET BY APPOINTMENT
        [HttpGet("appointment/{appointmentId}")]   
        public IActionResult GetByAppointment(int appointmentId)
        {
            // Return prescriptions for a specific appointment based on the provided appointment ID.
            var list = _context.Prescriptions
                .Where(x => x.AppointmentId == appointmentId)
                .ToList();

            return Ok(list);
        }
    }
}