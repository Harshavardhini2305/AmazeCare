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
    public class MedicalRecordControllerTests
    {
        private AppDbContext _context;
        private MedicalRecordController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new MedicalRecordController(_context);
        }

        // ✅ ADD RECORD
        [Test]
        public void AddRecord_Test()
        {
            var r = new MedicalRecord
            {
                AppointmentId = 1,
                Symptoms = "Fever"
            };

            _controller.Add(r);

            Assert.AreEqual(1, _context.MedicalRecords.Count());
        }

        // ✅ GET ALL
        [Test]
        public void GetAll_Test()
        {
            _context.MedicalRecords.Add(new MedicalRecord
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
            _context.MedicalRecords.Add(new MedicalRecord
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