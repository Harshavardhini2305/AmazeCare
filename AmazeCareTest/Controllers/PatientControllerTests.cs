using NUnit.Framework;
using AmazeCare.Models;
using AmazeCare.Data;
using AmazeCare.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AmazeCare.Tests
{
    [TestFixture]
    public class PatientControllerTests
    {
        private AppDbContext _context;
        private PatientController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new PatientController(_context);
        }

        // ✅ REGISTER
        [Test]
        public void Register_Test()
        {
            var p = new Patient
            {
                Name = "Test",
                Phone = "9999",
                Password = "123"
            };

            _controller.Register(p);

            Assert.AreEqual(1, _context.Patients.Count());
        }

        // ✅ LOGIN
        [Test]
        public void Login_Test()
        {
            _context.Patients.Add(new Patient
            {
                Phone = "9999",
                Password = "123"
            });

            _context.SaveChanges();

            var result = _controller.Login("9999", "123");

            Assert.IsNotNull(result);
        }

        // ✅ BOOK APPOINTMENT
        [Test]
        public void BookAppointment_Test()
        {
            var date = new DateTime(2025, 1, 1, 10, 0, 0);

            _context.Patients.Add(new Patient { PatientId = 1 });
            _context.Doctors.Add(new Doctor { DoctorId = 1 });
            _context.SaveChanges();

            var a = new Appointment
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = date,
                VisitType = "Checkup"   // IMPORTANT
            };

            _controller.BookAppointment(a);

            Assert.AreEqual(1, _context.Appointments.Count());
        }

        // ❌ DOCTOR BUSY
        [Test]
        public void DoctorBusy_Test()
        {
            var date = new DateTime(2025, 1, 1, 10, 0, 0);

            _context.Patients.Add(new Patient { PatientId = 1 });
            _context.Patients.Add(new Patient { PatientId = 2 });
            _context.Doctors.Add(new Doctor { DoctorId = 1 });

            _context.Appointments.Add(new Appointment
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = date,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var a = new Appointment
            {
                PatientId = 2,
                DoctorId = 1,
                AppointmentDate = date,
                VisitType = "Checkup"
            };

            _controller.BookAppointment(a);

            Assert.AreEqual(1, _context.Appointments.Count());
        }

        // ✅ CANCEL APPOINTMENT
        [Test]
        public void CancelAppointment_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                Status = "Booked",
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            _controller.CancelAppointment(1);

            var a = _context.Appointments.First();

            Assert.AreEqual("Cancelled", a.Status);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}