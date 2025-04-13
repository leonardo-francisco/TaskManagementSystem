using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectDto projectDto)
        {
            var id = await service.CreateAsync(projectDto);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {       
            await service.DeleteAsync(id);
            return NoContent();

        }
    }
}
