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
    public class PrescriptionControllerTests
    {
        private AppDbContext _context;
        private PrescriptionController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new PrescriptionController(_context);
        }

        // ✅ ADD PRESCRIPTION
        [Test]
        public void AddPrescription_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var p = new Prescription
            {
                AppointmentId = 1,
                MedicineName = "Paracetamol"
            };

            _controller.Add(p);

            Assert.AreEqual(1, _context.Prescriptions.Count());
        }

        // ❌ INVALID APPOINTMENT
        [Test]
        public void AddPrescription_Invalid_Test()
        {
            var p = new Prescription
            {
                AppointmentId = 99
            };

            var result = _controller.Add(p);

            Assert.IsNotNull(result);
        }

        // ✅ GET ALL
        [Test]
        public void GetAll_Test()
        {
            _context.Prescriptions.Add(new Prescription
            {
                AppointmentId = 1
            });

            _context.SaveChanges();

            var result = _controller.GetAll();

            Assert.IsNotNull(result);
        }

        // ✅ GET BY APPOINTMENT
        [Test]
        public void GetByAppointment_Test()
        {
            _context.Prescriptions.Add(new Prescription
            {
                AppointmentId = 1
            });

            _context.SaveChanges();

            var result = _controller.GetByAppointment(1);

            Assert.IsNotNull(result);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}