using backend_meditrack.Data;
using backend_meditrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_meditrack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly ClinicDBContext _context;

        public DoctorController(ClinicDBContext context)
        {
            _context = context;
        }

        // ✅ GET /api/doctor (get all doctors)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            return await _context.Doctors.ToListAsync();
        }

        // ✅ GET /api/doctor/{id} (get doctor by ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            return doctor;
        }

        // ✅ PUT /api/doctor/{id} - Update an existing doctor
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorUpdateDTO updatedDoctor)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.FullName = updatedDoctor.FullName;
            doctor.Email = updatedDoctor.Email;
            doctor.Specialty = updatedDoctor.Specialty;

            _context.Entry(doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(doctor);
        }

        // ✅ DELETE /api/doctor/{id} - Delete a doctor
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                return NotFound();

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return Ok($"Doctor with ID {id} deleted.");
        }
    }

}
