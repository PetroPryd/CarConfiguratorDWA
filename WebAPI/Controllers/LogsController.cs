using CarConfigurator.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly LogService _logService;

        public LogsController(LogService logService)
        {
            _logService = logService;
        }

        // GET /api/logs/get/{n}
        [HttpGet("get/{n}")]
        public async Task<IActionResult> GetLast(int n)
        {
            var logs = await _logService.GetLastAsync(n);
            return Ok(logs);
        }

        // GET /api/logs/count
        [HttpGet("count")]
        public async Task<IActionResult> Count()
        {
            var count = await _logService.CountAsync();
            return Ok(count);
        }
    }
}
