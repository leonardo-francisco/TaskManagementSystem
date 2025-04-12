using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Requests
{
    public sealed record CreateTaskRequest
    {
        public required string Title { get; init; }
        public string? Description { get; init; }
        public required ETaskPriority Priority { get; init; }
        public required string ProjectId { get; init; }
        public string? AssignedUserName { get; init; }
        public required string CreatedBy { get; init; }
        public List<CommentDto> Comments { get; init; } = [];
    }
}
