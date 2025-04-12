using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public class Project : BaseEntity
    {
        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("tasks")]
        public List<TaskCreated> Tasks { get; set; } = [];

        [BsonElement("collaborators")]
        public List<string> Collaborators { get; set; } = [];

        [BsonElement("createdBy")]
        public required string CreatedBy { get; set; }
    }
}
