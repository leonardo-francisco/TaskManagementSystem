using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Contracts
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDto>> GetByProjectIdAsync(string projectId);
        Task<TaskDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(CreateTaskRequest request);
        Task UpdateAsync(UpdateTaskRequest request);
    }
}
