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
    public class AppointmentControllerTests
    {
        private AppDbContext _context;
        private AppointmentController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new AppointmentController(_context);
        }

        // ✅ GET ALL
        [Test]
        public void GetAll_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var result = _controller.GetAll();

            Assert.IsNotNull(result);
        }

        // ✅ GET BY PATIENT
        [Test]
        public void GetByPatient_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                PatientId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var result = _controller.GetByPatient(1);

            Assert.IsNotNull(result);
        }

        // ✅ GET BY DOCTOR
        [Test]
        public void GetByDoctor_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                DoctorId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var result = _controller.GetByDoctor(1);

            Assert.IsNotNull(result);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}