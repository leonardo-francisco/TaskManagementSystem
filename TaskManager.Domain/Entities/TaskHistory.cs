using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Domain.Entities
{
    public class TaskHistory : BaseEntity
    {
        [BsonElement("taskId")]
        public string TaskId { get; set; }

        [BsonElement("fieldName")]
        public string FieldName { get; set; }

        [BsonElement("oldValue")]
        public string OldValue { get; set; }

        [BsonElement("newValue")]
        public string NewValue { get; set; }

        [BsonElement("modifiedBy")]
        public string ModifiedBy { get; set; }

        [BsonElement("modifiedAt")]
        public DateTime ModifiedAt { get; set; }
    }
}
