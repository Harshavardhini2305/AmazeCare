using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
//using System.Linq;

namespace AmazeCare.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins
                .FirstOrDefault(x => x.Username == username && x.Password == password);

            if (admin == null) return Content("Invalid");

            return Content("Login Success");
        }

        // ADD DOCTOR
        public IActionResult AddDoctor(Doctor d)
        {
            _context.Doctors.Add(d);
            _context.SaveChanges();

            return Content("Doctor Added");
        }

        // UPDATE DOCTOR
        public IActionResult UpdateDoctor(Doctor d)
        {
            var doc = _context.Doctors.Find(d.DoctorId);
            if (doc == null) return Content("Not Found");

            doc.Name = d.Name;
            doc.Specialization = d.Specialization;
            doc.Experience = d.Experience;

            _context.SaveChanges();

            return Content("Updated");
        }

        // DELETE DOCTOR
        public IActionResult DeleteDoctor(int id)
        {
            var d = _context.Doctors.Find(id);
            if (d != null)
            {
                _context.Doctors.Remove(d);
                _context.SaveChanges();
            }

            return Content("Deleted");
        }

        // VIEW ALL APPOINTMENTS
        public IActionResult AllAppointments()
        {
            return Json(_context.Appointments.ToList());
        }

        // REPORT
        public IActionResult Report()
        {
            var report = new
            {
                Patients = _context.Patients.Count(),
                Doctors = _context.Doctors.Count(),
                Appointments = _context.Appointments.Count()
            };

            return Json(report);
        }
    }
}