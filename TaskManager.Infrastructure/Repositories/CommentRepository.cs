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
    public sealed class CommentRepository(MongoDbContext context) : ICommentRepository
    {
        private readonly IMongoCollection<Comment> _collection = context.Comments;

        public async Task AddAsync(Comment comment) => await _collection.InsertOneAsync(comment);
        public async Task DeleteAsync(string id) => await _collection.DeleteOneAsync(c => c.Id.ToString() == id);
        public async Task<IEnumerable<Comment>> GetByTaskIdAsync(string taskId) =>
            await _collection.Find(c => c.TaskId == taskId).ToListAsync();
    }
}
