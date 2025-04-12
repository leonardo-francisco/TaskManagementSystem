using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public sealed class TaskCreated
    {
        [BsonElement("taskId")]
        public required string TaskId { get; set; }

        [BsonElement("createdBy")]
        public required string CreatedBy { get; set; }
    }
}
