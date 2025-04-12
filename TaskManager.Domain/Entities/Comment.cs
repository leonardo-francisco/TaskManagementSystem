using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public class Comment : BaseEntity
    {
        [BsonElement("taskId")]
        public required string TaskId { get; set; }

        [BsonElement("authorId")]
        public required string AuthorName { get; set; }

        [BsonElement("content")]
        public required string Content { get; set; }
    }
}
