using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Data
{
    public sealed class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _database = client.GetDatabase(options.Value.DatabaseName);
        }

        public IMongoCollection<Project> Projects => _database.GetCollection<Project>("Projects");
        public IMongoCollection<TaskItem> Tasks => _database.GetCollection<TaskItem>("Tasks");
        public IMongoCollection<TaskHistory> TaskHistorys => _database.GetCollection<TaskHistory>("TaskHistorys");
        public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("Comments");
    }
}
