using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Contracts
{
    public interface ITaskRepository
    {
        Task<TaskItem?> GetByIdAsync(string id);
        Task<IEnumerable<TaskItem>> GetByProjectIdAsync(string projectId);
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task DeleteAsync(string id);
    }
}
