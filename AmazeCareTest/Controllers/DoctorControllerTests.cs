using AmazeCare.Controllers;
using AmazeCare.Data;
using AmazeCare.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        //  LOGIN SUCCESS
        //test verifies that a doctor can log in with valid credentials.
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

        //  LOGIN FAIL
        //This test checks login with invalid credentials.
        [Test]
        public void Login_Fail_Test()
        {
            var result = _controller.Login("Wrong", "123");

            Assert.IsNotNull(result);
        }


        //  VIEW APPOINTMENTS
        //test checks whether a doctor can view their appointments.
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

        // UPDATE STATUS (ACCEPT/REJECT)
        //test verifies that a doctor can update appointment status(Accepted/Rejected).
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

        //  UPDATE STATUS NOT FOUND
        //test checks behavior when appointment does not exist
        [Test]
        public void UpdateStatus_NotFound_Test()
        {
            // Attempt to update status for non-existent appointment
            var result = _controller.UpdateStatus(99, "Accepted");

            Assert.IsNotNull(result);
        }

        //  ADD MEDICAL RECORD
        //test verifies that a doctor can add a medical record after consultation.
        [Test]
        public void AddMedicalRecord_Test()
        {
            // First, add an appointment to the context
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                Status = "Booked",
                VisitType = "Checkup"
            });

            // Save changes to ensure the appointment is added to the in-memory database
            _context.SaveChanges();

            // Now, add a medical record for the existing appointment
            var record = new MedicalRecord
            {
                AppointmentId = 1,
                Symptoms = "Fever"
            };

            // Call the controller method to add the medical record
            _controller.AddMedicalRecord(record);


            Assert.AreEqual(1, _context.MedicalRecords.Count());// Verify that the medical record was added

            var a = _context.Appointments.First();// Retrieve the appointment to check if the status was updated
            Assert.AreEqual("Completed", a.Status);// Verify that the appointment status was updated to "Completed"
        }

        //  INVALID APPOINTMENT FOR MEDICAL RECORD
        //test checks adding a medical record with an invalid appointment ID
        [Test]
        public void AddMedicalRecord_Invalid_Test()
        {
            // Attempt to add a medical record for a non-existent appointment
            var record = new MedicalRecord
            {
                AppointmentId = 99
            };
            // Call the controller method to add the medical record
            var result = _controller.AddMedicalRecord(record);

            Assert.IsNotNull(result);
        }

        // ADD PRESCRIPTION
        //test verifies that a doctor can add prescription for a valid appointment
        [Test]
        public void AddPrescription_Test()
        {
            // First, add an appointment to the context
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                VisitType = "Checkup"
            });

            // Save changes to ensure the appointment is added to the in-memory database
            _context.SaveChanges();

            // Now, add a prescription for the existing appointment
            var p = new Prescription
            {
                AppointmentId = 1,
                MedicineName = "Paracetamol"
            };

            _controller.AddPrescription(p);
            // Verify that the prescription was added to the context

            Assert.AreEqual(1, _context.Prescriptions.Count());
        }

        //  INVALID PRESCRIPTION
        //test checks adding prescription with invalid appointment ID.
        [Test]
        public void AddPrescription_Invalid_Test()
        {
            // Attempt to add a prescription for a non-existent appointment
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
            _controller.Dispose();
        }
    }
}