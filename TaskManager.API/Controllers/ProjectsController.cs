using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.Services;
using TaskManager.Domain.Enums;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController(IProjectService service, ITaskService taskService) : ControllerBase
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
        public async Task<IActionResult> Post([FromBody] CreateProjectRequest request)
        {
            var id = await service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var project = await service.GetByIdAsync(id);

            if (project is null)
                return NotFound("Projeto não encontrado.");

            var tasks = await taskService.GetByProjectIdAsync(id);

            var hasPendingTasks = tasks.Any(t => t.Status == ETaskStatus.ToDo || t.Status == ETaskStatus.InProgress);
            if (hasPendingTasks)
                return BadRequest("O projeto possui tarefas pendentes. Conclua ou remova as tarefas antes de excluir o projeto.");

            await service.DeleteAsync(id);
            return NoContent();

        }
    }
}
