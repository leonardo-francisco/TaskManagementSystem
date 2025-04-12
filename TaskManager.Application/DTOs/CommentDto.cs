using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs
{
    public sealed record CommentDto(
    string TaskId,
    string Content,
    string AuthorName,
    DateTime CreatedAt
);
}
