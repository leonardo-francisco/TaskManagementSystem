using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseEntity
    {
        [BsonElement("title")]
        public required string Title { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("status")]
        public ETaskStatus Status { get; set; } = ETaskStatus.ToDo;

        [BsonElement("priority")]
        public ETaskPriority Priority { get; set; } = ETaskPriority.Medium;

        [BsonElement("dueDate")]
        public DateTime? DueDate { get; set; }

        [BsonElement("comments")]
        public List<Comment> Comments { get; set; } = [];

        [BsonElement("assignedUserName")]
        public string? AssignedUserName { get; set; }

        [BsonElement("createdBy")]
        public required string CreatedBy { get; set; }

        [BsonElement("projectId")]
        public string ProjectId { get; set; } = default!;
    }
}
