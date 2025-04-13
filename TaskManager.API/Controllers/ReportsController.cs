using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("performance")]
        public async Task<ActionResult<List<PerformanceReportDto>>> GetPerformanceReport([FromQuery] string userRole)
        {
            if (!string.Equals(userRole, "gerente", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Apenas usuários com a função 'gerente' podem acessar este relatório.");
            }

            var result = await _reportService.GetPerformanceReportAsync();
            return Ok(result);
        }
    }
}
