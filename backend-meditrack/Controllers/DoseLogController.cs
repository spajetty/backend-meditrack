using backend_meditrack.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_meditrack.Controllers
{
    [ApiController]
    [Route("api/doselogs")]
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
                .Include(dl => dl.Prescription)  // 👈 This is the important fix
                .Where(dl => dl.Prescription != null && dl.Prescription.PatientId == patientId)
                .OrderByDescending(dl => dl.ScheduledDateTime)
                .ToListAsync();

            var formattedData = doseLogs.Select(dl => new
            {
                Id = dl.DoseLogId,
                Date = dl.ScheduledDateTime.Date.ToString("yyyy-MM-dd"),
                Time = dl.ScheduledDateTime.ToString("hh:mm tt"),
                MedicineName = dl.Prescription.MedicineName,
                Dosage = dl.Prescription.Instruction,
                Status = dl.Status.ToString(),
                Notes = "",
                Effectiveness = 0
            });

            return Ok(formattedData);
        }






    }
}
