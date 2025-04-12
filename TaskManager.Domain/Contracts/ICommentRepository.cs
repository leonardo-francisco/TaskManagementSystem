using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Contracts
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByTaskIdAsync(string taskId);
        Task AddAsync(Comment comment);
        Task DeleteAsync(string id);
    }
}
