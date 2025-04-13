using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Requests;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Contracts
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync();
        Task<ProjectDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(ProjectDto projectDto);
        Task DeleteAsync(string id);
    }
}
