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
using AutoMapper;
using TaskManager.Domain.Enums;
using TaskManager.Application.Exception;

namespace TaskManager.Application.Services
{
    public sealed class ProjectService(IProjectRepository repository,
                                       ITaskRepository taskRepository,
                                       IMapper mapper) : IProjectService
    {
        public async Task<string> CreateAsync(ProjectDto projectDto)
        {
            var project = mapper.Map<Project>(projectDto);           
            await repository.AddAsync(project);
            return project.Id.ToString()!;
        }

        public async Task DeleteAsync(string id)
        {
            var project = await repository.GetByIdAsync(id);

            if (project is null)
                throw new CustomException("Projeto não encontrado.");

            var tasks = await taskRepository.GetByProjectIdAsync(id);

            var hasPendingTasks = tasks.Any(t => t.Status == ETaskStatus.ToDo || t.Status == ETaskStatus.InProgress);
            
            if (hasPendingTasks)
                throw new CustomException("O projeto possui tarefas pendentes. Conclua ou remova as tarefas antes de excluir o projeto.");

            await repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProjectDto>> GetAllAsync()
        {
            var projects = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<ProjectDto>>(projects);
        }
     

        public async Task<ProjectDto?> GetByIdAsync(string id)
        {
            var project = await repository.GetByIdAsync(id);
            return mapper.Map<ProjectDto>(project);
        }
    }
}
