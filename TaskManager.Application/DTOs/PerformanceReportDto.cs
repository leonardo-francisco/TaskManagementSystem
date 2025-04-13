using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.DTOs
{
    public sealed record PerformanceReportDto(
    string UserName,
    int TasksCompleted,
    double AverageTasksPerDay
);
}
