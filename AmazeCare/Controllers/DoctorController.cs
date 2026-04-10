//Manage doctor data
//View appointments
//Add consultation



using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorController(AppDbContext context)
        {
            _context = context;
        }

        // LOGIN
        public IActionResult Login(string name, string password)
        {
            var doc = _context.Doctors
                .FirstOrDefault(x => x.Name == name && x.Password == password);//login

            if (doc == null) return Content("Invalid");

            return Content("Login Success");
        }

        // VIEW APPOINTMENTS
        public IActionResult MyAppointments(int doctorId)
        {
            var list = _context.Appointments
                .Where(x => x.DoctorId == doctorId)//show appoimts for the doctor
                .ToList();

            return Json(list);
        }

        // ACCEPT / REJECT APPOINTMENT
        public IActionResult UpdateStatus(int id, string status)
        {
            var a = _context.Appointments.Find(id);
            if (a == null) return Content("Not Found");

            a.Status = status; // Accepted / Rejected
            _context.SaveChanges();

            return Content("Status Updated");
        }

        // ADD MEDICAL RECORD (CONSULTATION)
        public IActionResult AddMedicalRecord(MedicalRecord r)
        {
            var a = _context.Appointments.Find(r.AppointmentId);
            if (a == null) return Content("Invalid Appointment");

            a.Status = "Completed";

            _context.MedicalRecords.Add(r);
            _context.SaveChanges();

            return Content("Consultation Completed");
        }

        // ADD PRESCRIPTION
        public IActionResult AddPrescription(Prescription p)
        {
            var a = _context.Appointments.Find(p.AppointmentId);
            if (a == null) return Content("Invalid Appointment");

            _context.Prescriptions.Add(p);
            _context.SaveChanges();

            return Content("Prescription Added");
        }
    }
}