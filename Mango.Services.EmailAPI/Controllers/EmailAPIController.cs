using Mango.Services.EmailAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.EmailAPI.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailAPIController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EmailAPIController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            try
            {
                await _db.Database.ExecuteSqlRawAsync("SELECT 1");
                return Ok(new { status = "Healthy", database = "Connected" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Unhealthy", error = ex.Message });
            }
        }
    }
}
