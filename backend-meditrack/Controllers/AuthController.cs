using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using backend_meditrack.Data;
using backend_meditrack.Models;

namespace backend_meditrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ClinicDbContext _context;
        private readonly PasswordHasher<string> _passwordHasher = new();

        public AuthController(ClinicDbContext context)
        {
            _context = context;
        }

        // ✅ Patient Registration
        [HttpPost("patient-register")]
        public async Task<IActionResult> RegisterPatient([FromBody] RegisterDto dto)
        {
            if (await _context.Patients.AnyAsync(p => p.Email == dto.Email))
                return BadRequest("Email already registered.");

            var doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == dto.DoctorId);
            if (!doctorExists)
                return BadRequest("Selected doctor does not exist.");

            var patient = new Patient
            {
                FullName = dto.FullName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                DoctorId = dto.DoctorId,
                PasswordHash = _passwordHasher.HashPassword(dto.Email, dto.Password)
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return Ok("Patient registered.");
        }

        // ✅ Patient Login
        [HttpPost("patient-login")]
        public async Task<IActionResult> LoginPatient([FromBody] LoginDto dto)
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Email == dto.Email);
            if (patient == null)
                return Unauthorized("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(dto.Email, patient.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Invalid email or password.");

            return Ok("Patient login successful.");
        }

        // ✅ Doctor Registration
        [HttpPost("doctor-register")]
        public async Task<IActionResult> RegisterDoctor([FromBody] RegisterDoctorDto dto)
        {
            if (await _context.Doctors.AnyAsync(d => d.Email == dto.Email))
                return BadRequest("Email already registered.");

            var doctor = new Doctor
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Specialty = dto.Specialty,
                PasswordHash = _passwordHasher.HashPassword(dto.Email, dto.Password)
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok("Doctor registered.");
        }

        // ✅ Doctor Login
        [HttpPost("doctor-login")]
        public async Task<IActionResult> LoginDoctor([FromBody] LoginDto dto)
        {
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Email == dto.Email);
            if (doctor == null)
                return Unauthorized("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(dto.Email, doctor.PasswordHash, dto.Password);
            if (result != PasswordVerificationResult.Success)
                return Unauthorized("Invalid email or password.");

            return Ok("Doctor login successful.");
        }


        [HttpGet("search-doctors")]
        public async Task<IActionResult> SearchDoctors([FromQuery] string query)
        {
            var matches = await _context.Doctors
                .Where(d => d.FullName.Contains(query))
                .Select(d => new { d.DoctorId, d.FullName })
                .Take(10)
                .ToListAsync();

            if (matches.Count == 0)
                return NotFound("No matching doctors found.");

            return Ok(matches);
        }

    }



    // ✅ DTO Classes
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int DoctorId { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDoctorDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Specialty { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
