using AmazeCare.Controllers;
using AmazeCare.Data;
using AmazeCare.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Buffers.Text;
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

        
        

        //  GET ALL
        //test checks whether all medical records can be retrieved.
        [Test]
        public void GetAll_Test()
        //adding a sample medical record to ensure there is data to retrieve
        {
            _context.MedicalRecords.Add(new MedicalRecord
            {
                AppointmentId = 1
            });

            _context.SaveChanges();

            //calling the GetAll method to fetch all medical records
            var result = _controller.GetAll();

            Assert.IsNotNull(result);
        }

        //  GET BY APPOINTMENT
        //test verifies that medical records can be fetched based on a specific appointment ID
        [Test]
        public void GetByAppointment_Test()
        {
            //adding a medical record with a specific appointment ID 
            _context.MedicalRecords.Add(new MedicalRecord
            {
                AppointmentId = 1
            });

            _context.SaveChanges();

            //calling the GetByAppointment method with the appointment ID to retrieve the medical record
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