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
    }
}
