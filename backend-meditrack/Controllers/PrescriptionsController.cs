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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] PrescriptionDTO dto)
        {
            try
            {
                var existing = await _context.Prescriptions
                    .Include(p => p.PrescriptionDays)
                    .Include(p => p.PrescriptionTimes)
                    .FirstOrDefaultAsync(p => p.PrescriptionId == id);

                if (existing == null)
                    return NotFound();

                existing.MedicineName = dto.MedicineName;
                existing.Instruction = dto.Instruction;
                existing.Dosage = dto.Dosage;
                existing.StartDate = dto.StartDate;
                existing.EndDate = dto.EndDate;
                existing.IsRecurring = dto.IsRecurring;

                // Remove old times and days
                _context.PrescriptionDays.RemoveRange(existing.PrescriptionDays);
                _context.PrescriptionTimes.RemoveRange(existing.PrescriptionTimes);

                // Add updated days
                if (!dto.IsRecurring && dto.Days != null)
                {
                    existing.PrescriptionDays = dto.Days.Select(d => new PrescriptionDay
                    {
                        DayOfWeek = (DayOfWeek)d
                    }).ToList();
                }

                // Add updated times
                if (dto.Times != null)
                {
                    existing.PrescriptionTimes = dto.Times.Select(t => new PrescriptionTime
                    {
                        TimeOfDay = TimeSpan.Parse(t)
                    }).ToList();
                }

                await _context.SaveChangesAsync();
                return Ok(existing);
            }
            catch (Exception ex)
            {
                // Log to console
                Console.WriteLine("Update error: " + ex.Message);
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("today/{patientId}")]
        public async Task<IActionResult> GetTodayLogs(int patientId)
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            var prescriptions = await _context.Prescriptions
                .Where(p => p.PatientId == patientId && p.StartDate <= today && p.EndDate >= today)
                .Include(p => p.PrescriptionTimes)
                .Include(p => p.PrescriptionDays)
                .Include(p => p.DoseLogs)
                .ToListAsync();

            var logsToReturn = new List<DoseLog>();

            foreach (var p in prescriptions)
            {
                var times = p.PrescriptionTimes
                    .Select(pt => today.Add(pt.TimeOfDay))
                    .ToList();

                foreach (var scheduledDt in times)
                {
                    var existing = p.DoseLogs
                        .FirstOrDefault(dl => dl.ScheduledDateTime == scheduledDt);

                    if (existing == null)
                    {
                        // Add new log with default status = Pending
                        existing = new DoseLog
                        {
                            PrescriptionId = p.PrescriptionId,
                            ScheduledDateTime = scheduledDt,
                            Status = DoseStatus.Pending
                        };
                        _context.DoseLogs.Add(existing);
                        await _context.SaveChangesAsync();

                        p.DoseLogs.Add(existing);
                    }

                    // Auto-mark missed doses
                    if (existing.Status == DoseStatus.Pending && scheduledDt < now && existing.TakenTime == null)
                    {
                        existing.Status = DoseStatus.Missed;
                        await _context.SaveChangesAsync();
                    }

                    logsToReturn.Add(existing);
                }
            }

            return Ok(logsToReturn
                .Where(l => l.ScheduledDateTime.Date == today)
                .ToList());
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
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var prescription = new Prescription
            {
                MedicineName = dto.MedicineName,
                Instruction = dto.Instruction,
                Dosage = dto.Dosage,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsRecurring = dto.IsRecurring,
                PatientId = dto.PatientId,
                PrescriptionDays = new List<PrescriptionDay>(),
                PrescriptionTimes = new List<PrescriptionTime>()
            };

            // Add times
            if (dto.Times != null && dto.Times.Any())
            {
                foreach (var timeStr in dto.Times)
                {
                    if (TimeSpan.TryParse(timeStr, out var time))
                    {
                        prescription.PrescriptionTimes.Add(new PrescriptionTime
                        {
                            TimeOfDay = time
                        });
                    }
                }
            }

            // Add days (only if not recurring)
            if (!dto.IsRecurring && dto.Days != null && dto.Days.Any())
            {
                foreach (var day in dto.Days)
                {
                    prescription.PrescriptionDays.Add(new PrescriptionDay
                    {
                        DayOfWeek = (DayOfWeek)day
                    });
                }
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return Ok(prescription);
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            var prescription = await _context.Prescriptions.FindAsync(id);
            if (prescription == null)
                return NotFound();

            return Ok(prescription);
        }

    }

}
