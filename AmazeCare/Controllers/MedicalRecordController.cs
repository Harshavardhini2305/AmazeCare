//using AmazeCare.Models;
//using AmazeCare.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace AmazeCare.Controllers
//{
//    public class MedicalRecordController : Controller
//    {
//        private readonly AppDbContext _context;

//        public MedicalRecordController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // ADD RECORD
//        //Accept a MedicalRecord , add to DB
//        public IActionResult Add(MedicalRecord r)
//        {
//            _context.MedicalRecords.Add(r);
//            _context.SaveChanges();

//            return Content("Medical Record Added");
//        }

//        // GET ALL
//        // Return all medical records 
//        public IActionResult GetAll()
//        {
//            return Json(_context.MedicalRecords.ToList());
//        }

//        // GET BY APPOINTMENT
//        // Return medical records for a specific appointment
//        public IActionResult GetByAppointment(int appointmentId)
//        {
//            var list = _context.MedicalRecords
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
    public class MedicalRecordController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalRecordController(AppDbContext context)
        {
            _context = context;
        }

        // ADD RECORD
        //Accept a MedicalRecord , add to DB
        [HttpPost]   
        public IActionResult Add([FromBody] MedicalRecord r)
        {
            // Optional validation (recommended)
            var appointment = _context.Appointments.Find(r.AppointmentId);
            if (appointment == null)
                return BadRequest("Invalid Appointment");

            _context.MedicalRecords.Add(r);
            _context.SaveChanges();

            return Ok("Medical Record Added");
        }

        // GET ALL
        // Return all medical records 
        [HttpGet]   
        public IActionResult GetAll()
        {
            return Ok(_context.MedicalRecords.ToList());
        }

        // GET BY APPOINTMENT
        // Return medical records for a specific appointment
        [HttpGet("appointment/{appointmentId}")]   
        public IActionResult GetByAppointment(int appointmentId)
        {
            var list = _context.MedicalRecords
                .Where(x => x.AppointmentId == appointmentId)
                .ToList();

            return Ok(list);
        }
    }
}