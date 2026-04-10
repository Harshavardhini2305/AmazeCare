//Booking flow
//Status handling
//Reschedule


using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL APPOINTMENTS
        public IActionResult GetAll()
        {
            return Json(_context.Appointments.ToList());
        }

        // GET BY PATIENT
        public IActionResult GetByPatient(int patientId)
        {
            var list = _context.Appointments
                .Where(x => x.PatientId == patientId)
                .ToList();

            return Json(list);
        }

        // GET BY DOCTOR
        public IActionResult GetByDoctor(int doctorId)
        {
            var list = _context.Appointments
                .Where(x => x.DoctorId == doctorId)
                .ToList();

            return Json(list);
        }
    }

}