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

        // Test endpoint
        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Ok("DoseLogs controller working!");
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetDoseLogsByPatient(int patientId)
        {
            Console.WriteLine($"Fetching dose logs for patientId: {patientId}");

            var doseLogs = await _context.DoseLogs
                .Include(dl => dl.Prescription) 
                .Where(dl => dl.Prescription != null && dl.Prescription.PatientId == patientId)
                .OrderByDescending(dl => dl.ScheduledDateTime)
                .ToListAsync();

            var formattedData = doseLogs.Select(dl => new
            {
                Id = dl.DoseLogId,
                PrescriptionId = dl.PrescriptionId,  
                Date = dl.ScheduledDateTime.Date.ToString("yyyy-MM-dd"),
                Time = dl.ScheduledDateTime.ToString("hh:mm tt"),
                MedicineName = dl.Prescription.MedicineName,
                Dosage = dl.Prescription.Dosage,
                Status = dl.Status.ToString(),
                Notes = dl.Prescription.Instruction
            });


            return Ok(formattedData);
        }

        [HttpGet("adherence-summary/patient/{patientId}")]
        public async Task<IActionResult> GetAdherenceSummaryByPatient(int patientId)
        {
            var prescriptions = await _context.Prescriptions
                .Where(p => p.PatientId == patientId)
                .Select(p => new
                {
                    p.PrescriptionId,
                    p.MedicineName,
                    p.Dosage,
                    p.Instruction,
                    p.StartDate,
                    p.EndDate,
                    DoseLogs = _context.DoseLogs
                        .Where(d => d.PrescriptionId == p.PrescriptionId)
                        .OrderByDescending(d => d.ScheduledDateTime)
                        .Select(d => new
                        {
                            d.ScheduledDateTime,
                            d.Status
                        })
                        .ToList()
                })
                .ToListAsync();

            var result = prescriptions.Select(p =>
            {
                var total = p.DoseLogs.Count;
                var taken = p.DoseLogs.Count(d => d.Status == DoseStatus.Taken);
                var missed = p.DoseLogs.Count(d => d.Status == DoseStatus.Missed);
                var pending = p.DoseLogs.Count(d => d.Status == DoseStatus.Pending);
                var adherenceRate = total > 0 ? Math.Round((double)taken / total * 100) : 0;

                var recentDoses = p.DoseLogs
                    .Take(5)
                    .Select(d => new
                    {
                        Date = d.ScheduledDateTime.ToString("yyyy-MM-dd"),
                        Time = d.ScheduledDateTime.ToString("hh:mm tt"),
                        Status = d.Status.ToString()
                    });

                return new
                {
                    p.PrescriptionId,
                    p.MedicineName,
                    p.Dosage,
                    p.Instruction,
                    StartDate = p.StartDate.ToString("yyyy-MM-dd"),
                    EndDate = p.EndDate.ToString("yyyy-MM-dd"),
                    Adherence = new
                    {
                        Total = total,
                        Taken = taken,
                        Missed = missed,
                        Pending = pending,
                        Rate = adherenceRate
                    },
                    RecentDoses = recentDoses
                };
            });

            return Ok(result);
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
