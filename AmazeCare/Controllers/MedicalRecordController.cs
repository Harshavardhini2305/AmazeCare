using AmazeCare.Models;
using AmazeCare.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AmazeCare.Controllers
{
    public class MedicalRecordController : Controller
    {
        private readonly AppDbContext _context;

        public MedicalRecordController(AppDbContext context)
        {
            _context = context;
        }

        // ADD RECORD
        public IActionResult Add(MedicalRecord r)
        {
            _context.MedicalRecords.Add(r);
            _context.SaveChanges();

            return Content("Medical Record Added");
        }

        // GET ALL
        public IActionResult GetAll()
        {
            return Json(_context.MedicalRecords.ToList());
        }

        // GET BY APPOINTMENT
        public IActionResult GetByAppointment(int appointmentId)
        {
            var list = _context.MedicalRecords
                .Where(x => x.AppointmentId == appointmentId)
                .ToList();

            return Json(list);
        }
    }
}