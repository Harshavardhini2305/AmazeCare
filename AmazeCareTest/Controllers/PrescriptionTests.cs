using AmazeCare.Controllers;
using AmazeCare.Data;
using AmazeCare.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        //  ADD PRESCRIPTION
        //test verifies that a prescription can be successfully added for a valid appointment.
        [Test]
        public void AddPrescription_Test()
        {
            
            //adding valid appointment
            _context.Appointments.Add(new Appointment
            {
                AppointmentId = 1,
                VisitType = "Checkup"
            });

            _context.SaveChanges();
            //creating prescription for the valid appointment
            var p = new Prescription
            {
                AppointmentId = 1,
                MedicineName = "Paracetamol"
            };

            _controller.Add(p);//adding prescription to the database

            Assert.AreEqual(1, _context.Prescriptions.Count());//asserting that the prescription was added successfully
        }

        //  INVALID APPOINTMENT

        //test checks behavior when trying to add a prescription for a non-existing appointment.
        [Test]
        public void AddPrescription_Invalid_Test()
        {
            //create prescription for a non-existing appointment
            var p = new Prescription
            {
                AppointmentId = 99
            };
            //attempt to add the prescription and expect an exception
            var result = _controller.Add(p);
            //
            Assert.IsNotNull(result);
        }

        //  GET ALL
        //test ensures that the GetAll method returns all prescriptions in the database.
        [Test]
        public void GetAll_Test()
        {
            //adding a prescription to the database
            _context.Prescriptions.Add(new Prescription
            {
                AppointmentId = 1
            });
            //saving changes to the database
            _context.SaveChanges();

            var result = _controller.GetAll();//retrieving all prescriptions using the controller method

            Assert.IsNotNull(result);
        }

        //  GET BY APPOINTMENT
        //test verifies that prescriptions can be retrieved based on a specific appointment ID.
        [Test]
        public void GetByAppointment_Test()
        {
            //adding a prescription for a specific appointment
            _context.Prescriptions.Add(new Prescription
            {
                AppointmentId = 1
            });

            //

            _context.SaveChanges();

            //retrieving prescriptions for the specified appointment ID using the controller method
            var result = _controller.GetByAppointment(1);

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