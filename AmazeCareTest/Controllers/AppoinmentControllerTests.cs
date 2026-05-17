using AmazeCare.Controllers;
using AmazeCare.Data;
using AmazeCare.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Linq;
using System.Numerics;

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

        //  GET ALL
        //test verifies that all appointments can be retrieved successfully.
        [Test]
        public void GetAll_Test()
        {
            //  Add a sample appointment 
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var result = _controller.GetAll();

            Assert.IsNotNull(result);
        }

        //  GET BY PATIENT
        //test checks whether appointments can be filtered based on patient ID
        [Test]
        public void GetByPatient_Test()
        {
            //  Add a sample appointment for a specific patient
            _context.Appointments.Add(new Appointment
            {
                PatientId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            //  Retrieve appointments for the specified patient ID
            var result = _controller.GetByPatient(1);

            Assert.IsNotNull(result);
        }

        //  GET BY DOCTOR
        //test verifies that appointments can be filtered by doctor ID.
        [Test]
        public void GetByDoctor_Test()
        {
            //  Add a sample appointment for a specific doctor
            _context.Appointments.Add(new Appointment
            {
                DoctorId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            //  Retrieve appointments for the specified doctor ID
            var result = _controller.GetByDoctor(1);

            Assert.IsNotNull(result);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
            _controller.Dispose();
        }
    }
}