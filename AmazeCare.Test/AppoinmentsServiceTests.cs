
//using AmazeCare.API.DTOs;
using AmazeCare.API.DTOS;
using AmazeCare.API.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AmazeCare.Tests
{
    [TestFixture]
    public class AppointmentServiceTests : TestBase
    {
        // ── TEST 23 ────────────────────────────────────────────
        [Test]
        [Description("BookAppointment creates a new appointment with Pending status")]
        public async Task BookAppointment_WithValidData_CreatesAppointment()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            // Add patient and doctor first
            db.Patients.Add(CreateTestPatient(id: 1, email: "patient@test.com"));
            db.Doctors.Add(CreateTestDoctor(id: 1, email: "doctor@test.com"));
            await db.SaveChangesAsync();

            var dto = new CreateAppointmentDto
            {
                DoctorId = 1,
                AppointmentDate = DateTime.Now.AddDays(2),
                TimeSlot = "10:00 AM",
                Symptoms = "Chest pain and shortness of breath",
                NatureOfVisit = "General Checkup",
            };

            // ACT
            var result = await service.BookAppointmentAsync(1, dto);

            // ASSERT
            result.Should().NotBeNull();
            result.PatientId.Should().Be(1);
            result.DoctorId.Should().Be(1);
            result.Status.Should().Be("Pending");    // new appointments = Pending
            result.TimeSlot.Should().Be("10:00 AM");
        }

        // ── TEST 24 ────────────────────────────────────────────
        [Test]
        [Description("BookAppointment throws exception when doctor ID does not exist")]
        public async Task BookAppointment_WithInvalidDoctorId_ThrowsException()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            await db.SaveChangesAsync();

            var dto = new CreateAppointmentDto
            {
                DoctorId = 999,  // ← doctor does not exist!
                AppointmentDate = DateTime.Now.AddDays(1),
                TimeSlot = "10:00 AM",
                Symptoms = "Headache",
                NatureOfVisit = "General Checkup",
            };

            // ACT + ASSERT
            var act = async () => await service.BookAppointmentAsync(1, dto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Doctor*");
        }

        // ── TEST 25 ────────────────────────────────────────────
        [Test]
        [Description("GetPatientAppointments returns only that patient's appointments")]
        public async Task GetPatientAppointments_ReturnsOnlyThatPatientsData()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            // Add two patients and their appointments
            db.Patients.Add(CreateTestPatient(id: 1, email: "p1@test.com"));
            db.Patients.Add(CreateTestPatient(id: 2, email: "p2@test.com"));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            await db.SaveChangesAsync();

            // Patient 1 has 2 appointments, Patient 2 has 1
            db.Appointments.Add(CreateTestAppointment(id: 1, patientId: 1, doctorId: 1));
            db.Appointments.Add(CreateTestAppointment(id: 2, patientId: 1, doctorId: 1));
            db.Appointments.Add(CreateTestAppointment(id: 3, patientId: 2, doctorId: 1));
            await db.SaveChangesAsync();

            // ACT — get appointments for patient 1 only
            var result = await service.GetPatientAppointmentsAsync(1);

            // ASSERT
            result.Should().HaveCount(2);   // only patient 1's appointments
            result.Should().OnlyContain(a => a.PatientId == 1);
        }

        // ── TEST 26 ────────────────────────────────────────────
        [Test]
        [Description("ConfirmAppointment changes status from Pending to Confirmed")]
        public async Task ConfirmAppointment_ChangesPendingToConfirmed()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            db.Appointments.Add(CreateTestAppointment(
                id: 1, patientId: 1, doctorId: 1, status: "Pending"));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.ConfirmAsync(1);

            // ASSERT
            result.Should().BeTrue();

            // Check in DB
            var appointment = db.Appointments.Find(1);
            appointment!.Status.Should().Be("Confirmed");
        }

        // ── TEST 27 ────────────────────────────────────────────
        [Test]
        [Description("CancelAppointment changes status to Cancelled")]
        public async Task CancelAppointment_ChangeStatusToCancelled()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            db.Appointments.Add(CreateTestAppointment(
                id: 1, patientId: 1, doctorId: 1, status: "Pending"));
            await db.SaveChangesAsync();

            var cancelDto = new CancelAppointmentDto
            {
                Reason = "Patient has another commitment",
            };

            // ACT
            var result = await service.CancelAsync(1, cancelDto);

            // ASSERT
            result.Should().BeTrue();

            var appointment = db.Appointments.Find(1);
            appointment!.Status.Should().Be("Cancelled");
        }

        // ── TEST 28 ────────────────────────────────────────────
        [Test]
        [Description("RescheduleAppointment updates the date and time slot")]
        public async Task RescheduleAppointment_UpdatesDateAndTimeSlot()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            db.Appointments.Add(CreateTestAppointment(
                id: 1, patientId: 1, doctorId: 1, status: "Pending"));
            await db.SaveChangesAsync();

            var newDate = DateTime.Now.AddDays(5);
            var rescheduleDto = new RescheduleAppointmentDto
            {
                NewDate = newDate,
                NewTimeSlot = "02:00 PM",
            };

            // ACT
            var result = await service.RescheduleAsync(1, rescheduleDto);

            // ASSERT
            result.Should().NotBeNull();
            result.TimeSlot.Should().Be("02:00 PM");
            result.AppointmentDate.Date.Should().Be(newDate.Date);
        }

        // ── TEST 29 ────────────────────────────────────────────
        [Test]
        [Description("GetPatientUpcomingAppointments returns only Pending and Confirmed")]
        public async Task GetPatientUpcoming_ReturnsOnlyPendingAndConfirmed()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            await db.SaveChangesAsync();

            // Add appointments with different statuses
            var pending = CreateTestAppointment(id: 1, patientId: 1, doctorId: 1, status: "Pending");
            var confirmed = CreateTestAppointment(id: 2, patientId: 1, doctorId: 1, status: "Confirmed");
            var completed = CreateTestAppointment(id: 3, patientId: 1, doctorId: 1, status: "Completed");
            var cancelled = CreateTestAppointment(id: 4, patientId: 1, doctorId: 1, status: "Cancelled");

            db.Appointments.AddRange(pending, confirmed, completed, cancelled);
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetPatientUpcomingAppointmentsAsync(1);

            // ASSERT — only Pending and Confirmed should be returned
            result.Should().HaveCount(2);
            result.Should().OnlyContain(
                a => a.Status == "Pending" || a.Status == "Confirmed");
        }

        // ── TEST 30 ────────────────────────────────────────────
        [Test]
        [Description("GetPatientCompletedAppointments returns only Completed status")]
        public async Task GetPatientCompleted_ReturnsOnlyCompleted()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new AppointmentService(db);

            db.Patients.Add(CreateTestPatient(id: 1));
            db.Doctors.Add(CreateTestDoctor(id: 1));
            await db.SaveChangesAsync();

            db.Appointments.AddRange(
                CreateTestAppointment(id: 1, patientId: 1, doctorId: 1, status: "Completed"),
                CreateTestAppointment(id: 2, patientId: 1, doctorId: 1, status: "Completed"),
                CreateTestAppointment(id: 3, patientId: 1, doctorId: 1, status: "Pending")
            );
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetPatientCompletedAppointmentsAsync(1);

            // ASSERT
            result.Should().HaveCount(2);
            result.Should().OnlyContain(a => a.Status == "Completed");
        }
    }
}
