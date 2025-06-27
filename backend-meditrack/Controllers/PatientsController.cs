using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend_meditrack.Data;
using backend_meditrack.Models;

namespace backend_meditrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly ClinicDBContext _context;

        public PatientsController(ClinicDBContext context)
        {
            _context = context;
        }

        // ✅ GET /api/patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        // ✅ GET /api/patients/{id} with Doctor Details
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.Doctor)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return NotFound();

            return patient;
        }

        // ✅ POST /api/patients
        [HttpPost]
        public async Task<IActionResult> AddPatient([FromBody] Patient patient)
        {
            if (patient == null)
                return BadRequest("No patient data provided.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var doctorExists = await _context.Doctors.AnyAsync(d => d.DoctorId == patient.DoctorId);
            if (!doctorExists)
                return BadRequest($"Doctor with ID {patient.DoctorId} does not exist.");

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
        }

        // ✅ PUT /api/patients/{id} (Update Patient Profile)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient updatedPatient)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                return NotFound();

            patient.FullName = updatedPatient.FullName;
            patient.Email = updatedPatient.Email;
            patient.DateOfBirth = updatedPatient.DateOfBirth;

            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(patient);
        }
    }
}
