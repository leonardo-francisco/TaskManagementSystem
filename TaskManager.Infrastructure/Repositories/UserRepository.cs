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
    public sealed class UserRepository(MongoDbContext context) : IUserRepository
    {
        private readonly IMongoCollection<User> _collection = context.Users;

        public async Task AddAsync(User user) => await _collection.InsertOneAsync(user);
        public async Task<IEnumerable<User>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<User?> GetByIdAsync(string id) => await _collection.Find(u => u.Id.ToString() == id).FirstOrDefaultAsync();
        public async Task<User?> GetByEmailAsync(string email) => await _collection.Find(u => u.Email == email).FirstOrDefaultAsync();
        public async Task UpdateAsync(User user) =>
            await _collection.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
