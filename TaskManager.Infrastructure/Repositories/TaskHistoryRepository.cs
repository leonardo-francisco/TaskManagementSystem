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
    public sealed class TaskHistoryRepository(MongoDbContext context) : ITaskHistoryRepository
    {
        private readonly IMongoCollection<TaskHistory> _collection = context.TaskHistorys;

        public async Task AddHistoryAsync(TaskHistory history) => await _collection.InsertOneAsync(history);

    }
}
