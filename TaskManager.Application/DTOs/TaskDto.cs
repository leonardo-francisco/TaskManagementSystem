using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs
{
    public sealed record TaskDto(
    string Id,
    string Title,
    string Description,
    string Status,
    string Priority,
    string ProjectId,
    string AssignedUserName,
    string CreatedBy,
    List<CommentDto> Comments
);
}
