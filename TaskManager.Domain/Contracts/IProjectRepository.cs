using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Contracts
{
    public interface IProjectRepository
    {
        Task<Project?> GetByIdAsync(string id);
        Task<IEnumerable<Project>> GetAllAsync();
        Task AddTaskToProjectAsync(string projectId, TaskCreated taskCreated);
        Task AddCollaboratorToProjectAsync(string projectId, string collaboratorName);
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(string id);
        Task DeleteTaskAsync(string projectId, string taskId);
    }
}
