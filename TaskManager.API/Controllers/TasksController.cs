using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs.Requests;

namespace TaskManager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(ITaskService service) : ControllerBase
    {
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetByProject(string projectId) =>
            Ok(await service.GetByProjectIdAsync(projectId));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var task = await service.GetByIdAsync(id);
            return task is null ? NotFound() : Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTaskRequest request)
        {
            var id = await service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateTaskRequest request)
        {
            if (id != request.Id) return BadRequest("Incompatibilidade de ID");
            await service.UpdateAsync(request);
            return NoContent();
        }

        [HttpDelete("{projectId}/{taskId}")]
        public async Task<IActionResult> Delete(string projectId, string taskId)
        {
            await service.DeleteAsync(projectId, taskId);

            return NoContent();
        }
    }
}
