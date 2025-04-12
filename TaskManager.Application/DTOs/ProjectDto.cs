using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs
{
    public sealed record ProjectDto(string Id, string Name, string Description, string CreatedBy);
}
