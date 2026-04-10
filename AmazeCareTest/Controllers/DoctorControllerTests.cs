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
    public class DoctorControllerTests
    {
        private AppDbContext _context;
        private DoctorController _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new DoctorController(_context);
        }

        // ✅ LOGIN SUCCESS
        [Test]
        public void Login_Test()
        {
            _context.Doctors.Add(new Doctor
            {
                Name = "Dr A",
                Password = "123"
            });

            _context.SaveChanges();

            var result = _controller.Login("Dr A", "123");

            Assert.IsNotNull(result);
        }

        // ❌ LOGIN FAIL
        [Test]
        public void Login_Fail_Test()
        {
            var result = _controller.Login("Wrong", "123");

            Assert.IsNotNull(result);
        }

        // ✅ VIEW APPOINTMENTS
        [Test]
        public void ViewAppointments_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                DoctorId = 1,
                PatientId = 1,
                AppointmentDate = DateTime.Now,
                Status = "Booked",
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var result = _controller.MyAppointments(1);

            Assert.IsNotNull(result);
        }

        // ✅ UPDATE STATUS (ACCEPT/REJECT)
        [Test]
        public void UpdateStatus_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                Status = "Booked",
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            _controller.UpdateStatus(1, "Accepted");

            var a = _context.Appointments.First();

            Assert.AreEqual("Accepted", a.Status);
        }

        // ❌ UPDATE STATUS NOT FOUND
        [Test]
        public void UpdateStatus_NotFound_Test()
        {
            var result = _controller.UpdateStatus(99, "Accepted");

            Assert.IsNotNull(result);
        }

        // ✅ ADD MEDICAL RECORD
        [Test]
        public void AddMedicalRecord_Test()
        {
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                Status = "Booked",
                VisitType = "Checkup"
            });

            _context.SaveChanges();

            var record = new MedicalRecord
            {
                AppointmentId = 1,
                Symptoms = "Fever"
            };

            _controller.AddMedicalRecord(record);

            Assert.AreEqual(1, _context.MedicalRecords.Count());

            var a = _context.Appointments.First();
            Assert.AreEqual("Completed", a.Status);
        }

        // ❌ INVALID APPOINTMENT FOR MEDICAL RECORD
        [Test]
        public void AddMedicalRecord_Invalid_Test()
        {
            var record = new MedicalRecord
            {
                AppointmentId = 99
            };

            var result = _controller.AddMedicalRecord(record);

            Assert.IsNotNull(result);
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

            _controller.AddPrescription(p);

            Assert.AreEqual(1, _context.Prescriptions.Count());
        }

        // ❌ INVALID PRESCRIPTION
        [Test]
        public void AddPrescription_Invalid_Test()
        {
            var p = new Prescription
            {
                AppointmentId = 99
            };

            var result = _controller.AddPrescription(p);

            Assert.IsNotNull(result);
        }

        [TearDown]
        public void Cleanup()
        {
            _context.Dispose();
        }
    }
}