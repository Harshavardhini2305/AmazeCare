////Booking flow
////Status handling
////Reschedule


//using AmazeCare.Models;
//using AmazeCare.Data;
//using Microsoft.AspNetCore.Mvc;
//using System.Linq;

//namespace AmazeCare.Controllers
//{
//    public class AppointmentController : Controller
//    {
//        private readonly AppDbContext _context;

//        public AppointmentController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET ALL APPOINTMENTS
//        //This method retrieves all appointments from the database 
//        public IActionResult GetAll()
//        {
//            return Json(_context.Appointments.ToList());
//        }

//        // GET BY PATIENT
//        //This method retrieves appointments for a specific patient based on the provided patient ID.
//        public IActionResult GetByPatient(int patientId)
//        {
//            var list = _context.Appointments
//                .Where(x => x.PatientId == patientId)
//                .ToList();

//            return Json(list);
//        }

//        // GET BY DOCTOR

//        public IActionResult GetByDoctor(int doctorId)
//        {
//            var list = _context.Appointments
//                .Where(x => x.DoctorId == doctorId)
//                .ToList();

//            return Json(list);
//        }
//    }

//}


//Booking flow
//Status handling
//Reschedule

using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{
    [ApiController]   
    [Route("api/[controller]")]   
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL APPOINTMENTS
        //This method retrieves all appointments from the database 
        [HttpGet]   
        public IActionResult GetAll()
        {
            return Ok(_context.Appointments.ToList());
        }

        // GET BY PATIENT
        //This method retrieves appointments for a specific patient based on the provided patient ID.
        [HttpGet("patient/{patientId}")]   
        public IActionResult GetByPatient(int patientId)
        {
            var list = _context.Appointments
                .Where(x => x.PatientId == patientId)
                .ToList();

            return Ok(list);
        }

        // GET BY DOCTOR
        [HttpGet("doctor/{doctorId}")]   
        public IActionResult GetByDoctor(int doctorId)
        {
            var list = _context.Appointments
                .Where(x => x.DoctorId == doctorId)
                .ToList();

            return Ok(list);
        }
    }
}