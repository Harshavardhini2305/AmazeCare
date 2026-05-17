
using AmazeCare.API.DTOS;
using AmazeCare.API.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AmazeCare.Tests
{
    [TestFixture]
    public class DoctorServiceTests : TestBase
    {
        // ── TEST 16 ────────────────────────────────────────────
        [Test]
        [Description("GetAllDoctors returns only active doctors with pagination")]
        public async Task GetAllDoctors_ReturnsPaginatedActiveDoctors()
        {
            // ARRANGE — add 5 doctors
            var db = CreateDbContext();
            var service = new DoctorService(db);

            for (int i = 1; i <= 5; i++)
            {
                db.Doctors.Add(CreateTestDoctor(
                    id: i,
                    email: $"doctor{i}@test.com",
                    specialty: "Cardiology"));
            }
            await db.SaveChangesAsync();

            // Request page 1 with 3 items per page
            var query = new QueryParameters
            {
                PageNumber = 1,
                PageSize = 3,
                SortBy = "name",
                SortDirection = "asc",
            };

            // ACT
            var result = await service.GetAllDoctorsAsync(query);

            // ASSERT
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(3);   // only 3 per page
            result.TotalCount.Should().Be(5);    // total is 5
            result.TotalPages.Should().Be(2);    // 5 / 3 = 2 pages
        }

        // ── TEST 17 ────────────────────────────────────────────
        [Test]
        [Description("SearchDoctors by specialty returns matching doctors only")]
        public async Task SearchDoctors_BySpecialty_ReturnsMatchingDoctors()
        {
            // ARRANGE — add doctors with different specialties
            var db = CreateDbContext();
            var service = new DoctorService(db);

            db.Doctors.AddRange(
                CreateTestDoctor(id: 1, email: "card1@test.com", specialty: "Cardiology"),
                CreateTestDoctor(id: 2, email: "card2@test.com", specialty: "Cardiology"),
                CreateTestDoctor(id: 3, email: "neuro@test.com", specialty: "Neurology")
            );
            await db.SaveChangesAsync();

            // ACT — search for Cardiology only
            var result = await service.SearchDoctorsAsync("Cardiology");

            // ASSERT
            result.Should().HaveCount(2);  // only 2 Cardiology doctors
            result.Should().OnlyContain(d => d.Specialty == "Cardiology");
        }

        // ── TEST 18 ────────────────────────────────────────────
        [Test]
        [Description("GetDoctorById returns the correct doctor")]
        public async Task GetDoctorById_WithValidId_ReturnsDoctor()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new DoctorService(db);

            db.Doctors.Add(CreateTestDoctor(
                id: 10,
                name: "Dr. Arun Kumar",
                email: "arun@test.com",
                specialty: "Cardiology"));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.GetDoctorByIdAsync(10);

            // ASSERT
            result.Should().NotBeNull();
            result!.DoctorId.Should().Be(10);
            result.FullName.Should().Be("Dr. Arun Kumar");
            result.Specialty.Should().Be("Cardiology");
        }

        // ── TEST 19 ────────────────────────────────────────────
        [Test]
        [Description("CreateDoctor adds new doctor to the database")]
        public async Task CreateDoctor_WithValidData_SavesDoctorToDb()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new DoctorService(db);

            var dto = new CreateDoctorDto
            {
                FullName = "Dr. New Doctor",
                Email = "newdoctor@test.com",
                Password = "Doctor@123",
                Specialty = "Neurology",
                ExperienceYears = 8,
                Qualification = "MD Neurology",
                Designation = "Consultant",
                MobileNumber = "9876543300",
            };

            // ACT
            var result = await service.CreateDoctorAsync(dto);

            // ASSERT
            result.Should().NotBeNull();
            result.FullName.Should().Be("Dr. New Doctor");
            result.Specialty.Should().Be("Neurology");

            // Verify saved in DB
            var count = db.Doctors.Count();
            count.Should().Be(1);
        }

        // ── TEST 20 ────────────────────────────────────────────
        [Test]
        [Description("CreateDoctor with duplicate email throws exception")]
        public async Task CreateDoctor_WithDuplicateEmail_ThrowsException()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new DoctorService(db);

            // Add doctor first
            db.Doctors.Add(CreateTestDoctor(
                id: 1,
                email: "existing@test.com"));
            await db.SaveChangesAsync();

            var dto = new CreateDoctorDto
            {
                FullName = "Another Doctor",
                Email = "existing@test.com",  // ← duplicate!
                Password = "Doctor@123",
                Specialty = "Cardiology",
                ExperienceYears = 5,
                Qualification = "MD",
                Designation = "Consultant",
                MobileNumber = "9876543301",
            };

            // ACT + ASSERT
            var act = async () => await service.CreateDoctorAsync(dto);
            await act.Should().ThrowAsync<Exception>();
        }

        // ── TEST 21 ────────────────────────────────────────────
        [Test]
        [Description("UpdateDoctor changes only the fields that are provided")]
        public async Task UpdateDoctor_WithValidData_UpdatesSuccessfully()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new DoctorService(db);

            db.Doctors.Add(CreateTestDoctor(
                id: 1,
                name: "Dr. Old Name",
                specialty: "Cardiology"));
            await db.SaveChangesAsync();

            var updateDto = new UpdateDoctorDto
            {
                FullName = "Dr. New Name",   // ← update name
                Specialty = "Neurology",      // ← update specialty
                ExperienceYears = 15,
                Qualification = "DM Neurology",
                Designation = "Senior Consultant",
                MobileNumber = "9876543400",
            };

            // ACT
            var result = await service.UpdateDoctorAsync(1, updateDto);

            // ASSERT
            result.Should().NotBeNull();
            result.FullName.Should().Be("Dr. New Name");
            result.Specialty.Should().Be("Neurology");
            result.ExperienceYears.Should().Be(15);
        }

        // ── TEST 22 ────────────────────────────────────────────
        [Test]
        [Description("DeleteDoctor performs soft delete — sets IsActive to false")]
        public async Task DeleteDoctor_SetsIsActiveToFalse()
        {
            // ARRANGE
            var db = CreateDbContext();
            var service = new DoctorService(db);

            db.Doctors.Add(CreateTestDoctor(id: 1, email: "doctor@test.com"));
            await db.SaveChangesAsync();

            // ACT
            var result = await service.DeleteDoctorAsync(1);

            // ASSERT
            result.Should().BeTrue();

            // Doctor still in DB but inactive
            var deletedDoctor = db.Doctors.Find(1);
            deletedDoctor.Should().NotBeNull();
            deletedDoctor!.IsActive.Should().BeFalse();
        }
    }
}
