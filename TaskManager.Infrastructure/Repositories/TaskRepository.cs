using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Contracts;
using TaskManager.Domain.Entities;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public sealed class TaskRepository(MongoDbContext context) : ITaskRepository
    {
        private readonly IMongoCollection<TaskItem> _collection = context.Tasks;

        public async Task AddAsync(TaskItem task) => await _collection.InsertOneAsync(task);
        public async Task DeleteAsync(string id) => await _collection.DeleteOneAsync(t => t.Id.ToString() == id);
        public async Task<TaskItem?> GetByIdAsync(string id) => await _collection.Find(t => t.Id.ToString() == id).FirstOrDefaultAsync();
        public async Task<IEnumerable<TaskItem>> GetByProjectIdAsync(string projectId) =>
            await _collection.Find(t => t.ProjectId == projectId).ToListAsync();
        public async Task UpdateAsync(TaskItem task) =>
            await _collection.ReplaceOneAsync(t => t.Id == task.Id, task);
    }
}
