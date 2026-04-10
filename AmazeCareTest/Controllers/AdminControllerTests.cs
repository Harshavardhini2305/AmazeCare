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
    public class AdminControllerTests
    {
        private AppDbContext _context;
        private AdminController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new AdminController(_context);
        }

        // ✅ LOGIN SUCCESS
        [Test]
        public void Login_Test()
        {
            _context.Admins.Add(new Admin
            {
                Username = "admin",
                Password = "123"
            });

            _context.SaveChanges();

            var result = _controller.Login("admin", "123");

            Assert.IsNotNull(result);
        }

        // ❌ LOGIN FAIL
        [Test]
        public void Login_Fail_Test()
        {
            var result = _controller.Login("wrong", "123");

            Assert.IsNotNull(result);
        }

        // ✅ ADD DOCTOR
        [Test]
        public void AddDoctor_Test()
        {
            var d = new Doctor
            {
                Name = "Dr A"
            };

            _controller.AddDoctor(d);

            Assert.AreEqual(1, _context.Doctors.Count());
        }

        // ✅ UPDATE DOCTOR
        [Test]
        public void UpdateDoctor_Test()
        {
            _context.Doctors.Add(new Doctor
            {
                DoctorId = 1,
                Name = "Old"
            });

            _context.SaveChanges();

            var d = new Doctor
            {
                DoctorId = 1,
                Name = "New"
            };

            _controller.UpdateDoctor(d);

            var doc = _context.Doctors.First();

            Assert.AreEqual("New", doc.Name);
        }

        // ❌ UPDATE NOT FOUND
        [Test]
        public void UpdateDoctor_NotFound_Test()
        {
            var d = new Doctor { DoctorId = 99 };

            var result = _controller.UpdateDoctor(d);

            Assert.IsNotNull(result);
        }

        // ✅ DELETE DOCTOR
        [Test]
        public void DeleteDoctor_Test()
        {
            _context.Doctors.Add(new Doctor { DoctorId = 1 });

            _context.SaveChanges();

            _controller.DeleteDoctor(1);

            Assert.AreEqual(0, _context.Doctors.Count());
        }

        // ✅ VIEW ALL APPOINTMENTS
        [Test]
        public void AllAppointments_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var result = _controller.AllAppointments();

            Assert.IsNotNull(result);
        }

        // ✅ REPORT
        [Test]
        public void Report_Test()
        {
            _context.Patients.Add(new Patient());
            _context.Doctors.Add(new Doctor());
            _context.Appointments.Add(new Appointment { VisitType = "Checkup" });

            _context.SaveChanges();

            var result = _controller.Report();

            Assert.IsNotNull(result);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}