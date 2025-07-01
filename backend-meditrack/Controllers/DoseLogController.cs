using backend_meditrack.Data;
using backend_meditrack.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend_meditrack.Controllers
{
    [ApiController]
    [Route("api/doselog")]
    public class DoseLogController : ControllerBase
    {
        private readonly ClinicDBContext _context;

        public DoseLogController(ClinicDBContext context)
        {
            _context = context;
        }

        // POST: api/doselog/mark-taken/5
        [HttpPost("mark-taken/{doseLogId}")]
        public async Task<IActionResult> MarkDoseAsTaken(int doseLogId)
        {
            var log = await _context.DoseLogs.FindAsync(doseLogId);
            if (log == null)
                return NotFound();

            if (log.Status == DoseStatus.Taken)
                return BadRequest("Dose already marked as taken.");

            // Allow updating even if missed
            log.TakenTime = DateTime.Now;
            log.Status = DoseStatus.Taken;

            await _context.SaveChangesAsync();
            return Ok(log);
        }

        [HttpPost("undo/{doseLogId}")]
        public async Task<IActionResult> UndoDose(int doseLogId)
        {
            var log = await _context.DoseLogs.FindAsync(doseLogId);
            if (log == null)
                return NotFound();

            log.TakenTime = null;
            log.Status = DoseStatus.Pending;

            await _context.SaveChangesAsync();
            return Ok(log);
        }

        // GET: api/doselog/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetDoseLogsByPatient(int patientId)
        {
            var logs = await _context.DoseLogs
                .Include(dl => dl.Prescription)
                .Where(dl => dl.Prescription.PatientId == patientId)
                .OrderBy(dl => dl.ScheduledDateTime)
                .ToListAsync();

            return Ok(logs);
        }

        // GET: api/doselog/history/{prescriptionId}
        [HttpGet("history/{prescriptionId}")]
        public async Task<IActionResult> GetDoseHistory(int prescriptionId)
        {
            try
            {
                var logs = await _context.DoseLogs
                    .Where(dl => dl.PrescriptionId == prescriptionId &&
                                 dl.Status != DoseStatus.Pending) // Only Taken or Missed
                    .OrderBy(dl => dl.ScheduledDateTime)
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                Console.WriteLine("History fetch error: " + ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
