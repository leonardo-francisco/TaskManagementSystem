using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Contracts;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Contracts;
using TaskManager.Domain.Entities;
using MongoDB.Bson;

namespace TaskManager.Application.Services
{
    public sealed class ProjectService(IProjectRepository repository) : IProjectService
    {
        public async Task<string> CreateAsync(CreateProjectRequest request)
        {
            var project = new Project
            {
                Id = ObjectId.GenerateNewId(),
                Name = request.Name,
                Description = request.Description,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            await repository.AddAsync(project);
            return project.Id.ToString()!;
        }

        public async Task DeleteAsync(string id)
        {
            await repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync() =>
            (await repository.GetAllAsync())
            .Select(p => new ProjectDto(p.Id.ToString()!, p.Name, p.Description ?? "", p.CreatedBy));

        public async Task<ProjectDto?> GetByIdAsync(string id)
        {
            var project = await repository.GetByIdAsync(id);
            return project is null ? null : new ProjectDto(project.Id.ToString()!, project.Name, project.Description ?? "", project.CreatedBy);
        }
    }
}
