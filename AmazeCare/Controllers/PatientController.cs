

//Patient CRUD
//View appointments
//Book / cancel / reschedule



using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{

    public class PatientController : Controller
    {
        private readonly AppDbContext _context;

        public PatientController(AppDbContext context)
        {
            _context = context;
        }

        // REGISTER
        //Check if phone already exists 
        //If not → add patient → save to DB
        public IActionResult Register(Patient p)
        {
            if (_context.Patients.Any(x => x.Phone == p.Phone))
                return Content("Patient already exists");

            _context.Patients.Add(p);
            _context.SaveChanges();
            return new ContentResult { Content = "Registered Successfully" };
        }

        // LOGIN
        //Match phone +password
        //If found → login success
        public IActionResult Login(string phone, string password)
        {
            var user = _context.Patients
                .FirstOrDefault(x => x.Phone == phone && x.Password == password);

            if (user == null) return Content("Invalid Credentials");

            return new ContentResult { Content = "Login Success" };
        }

        // UPDATE PROFILE
        public IActionResult Update(Patient p)
        {
            var existing = _context.Patients.Find(p.PatientId);//find paient by id
            if (existing == null) return Content("Not Found");

            existing.Name = p.Name;//update fields
            existing.Phone = p.Phone;
            existing.Gender = p.Gender;
            existing.Address = p.Address;

            _context.SaveChanges();//save changes
            return new ContentResult { Content = "Updated Successfully" };
        }

        // BOOK APPOINTMENT
        public IActionResult BookAppointment(Appointment a)
        {
            var exists = _context.Appointments.Any(x =>
                x.DoctorId == a.DoctorId &&
                x.AppointmentDate == a.AppointmentDate); // ✅ SIMPLE

            if (exists)
                return new ContentResult { Content = "Doctor not available" };

            a.Status = "Booked";

            _context.Appointments.Add(a);
            _context.SaveChanges();

            return new ContentResult { Content = "Appointment Booked" };
        }

        // VIEW APPOINTMENTS
        public IActionResult MyAppointments(int patientId)
        {
            var list = _context.Appointments
                .Where(x => x.PatientId == patientId)//show only this patients appoinment
                .ToList();

            return Json(list);
        }

        // CANCEL APPOINTMENT
        public IActionResult CancelAppointment(int id)
        {
            var a = _context.Appointments.Find(id);
            if (a == null) return Content("Not Found");

            a.Status = "Cancelled";//here we will not delete directly but update status
            _context.SaveChanges();

       
            return new ContentResult { Content = "Cancelled Successfully" };
        }

        // MEDICAL HISTORY
        public IActionResult MedicalHistory(int patientId)
        {
            var records = _context.MedicalRecords
                .Where(x => x.Appointment.PatientId == patientId)//get all medicla records of patient
                .ToList();

            return Json(records);
        }
    }
}