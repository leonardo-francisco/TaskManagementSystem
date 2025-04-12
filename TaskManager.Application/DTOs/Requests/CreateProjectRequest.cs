using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs.Requests
{
    public sealed record CreateProjectRequest
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required string CreatedBy { get; init; }
    }
}
