

using AmazeCare.API.DTOS;
using AmazeCare.API.Services;
using FluentAssertions;
using NUnit.Framework;

namespace AmazeCare.Tests
{
    [TestFixture]  // ← NUnit: marks this class as a test class
    public class AuthServiceTests : TestBase
    {
        // ── TEST 1 ─────────────────────────────────────────────
        [Test]
        [Description("Patient registers with valid data and gets a JWT token back")]
        public async Task RegisterPatient_WithValidData_ReturnsToken()
        {
            // ARRANGE — set up database and service
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            var dto = new RegisterPatientDto
            {
                FullName = "Rahul Sharma",
                Email = "rahul@test.com",
                Password = "Rahul@1234",
                Gender = "Male",
                MobileNumber = "9876543210",
                DateOfBirth = new DateTime(1995, 5, 15),
            };

            // ACT — call the method we are testing
            var result = await service.RegisterPatientAsync(dto);

            // ASSERT — check the result is what we expect
            result.Should().NotBeNull();               // result must exist
            result.Token.Should().NotBeNullOrEmpty();  // must have a token
            result.Role.Should().Be("Patient");        // role must be Patient
            result.FullName.Should().Be("Rahul Sharma"); // name must match
        }

        // ── TEST 2 ─────────────────────────────────────────────
        [Test]
        [Description("Registering with duplicate email throws an exception")]
        public async Task RegisterPatient_WithDuplicateEmail_ThrowsException()
        {
            // ARRANGE
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            // Add patient to DB first
            db.Patients.Add(CreateTestPatient(email: "duplicate@test.com"));
            await db.SaveChangesAsync();

            var dto = new RegisterPatientDto
            {
                FullName = "Another Person",
                Email = "duplicate@test.com",  // ← same email!
                Password = "Test@1234",
                Gender = "Male",
                MobileNumber = "9876543211",
                DateOfBirth = new DateTime(1990, 1, 1),
            };

            // ACT + ASSERT — expect an exception to be thrown
            var act = async () => await service.RegisterPatientAsync(dto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*already*");  // message should contain "already"
        }

        // ── TEST 3 ─────────────────────────────────────────────
        [Test]
        [Description("Patient can login with correct email and password")]
        public async Task LoginPatient_WithCorrectCredentials_ReturnsToken()
        {
            // ARRANGE
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            // Add patient to DB
            db.Patients.Add(CreateTestPatient(
                email: "rahul@test.com",
                password: "Rahul@1234"));
            await db.SaveChangesAsync();

            var dto = new LoginDto
            {
                Email = "rahul@test.com",
                Password = "Rahul@1234",
            };

            // ACT
            var result = await service.LoginPatientAsync(dto);

            // ASSERT
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("Patient");
        }

        // ── TEST 4 ─────────────────────────────────────────────
        [Test]
        [Description("Patient login with wrong password throws exception")]
        public async Task LoginPatient_WithWrongPassword_ThrowsException()
        {
            // ARRANGE
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            db.Patients.Add(CreateTestPatient(
                email: "rahul@test.com",
                password: "Rahul@1234"));
            await db.SaveChangesAsync();

            var dto = new LoginDto
            {
                Email = "rahul@test.com",
                Password = "WrongPassword!",  // ← wrong password
            };

            // ACT + ASSERT
            var act = async () => await service.LoginPatientAsync(dto);
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Invalid*");
        }

        // ── TEST 5 ─────────────────────────────────────────────
        [Test]
        [Description("Patient login with email that does not exist throws exception")]
        public async Task LoginPatient_WithNonExistentEmail_ThrowsException()
        {
            // ARRANGE — empty database, no patients added
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            var dto = new LoginDto
            {
                Email = "nobody@test.com",  // ← email not in DB
                Password = "Test@1234",
            };

            // ACT + ASSERT
            var act = async () => await service.LoginPatientAsync(dto);
            await act.Should().ThrowAsync<Exception>();
        }

        // ── TEST 6 ─────────────────────────────────────────────
        [Test]
        [Description("Doctor can login with correct credentials")]
        public async Task LoginDoctor_WithCorrectCredentials_ReturnsToken()
        {
            // ARRANGE
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            db.Doctors.Add(CreateTestDoctor(
                email: "doctor@test.com",
                password: "Doctor@123"));
            await db.SaveChangesAsync();

            var dto = new LoginDto
            {
                Email = "doctor@test.com",
                Password = "Doctor@123",
            };

            // ACT
            var result = await service.LoginDoctorAsync(dto);

            // ASSERT
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("Doctor");
        }

        // ── TEST 7 ─────────────────────────────────────────────
        [Test]
        [Description("Admin can login with correct credentials")]
        public async Task LoginAdmin_WithCorrectCredentials_ReturnsToken()
        {
            // ARRANGE
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            db.Admins.Add(CreateTestAdmin(
                email: "admin@amazecare.com",
                password: "Admin@123"));
            await db.SaveChangesAsync();

            var dto = new LoginDto
            {
                Email = "admin@amazecare.com",
                Password = "Admin@123",
            };

            // ACT
            var result = await service.LoginAdminAsync(dto);

            // ASSERT
            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.Role.Should().Be("Admin");
        }

        // ── TEST 8 ─────────────────────────────────────────────
        [Test]
        [Description("After registration, patient data is saved in database")]
        public async Task RegisterPatient_SavesPatientToDatabase()
        {
            // ARRANGE
            var db = CreateDbContext();
            var config = CreateJwtConfig();
            var service = new AuthService(db, config);

            var dto = new RegisterPatientDto
            {
                FullName = "Priya Singh",
                Email = "priya@test.com",
                Password = "Priya@1234",
                Gender = "Female",
                MobileNumber = "9876543220",
                DateOfBirth = new DateTime(1998, 8, 20),
            };

            // ACT
            await service.RegisterPatientAsync(dto);

            // ASSERT — check patient was saved in DB
            var savedPatient = db.Patients
                .FirstOrDefault(p => p.Email == "priya@test.com");

            savedPatient.Should().NotBeNull();           // patient must exist in DB
            savedPatient!.FullName.Should().Be("Priya Singh");
            savedPatient.Role.Should().Be("Patient");
            savedPatient.IsActive.Should().BeTrue();     // must be active
        }
    }
}
