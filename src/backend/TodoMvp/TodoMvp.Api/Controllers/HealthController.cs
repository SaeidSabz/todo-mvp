using Microsoft.AspNetCore.Mvc;

namespace TodoMvp.Api.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var response = new
            {
                status = "ok",
                timestampUtc = DateTime.UtcNow
            };

            return Ok(response);
        }
    }
}
