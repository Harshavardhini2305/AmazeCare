using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly AppDbContext _context;

        public PrescriptionController(AppDbContext context)
        {
            _context = context;
        }

        // ADD PRESCRIPTION
        public IActionResult Add(Prescription p)
        {
            var a = _context.Appointments.Find(p.AppointmentId);
            if (a == null) return Content("Invalid Appointment");

            _context.Prescriptions.Add(p);
            _context.SaveChanges();

            return Content("Prescription Added");
        }

        // GET ALL
        public IActionResult GetAll()
        {
            return Json(_context.Prescriptions.ToList());
        }

        // GET BY APPOINTMENT
        public IActionResult GetByAppointment(int appointmentId)
        {
            var list = _context.Prescriptions
                .Where(x => x.AppointmentId == appointmentId)
                .ToList();

            return Json(list);
        }
    }
}