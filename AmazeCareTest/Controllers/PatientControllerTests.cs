using AmazeCare.Controllers;
using AmazeCare.Data;
using AmazeCare.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace AmazeCare.Tests
{
    [TestFixture]
    public class PatientControllerTests
    {
        // database context and controller for testing
        private AppDbContext _context;
        private PatientController _controller;

        [SetUp]
        public void Setup()
        // Set up in-memory database and controller before each test
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new PatientController(_context);
        }

        //  REGISTER
        //test verifies that a new patient can register successfully.
        public void Register_Test()
        {
            // Arrange
            var p = new Patient
            {
                Name = "Test",
                Phone = "9999",
                Password = "123"
            };

           
            _controller.Register(p);

            Assert.AreEqual(1, _context.Patients.Count());
        }

        //  LOGIN
        //
        //patient can log in with valid credentials.
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

        //  BOOK APPOINTMENT
        //test verifies the patient can successfully book an appointment when the doctor is available.
        [Test]
        public void BookAppointment_Test()
        {
             
            var date = new DateTime(2025, 1, 1, 10, 0, 0);// Set up patient and doctor

            _context.Patients.Add(new Patient { PatientId = 1 });// Add a patient with ID 1
            _context.Doctors.Add(new Doctor { DoctorId = 1 });// Add a doctor with ID 1
            _context.SaveChanges();

            // Book an appointment for the patient with the doctor at the specified date and time
            var a = new Appointment
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = date,
                VisitType = "Checkup"   // IMPORTANT
            };
            // Call the BookAppointment method to attempt to book the appointment
            _controller.BookAppointment(a);

            Assert.AreEqual(1, _context.Appointments.Count());
        }

        // DOCTOR BUSY
        //This test ensures that if a doctor already has an appointment at the same time, another booking is not allowed.
        [Test]
        public void DoctorBusy_Test()
        {
            // Set up patient and doctor
            var date = new DateTime(2025, 1, 1, 10, 0, 0);

            _context.Patients.Add(new Patient { PatientId = 1 });
            _context.Patients.Add(new Patient { PatientId = 2 });
            _context.Doctors.Add(new Doctor { DoctorId = 1 });

            // Book an appointment for the first patient with the doctor at the specified date and time
            _context.Appointments.Add(new Appointment
            {
                PatientId = 1,
                DoctorId = 1,
                AppointmentDate = date,
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            // Attempt to book another appointment for the second patient with the same doctor at the same date and time
            var a = new Appointment
            {
                PatientId = 2,
                DoctorId = 1,
                AppointmentDate = date,
                VisitType = "Checkup"
            };
            // Call the BookAppointment method to attempt to book the second appointment
            _controller.BookAppointment(a);

            // Assert that only one appointment was booked for the doctor at that time, indicating that the second booking was not allowed
            Assert.AreEqual(1, _context.Appointments.Count());
        }

        // CANCEL APPOINTMENT
        //This test checks whether cancelling an appointment updates its status correctly.
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
            _controller.Dispose();
        }
    }
}