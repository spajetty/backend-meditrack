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
        public async Task<IActionResult> GetPrescriptions(int patientId)
        {
            try
            {
                var prescriptions = await _context.Prescriptions
                    .Where(p => p.PatientId == patientId)
                    .Include(p => p.PrescriptionDays)
                    .Include(p => p.PrescriptionTimes)
                    .Include(p => p.DoseLogs)
                    .ToListAsync();

                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 ERROR: " + ex.ToString());
                return StatusCode(500, ex.Message); // return full error to frontend for now
            }
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

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prescription = new Prescription
            {
                MedicineName = dto.MedicineName,
                Instruction = dto.Instruction,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsRecurring = dto.IsRecurring,
                RecurringIntervalHours = dto.IsRecurring ? dto.RecurringIntervalHours : null,
                PatientId = dto.PatientId,
                PrescriptionDays = null,
                PrescriptionTimes = null,
                DoseLogs = null
            };

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }
    }

}
