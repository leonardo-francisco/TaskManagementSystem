using MongoDB.Bson;
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
    public sealed class ProjectRepository(MongoDbContext context) : IProjectRepository
    {
        private readonly IMongoCollection<Project> _collection = context.Projects;

        public async Task AddAsync(Project project) => await _collection.InsertOneAsync(project);

        public async Task AddCollaboratorToProjectAsync(string projectId, string collaboratorName)
        {
            if (!ObjectId.TryParse(projectId, out ObjectId objectId))
                return;

            var filter = Builders<Project>.Filter.Eq(p => p.Id, objectId);

            var update = Builders<Project>.Update
                .AddToSet(p => p.Collaborators, collaboratorName);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task AddTaskToProjectAsync(string projectId, TaskCreated taskCreated)
        {
            if (!ObjectId.TryParse(projectId, out ObjectId objectId))
                return;

            var filter = Builders<Project>.Filter.Eq(p => p.Id, objectId);

            var update = Builders<Project>.Update
                .AddToSet(p => p.Tasks, taskCreated)
                .AddToSet(p => p.Collaborators, taskCreated.CreatedBy);

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id) => await _collection.DeleteOneAsync(p => p.Id.ToString() == id);
        public async Task<IEnumerable<Project>> GetAllAsync() => await _collection.Find(_ => true).ToListAsync();
        public async Task<Project?> GetByIdAsync(string id) => await _collection.Find(p => p.Id.ToString() == id).FirstOrDefaultAsync();
        public async Task UpdateAsync(Project project) =>
            await _collection.ReplaceOneAsync(p => p.Id == project.Id, project);
    }
}
