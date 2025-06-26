using backend_meditrack.Data;
using Microsoft.EntityFrameworkCore;
using backend_meditrack.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend_meditrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly ClinicDBContext _context;

        public PrescriptionsController(ClinicDBContext context)
        {
            _context = context;
        }

        [HttpGet("{patientId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions(int patientId)
        {
            return await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Include(p => p.PrescriptionDays)
                .Include(p => p.PrescriptionTimes)
                .Include(p => p.DoseLogs)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
            return Ok(prescription);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null) return NotFound();

            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}
